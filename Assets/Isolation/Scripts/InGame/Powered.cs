using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Isolation.Scripts.InGame {
    public class Powered: MonoBehaviour {

        public new Light light;

        private CancellationTokenSource _job;

        private void OnEnable() {
            IsolationEvents.HuntStateChangedEvent += OnHuntStateChanged;
            GameState.IsPowerOn.OnValueChanged += Toggle;
        }

        private void OnDisable() {
            IsolationEvents.HuntStateChangedEvent -= OnHuntStateChanged;
            GameState.IsPowerOn.OnValueChanged -= Toggle;
        }

        private void OnHuntStateChanged(bool isHunting) {
            if (isHunting) {
                _job = new CancellationTokenSource();
                UniTask.Void(Flicker, _job.Token);
            } else {
                _job?.Cancel();
            }
        }

        public void Toggle(bool isPowerOn, bool newValue) {
            light.enabled = isPowerOn;
        }

        private async UniTaskVoid Flicker(CancellationToken cancellationToken) {
            while (isActiveAndEnabled) {
                if (cancellationToken.IsCancellationRequested) {
                    Debug.Log($"[Powered] - Flicker " + GetInstanceID());
                    break;
                }
                light.enabled = true;
                await UniTask.Delay(Random.Range(50, 500), cancellationToken: cancellationToken);
                light.enabled = false;
                await UniTask.Delay(Random.Range(250, 1_250), cancellationToken: cancellationToken);
            }
        }

    }
}