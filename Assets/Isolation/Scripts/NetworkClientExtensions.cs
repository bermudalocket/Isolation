using Unity.Netcode;

namespace Isolation.Scripts {

    public static class NetworkClientExtensions {

        public static PlayerNetworkState? GetNetworkState(this NetworkClient client) {
            if (client.PlayerObject is not { } player) {
                return null;
            }
            return player.GetComponent<PlayerNetworkState>();
        }

    }

}