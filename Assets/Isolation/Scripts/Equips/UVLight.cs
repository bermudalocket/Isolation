using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Isolation.Scripts.InGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Isolation.Scripts.Equips {

    public class UVLight: AbstractEquipment {

        public Light Light;

        public Transform RevealEffector;

        private void Awake() {
            UniTask.Void(UpdateLoop, this.GetCancellationTokenOnDestroy());
        }

        private void OnDrawGizmos() {
            var t = transform;
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(t.position, Light.range * t.forward);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(RevealEffector.position, 0.1f);
        }

        private protected override void OnShow() {
            gameObject.SetActive(true);
        }

        private protected override void OnHide() {
            gameObject.SetActive(false);
        }

        private async UniTaskVoid UpdateLoop(CancellationToken cancellationToken) {
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate()) {
                if (cancellationToken.IsCancellationRequested) break;

                var shouldToggle1 = IsActive && Mouse.current.leftButton.wasPressedThisFrame;
                var shouldToggle2 = IsHeld && !IsActive && Keyboard.current.tKey.wasPressedThisFrame;
                if (shouldToggle1 || shouldToggle2) {
                    Toggle();
                }

                if (IsActive && RevealEffector.gameObject.activeSelf) {
                    var t = transform;
                    var ray = new Ray {
                        origin = t.position,
                        direction = 10f * t.forward
                    };
                    if (Physics.Raycast(ray, out var hit, Light.range)) {
                        Debug.Log(hit.transform.name);
                        RevealEffector.position = hit.point;
                    }
                }
            }
        }

        private void Toggle() {
            var newState = !Light.enabled;
            Light.enabled = newState;
            RevealEffector.gameObject.SetActive(newState);
            AudioSource.PlayClipAtPoint(UseSound.editorAsset, transform.position);
            SetStateServerRPC(newState);
        }

        [ServerRpc]
        private void SetStateServerRPC(bool isOn) {
            SetStateClientRPC(isOn);
        }
        
        [ClientRpc]
        private void SetStateClientRPC(bool isOn) {
            Light.enabled = isOn;
            AudioSource.PlayClipAtPoint(UseSound.editorAsset, transform.position);
        }

    }
}