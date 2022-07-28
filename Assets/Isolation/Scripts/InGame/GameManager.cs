using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Isolation.Scripts.InGame {

    public partial class GameManager: Singleton<GameManager> {

        private void Awake() {
            Debug.Log("[GameManager] Awake.");
            GameState.Stage.Value = GameStage.InGame;

            if (IsServer) {
                GetComponent<NetworkObject>().Spawn();
            }
            if (Services.Network.IsHost) {
                BuildLevel();
            }
        }

        ~GameManager() {
            Debug.Log("[GameManager] Deinit.");
        }

        private static async UniTaskVoid DoSafeTimeCountdown(CancellationToken cancellationToken) {
            while (GameState.SafeTimeRemaining.Value > 0) {
                if (cancellationToken.IsCancellationRequested) {
                    Debug.Log("[GameManager] Stopping <b>DoSafeTimeCountdown</b> task.");
                    break;
                }
                GameState.SafeTimeRemaining.Value -= 1;
                await UniTask.Delay(1_000, cancellationToken: cancellationToken);
            }
        }

    }
}