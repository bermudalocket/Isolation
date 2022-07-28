using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NSubstitute;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Isolation.Scripts {

    public class TestNetwork: MonoBehaviour {

        [SerializeField] 
        private bool _mockAuth;
        
        [SerializeField]
        private bool _mockNetwork;

        [SerializeField] private bool _mockLobby;

        [SerializeField]
        private string _clientId;

        private ulong ClientId => ulong.TryParse(_clientId, out var id) ? id : 0;

        private void Awake() {
            if (!_mockAuth && !_mockNetwork) {
                Debug.Log("No mocks chosen.");
                return;
            }
            Debug.Log("-----------------------------------------------------");
            Debug.Log("<b><color=purple>Setting up mock environment...</color></b>");
            Debug.Log("-----------------------------------------------------");
            if (_mockAuth) {
                Debug.Log("Mocking: <b><color=purple>Auth</color></b>");
                Services.Auth = new AnonAuthService();
                Debug.Log("Mocked!");
            }
            if (_mockLobby) {
                var lobby = Substitute.For<IUnityService>();
                lobby.HostRelayServer()
                     .Returns(new UniTask<bool>(true));
                lobby.Lobby
                     .Returns(new Lobby(
                         id: "lobby_id",
                         lobbyCode: "lobbycode",
                         maxPlayers: 4,
                         name: "lobby_name",
                         isPrivate: true
                     ));
                Services.Unity = lobby;
            }
            if (_mockNetwork) {
                Debug.Log("Mocking: <b><color=green>Network</color></b>");
                var service = Substitute.For<INetworkService>();
                service.IsHost
                       .Returns(true);
                service.BindToRelayServer(Arg.Any<RelayData>())
                       .Returns(true);
                service.ClientId
                       .Returns(new Success<ulong>(ClientId));
                service.GetLocalNetworkState()
                       .Returns(_ => {
                           if (FindObjectOfType<PlayerNetworkState>() is { } state) {
                               return new Success<PlayerNetworkState>(state);
                           }
                           return new Failure<PlayerNetworkState>("No PlayerNetworkState found.");
                       });
                service.Connections()
                       .Returns(new[] {
                           new NetworkClient { ClientId = this.ClientId }
                       });

                Services.Network = service;
                
                Debug.Log("Mocked!");
            }
        }

    }

}