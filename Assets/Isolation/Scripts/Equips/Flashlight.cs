using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Isolation.Scripts.InGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Isolation.Scripts.Equips {

    public class Flashlight: AbstractEquipment {

        private readonly NetworkVariable<bool> _isOn = new();
        
        public Light Light;

        public MeshRenderer Renderer;

        private void Start() {
            UniTask.Void(UpdateLoop, this.GetCancellationTokenOnDestroy());
            _isOn.OnValueChanged += SetState;
        }

        private new void OnDestroy() {
            _isOn.OnValueChanged -= SetState;
        }

        private void SetState(bool oldState, bool state) {
            Light.enabled = state;
            PlayUseSound();
        }
        
        private protected override void OnShow() {
            Renderer.enabled = true;
        }

        private protected override void OnHide() {
            Renderer.enabled = false;
        }

        private async UniTaskVoid UpdateLoop(CancellationToken cancellationToken) {
            Debug.Log("+ UpdateLoop for Flashlight " + GetInstanceID());
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate()) {
                if (cancellationToken.IsCancellationRequested) {
                    Debug.Log("- UpdateLoop for Flashlight " + GetInstanceID());
                    break;
                }

                var isRendered = Renderer.enabled;
                var shouldToggle1 = IsActive && isRendered && Mouse.current.leftButton.wasPressedThisFrame;
                var shouldToggle2 = IsHeld && !isRendered && Keyboard.current.tKey.wasPressedThisFrame;
                if (shouldToggle1 || shouldToggle2) {
                    _isOn.Value = !_isOn.Value;
                }
            }
        }


    }
}