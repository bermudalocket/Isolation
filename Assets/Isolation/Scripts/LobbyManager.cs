using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts {
    
    public class LobbyManager: NetworkBehaviour {

        private static readonly int SkyboxRotationProp = Shader.PropertyToID("_Rotation");

        private int _tick;
        private void Update() {
            if (_tick++ % 5 == 0) {
                RenderSettings.skybox.SetFloat(SkyboxRotationProp, Time.time);
            }
        }
        
        private void Awake() {
            if (IsServer) {
                GetComponent<NetworkObject>().Spawn(true);
            }
        }

        private void Start() {
            if (GameState.Stage.Value == GameStage.PostGame) {
                UniTask.Void(async () => {
                    await UniTask.Delay(1_000);
                    UIManager.Instance.Open(ScreenType.PostGame);
                });
            }
        }
        
        private void OnEnable() {
            // only the host needs to tell the server to spawn a player
            if (IsHost) {
                Debug.Log($"[LobbyManager] OnEnable - spawning player prefab for host");
                Services.Network.OnPlayerJoined += OnPlayerJoined;
            }
        }
        
        private void OnDisable() {
            if (IsHost) {
                Services.Network.OnPlayerJoined -= OnPlayerJoined;
            }
        }

        private void OnPlayerJoined(ulong uid) => SpawnPlayerServerRPC(uid);
        
        [ServerRpc]
        private void SpawnPlayerServerRPC(ulong id) {
            Debug.Log($"[LobbyManager] Spawning player prefab for client uid {id}");
            var player = Addressables.InstantiateAsync("Isolation/Player", Vector3.zero, Quaternion.identity)
                                     .WaitForCompletion();
            var networkObject = player.GetComponent<NetworkObject>();
            networkObject
                  .SpawnAsPlayerObject(id, destroyWithScene: false);
            DontDestroyOnLoad(networkObject.gameObject);
        }

        ~LobbyManager() {
            Debug.Log($"[LobbyManager] Deinit.");
        }

    }
    
}