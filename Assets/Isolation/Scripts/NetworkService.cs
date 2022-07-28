using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Isolation.Scripts {

    public interface INetworkService {
        
        public bool IsHost { get; }

        public Result<ulong> ClientId { get; }

        public bool BindToRelayServer(RelayData relayData);

        public Result<PlayerNetworkState> GetLocalNetworkState();

        public void Load(string scene) {
            SceneManager.LoadScene(scene);
        }

        public IReadOnlyList<NetworkClient> Connections();

        public void Quit();

        public event NetworkService.ClientEvent OnPlayerJoined;

        public event NetworkService.ClientEvent OnPlayerLeft;

    }

    public class NetworkService: INetworkService {

        public Result<ulong> ClientId {
            get {
                if (NetworkManager.Singleton is not { } networkManager) {
                    return new Failure<ulong>("Not initialized.");
                }
                return new Success<ulong>(networkManager.LocalClientId);
            }
        }

        public bool IsHost => NetworkManager.Singleton && NetworkManager.Singleton.IsHost;
        
        public bool BindToRelayServer(RelayData data) {
            var transport = Object.FindObjectOfType<UnityTransport>();
            if (transport is null) {
                Debug.Log("Failed to find UnityTransport");
                return false;
            }
            transport.SetRelayServerData(
                data.IPv4Address,
                data.Port,
                data.AllocationIDBytes,
                data.Key,
                data.ConnectionData,
                data.HostConnectionData
            );
            bool result;
            if (data.HostConnectionData is not null) {
                Debug.Log("Starting host...");
                result = NetworkManager.Singleton.StartHost();
            } else {
                Debug.Log("Starting client...");
                result = NetworkManager.Singleton.StartClient();
            }
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            return result;
        }
        
        public Result<PlayerNetworkState> GetLocalNetworkState() {
            if (NetworkManager.Singleton is not { } networkManager) {
                return new Failure<PlayerNetworkState>("Network not initialized.");
            }
            if (networkManager.SpawnManager.GetLocalPlayerObject() is not { } player) {
                return new Failure<PlayerNetworkState>("Player object not found.");
            }
            if (player.TryGetComponent<PlayerNetworkState>(out var state)) {
                return new Success<PlayerNetworkState>(state);
            }
            return new Failure<PlayerNetworkState>(
                "Failed to get PlayerNetworkState component from GameObject."
            );
        }

        public delegate void ClientEvent(ulong uid);
        public event ClientEvent OnPlayerJoined;
        public event ClientEvent OnPlayerLeft;

        private void OnClientConnected(ulong uid) {
            Debug.Log($"[NetworkService] Client {uid} <color=green>connected</color>. Propagating event...");
            OnPlayerJoined?.Invoke(uid);
        }

        private void OnClientDisconnected(ulong uid) {
            Debug.Log($"[NetworkService] Client {uid} <color=red>disconnected</color>. Propagating event...");
            OnPlayerLeft?.Invoke(uid);
        }

        public IReadOnlyList<NetworkClient> Connections() {
            return NetworkManager.Singleton.ConnectedClientsList;
        }

        [ServerRpc]
        private void HostQuitGameServerRPC() {
            if (Services.Unity.Lobby is not {} lobby) return;
            if (NetworkManager.Singleton.ConnectedClients.Count > 0) {
                _ = Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions {
                    HostId = lobby.Players.RandomElement().Id
                }).Result;
            }
        }

        public void Quit() {
            if (!IsHost) {
                NetworkManager.Singleton.Shutdown();
                return;
            }
            HostQuitGameServerRPC();
        }

        public NetworkService() {
            Debug.Log("[NetworkService] Init.");
        }

        ~NetworkService() {
            Debug.Log("[NetworkService] Deinit.");
        }

    }
    
}