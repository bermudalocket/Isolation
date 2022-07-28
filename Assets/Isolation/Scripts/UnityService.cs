using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace Isolation.Scripts {

    public interface IUnityService {
                
        public Lobby Lobby { get; }
        
        public UniTask<bool> HostRelayServer();

        public UniTask<bool> JoinRelayServer(string joinCode);

    }

    public sealed class UnityService: IUnityService {

        public Lobby Lobby { get; private set; }

        public async UniTask<bool> JoinRelayServer(string joinCode) {
            var allocation = await Relay.Instance.JoinAllocationAsync(joinCode).ConfigureAwait(false);
            var data = new RelayData {
                Key = allocation.Key,
                Port = (ushort) allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };
            Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
            return Services.Network.BindToRelayServer(data);
        }
        
        public async UniTask<bool> HostRelayServer() {
            Debug.Log("Allocating...");
            try {
                var allocation = await Relay.Instance.CreateAllocationAsync(4);
                Debug.Log("Creating RelayData...");
                var data = new RelayData {
                    Key = allocation.Key,
                    Port = (ushort) allocation.RelayServer.Port,
                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    ConnectionData = allocation.ConnectionData,
                    HostConnectionData = allocation.ConnectionData,
                    IPv4Address = allocation.RelayServer.IpV4
                };
                Debug.Log("Getting join code...");
                data.JoinCode = await Relay.Instance.GetJoinCodeAsync(data.AllocationID);
                Debug.Log(data.JoinCode);
                Debug.Log("Creating Lobby...");
                Lobby = await Lobbies.Instance.CreateLobbyAsync(data.JoinCode, 4, new CreateLobbyOptions {
                    IsPrivate = true,
                });
                UniTask.Void(HeartbeatLobby);
                Debug.Log("Binding...");
                return Services.Network.BindToRelayServer(data);
            } catch (Exception e) {
                Debug.Log($"[UnityService] Caught exception while hosting relay server: {e.Message}");
                return false;
            }
        }

        private async UniTaskVoid HeartbeatLobby() {
            while (true) {
                await Lobbies.Instance.SendHeartbeatPingAsync(Lobby.Id);
                await UniTask.Delay(15_000);
            }
        }

        private async UniTaskVoid CleanUp() {
            if (Lobby is { Id: var id }) {
                await Lobbies.Instance.DeleteLobbyAsync(id);
                Debug.Log($"[UnityService] Deleted lobby {id}.");
            }
        }

        ~UnityService() {
            UniTask.Void(CleanUp);
        }
        
    }

}