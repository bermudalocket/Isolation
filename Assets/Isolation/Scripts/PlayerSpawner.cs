using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts {

    public class PlayerSpawner: NetworkBehaviour {

        private void Start() {
            switch (Services.Network.ClientId) {
                case Success<ulong> uid:
                    if (IsServer) SpawnPlayerServerRPC(uid.Value);
                    break;
                
                default:
                    Addressables.InstantiateAsync("Isolation/Player", Vector3.zero, Quaternion.identity);
                    break;    
            }
        }
        
        [ServerRpc]
        private void SpawnPlayerServerRPC(ulong id) {
            var player = Addressables.InstantiateAsync("Isolation/Player", Vector3.zero, Quaternion.identity)
                        .WaitForCompletion();
            player.GetComponent<NetworkObject>()
                  .SpawnAsPlayerObject(id, destroyWithScene: true);
            foreach (var networkObject in player.GetComponentsInChildren<NetworkObject>()
                                    .Where(no => !no.IsSpawned)) {
                networkObject.Spawn(true);
            }
        }

    }

}