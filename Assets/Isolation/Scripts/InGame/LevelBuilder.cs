using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

namespace Isolation.Scripts.InGame {

    public partial class GameManager {

        [SerializeField]
        private LoadingViewController _loadingViewController;

        public delegate void LevelBuilderCompleteHandler();
        public static event LevelBuilderCompleteHandler LevelBuilderCompleteEvent;

        private static void Log(string message) {
            Debug.Log($"[LevelBuilder] {message}");
        }

        private readonly Dictionary<Vector3, RoomType> _rooms = new() {
            [Vector3.zero] = RoomType.Start,
            [Vector3.right] = RoomType.Plus,
        };

        private void BuildLevel() {
            _loadingViewController.Show();

            Debug.Log($"[LevelBuilder] Started.");
            var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Debug.Log($"[LevelBuilder] Using seed <b>{seed}</b>.");
            var random = new Random(seed);
            
            var curPos = Vector3.right;
            var roomsWithUnusedExits = new List<Vector3>();
            var maxRooms = GameState.Difficulty.Value.MaxRooms();
            Debug.Log($"[LevelBuilder] Need to create <b>{maxRooms}</b> rooms.");

            var generateTimer = Stopwatch.StartNew();
            // --------------------------------------------------------------------
            // Map spine
            while (_rooms.Count <= maxRooms) {
                var currentHallwayType = _rooms[curPos];

                // find all exits that don't have a room yet
                var eligibleExits = new List<Vector3>();
                Debug.Log("[LevelBuilder] curType = " + currentHallwayType);
                foreach (var direction in currentHallwayType.Exits()) {
                    if (_rooms.ContainsKey(curPos + direction)) {
                        continue;
                    }
                    eligibleExits.Add(direction);
                }
                
                if (eligibleExits.Count > 0) {
                    // pick an exit
                    var exit = eligibleExits.RandomElement(random);

                    // remove it from the eligibility list
                    eligibleExits.Remove(exit);

                    // if this room has more unused exits, remember them for later
                    if (eligibleExits.Count > 0) {
                        roomsWithUnusedExits.Add(curPos);
                    }

                    // step in the chosen direction and choose a hallway type
                    curPos += exit;
                    var type = RoomTypeHelper.RoomsThatOpen(-exit).RandomElement(random);
                    SpawnRoom(type, 12f * curPos, Quaternion.identity);
                    _rooms[curPos] = type;
                } else {
                    // if none, go back to one of the unused exits
                    // (or, rarely, force our way forward)
                    if (roomsWithUnusedExits.Count == 0) {
                        _rooms[curPos] = RoomType.Plus;
                        continue;
                    }
                    curPos = roomsWithUnusedExits.First();
                    roomsWithUnusedExits.Remove(curPos);
                }
            }

            // --------------------------------------------------------------------
            // rooms
            var hallways = _rooms.Keys.ToArray();
            var powerRoomExists = false;
            Log("Processing rooms");
            foreach (var hallway in hallways) {
                var exits = _rooms[hallway].Exits();
                Log($"Processing <b>{hallway}</b>. Exits: <b>{exits.Description()}</b>");
                foreach (var exit in exits) {
                    // if there's something here, skip or cap
                    if (_rooms.ContainsKey(hallway + exit)) {
                        var otherRoom = _rooms[hallway + exit];
                        if (!otherRoom.Exits().Contains(-exit)) {
                            // cap
                            var rotation = Quaternion.identity;
                            if (exit == Vector3.forward) {
                                rotation = Quaternion.identity;
                            } else if (exit == Vector3.back) {
                                rotation = Quaternion.Euler(0f, 180f, 0f);
                            } else if (exit == Vector3.right) {
                                rotation = Quaternion.Euler(0f, 90f, 0f);
                            } else if (exit == Vector3.left) {
                                rotation = Quaternion.Euler(0f, 270f, 0f);
                            }
                            SpawnCap(12f * hallway, rotation);
                        }
                        continue;
                    }
                    var type = RoomTypeHelper
                        .ByCategory(RoomCategory.Room)
                        .Where(r => r.Limit() > _rooms.Count(sr => sr.Value == r))
                        .RandomElement(random);
                    if (type == RoomType.Power) {
                        powerRoomExists = true;
                    }
                    var roomPos = hallway + exit;
                    SpawnRoom(type, 12f * roomPos, Quaternion.LookRotation(exit));
                    _rooms[roomPos] = type;
                    Debug.Log($"[LevelBuilder] Spawned <b>{type}</b>");
                }
            }
            if (!powerRoomExists) {
                var replaceables = RoomTypeHelper.ByCategory(RoomCategory.Room);
                var toReplace = FindObjectsOfType<Room>()
                    .Where(r => replaceables.Contains(r.Type))
                    .RandomElement(random);
                var rotation = toReplace.transform.rotation;
                
                DestroyServerRPC(toReplace.GetComponent<NetworkObject>().NetworkObjectId);

                SpawnRoom(RoomType.Power, toReplace.WorldPos, rotation);
                _rooms[new Vector3(toReplace.i, 0f, toReplace.j)] = RoomType.Power;
            }

            generateTimer.Stop();
            Debug.Log($"[LevelBuilder] Took <b>{generateTimer.ElapsedMilliseconds} ms</b> to generate.");
            
            MinimapCamera.Instance.FindCentralPosition();

            UniTask.Void(BakeLevel, this.GetCancellationTokenOnDestroy());
        }
        
        // --------------------------------------------------------------------

        private void SpawnRoom(RoomType type, Vector3 pos, Quaternion rotation) {
            if (Services.Production) {
                SpawnRoomServerRPC(type, pos, rotation);
            } else {
                SpawnRoomImpl(type, pos, rotation);
            }
        }

        [ServerRpc]
        private void SpawnRoomServerRPC(RoomType type, Vector3 pos, Quaternion rotation) {
            SpawnRoomImpl(type, pos, rotation);
        }
        
        private void SpawnRoomImpl(RoomType type, Vector3 pos, Quaternion rotation) {
            var go = Addressables.InstantiateAsync(type.AddressKey(), pos, rotation).WaitForCompletion();
            var roomType = RoomTypeHelper.ByCategory(RoomCategory.Hallway).Contains(type) ? "Hallway" : "Room";
            go.name = $"{roomType} ({pos.x}, {pos.z})";
            var roomInfo = go.AddComponent<Room>();
            roomInfo.i = pos.x;
            roomInfo.j = pos.z;
            roomInfo.Type = type;
            go.layer = LayerMask.NameToLayer("Room");
            if (IsServer) {
                go.GetComponent<NetworkObject>().Spawn();
            }
        }
        
        // --------------------------------------------------------------------
        
        private void SpawnCap(Vector3 pos, Quaternion rotation) {
            if (Services.Production) {
                SpawnCapServerRPC(pos, rotation);
            } else {
                var go = Addressables.InstantiateAsync("Cap-N", pos, rotation).WaitForCompletion();
                go.name = $"Cap ({pos})";
            }
        }

        [ServerRpc]
        private void SpawnCapServerRPC(Vector3 pos, Quaternion rotation) {
            Debug.Log($"[ServerRPC - SpawnCapServerRPC] " + pos);
            var go = Addressables.InstantiateAsync("Cap-N", pos, rotation).WaitForCompletion();
            go.name = $"Cap ({pos})";
            go.GetComponent<NetworkObject>().Spawn();
        }
        
        // --------------------------------------------------------------------

        [ServerRpc]
        private void DestroyServerRPC(ulong key) {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(key, out var go)) {
                go.GetComponent<NetworkObject>().Despawn();
            }
        }
        
        // --------------------------------------------------------------------

        private async UniTaskVoid BakeLevel(CancellationToken token) {
            Debug.Log("[LevelBuilder] Baking level.");
            var bakeTimer = Stopwatch.StartNew();
            var surfaces = FindObjectsOfType<NavMeshSurface>();

            var i = 0;
            var totalSurfaces = surfaces.Length;
            
            foreach (var surface in surfaces) {
                if (token.IsCancellationRequested) return;
                try {
                    surface.BuildNavMesh();
                    i += 1;
                    Debug.Log(i);
                    if (i > 0.9f * totalSurfaces) {
                        _loadingViewController.SetStage(LoadingViewController.Stage.Fifth);
                    } else if (i > 0.8f * totalSurfaces) {
                        _loadingViewController.SetStage(LoadingViewController.Stage.Fourth);
                    } else if (i > 0.45f * totalSurfaces) {
                        _loadingViewController.SetStage(LoadingViewController.Stage.Third);
                    } else if (i > 0.2f * totalSurfaces) {
                        _loadingViewController.SetStage(LoadingViewController.Stage.Second);
                    } else if (i > 0.1f * totalSurfaces) {
                        _loadingViewController.SetStage(LoadingViewController.Stage.First);
                    }
                } catch (Exception e) {
                    Log($"Exception baking nav mesh: {e.Message}.");
                }
                await UniTask.Yield();
            }
            bakeTimer.Stop();
            Debug.Log($"[LevelBuilder] Baking rooms took <b>{bakeTimer.ElapsedMilliseconds} ms</b>.");

            SummonAntagonist();
            SummonEquipment();

            LevelBuilderCompleteEvent?.Invoke();
            
            var cancelToken = this.GetCancellationTokenOnDestroy();
            UniTask.Void(DoSafeTimeCountdown, cancelToken);

            _loadingViewController.Hide();
        }
        
        // --------------------------------------------------------------------

        private void SummonEquipment() {
            if (Services.Production) {
                SummonEquipmentServerRPC();
            } else {
                SummonEquipmentImpl();
            }
        }

        [ServerRpc]
        private void SummonEquipmentServerRPC() => SummonEquipmentImpl();

        private void SummonEquipmentImpl() {
            var players = Services.Network.Connections();
            var tables = FindObjectsOfType<EquipmentTable>();
            for (var i = 0; i < Constants.MaxPlayers && i < players.Count; i++) {
                var player = players[i];
                var state = player.GetNetworkState();
                if (state is not { } playerState) {
                    continue;
                }
                var table = tables[i];
                var j = 0;
                foreach (var type in EquipmentTypeExtensions.All) {
                    var spot = table.transform.GetChild(j);
                    var go = Addressables
                             .InstantiateAsync(type.Key(), spot.transform.position, Quaternion.identity)
                             .WaitForCompletion();
                    
                    var equip = go.GetComponent<AbstractEquipment>();
                    Assert.IsNotNull(equip);
                    equip.Level = playerState.GetLevel(type).Value;
                    
                    if (IsServer) {
                        go.GetComponent<NetworkObject>()
                          .SpawnWithOwnership(player.ClientId, destroyWithScene: true);
                    }
                    j++;
                }
            }
        }
        
        // --------------------------------------------------------------------

        private void SummonAntagonist() {
            if (Services.Production) {
                SummonAntagonistServerRPC();
            } else {
                SummonAntagonistImpl();
            }
        }

        [ServerRpc]
        private void SummonAntagonistServerRPC() => SummonAntagonistImpl();

        private void SummonAntagonistImpl() {
            var type = AntagonistType.Random();
            GameState.Answer.Value = type.NetworkTyped();
            var model = AntagonistModel.Crawler; // TODO
            
            var eligibleRooms = (
                from room in FindObjectsOfType<Room>() 
                where RoomTypeHelper.SpecialRoomTypes.Contains(room.Type) 
                select room.transform.position
            ).ToList();
            var pos = eligibleRooms.RandomElement();
            Debug.Log($"[SummonAntagonist] Summoning antagonist at <b>{pos}</b>.");
            var go = Addressables.InstantiateAsync(model.AddressKey, pos, Quaternion.identity).WaitForCompletion();
            if (NavMesh.SamplePosition(pos, out var hit, 100f, -1)) {
                Debug.Log($"[SummonAntagonist] Found hit: {hit.position}");
                go.transform.position = hit.position;
            }
            if (IsServer) {
                go.GetComponent<NetworkObject>().Spawn();
            }
        }
        
        // --------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init() {
            LevelBuilderCompleteEvent = null;
        }

    }
    
}