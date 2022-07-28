using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts.InGame {
    
    public class DoorTablet: MonoBehaviour {

        private Door _door;

        private Material _litMaterial;
        private Material _unlitMaterial;

        private MeshRenderer _meshRenderer;

        private async void Awake() {
            _door = GetComponentInParent<Door>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _litMaterial = await Addressables.LoadAssetAsync<Material>("TabletLit");
            _unlitMaterial = await Addressables.LoadAssetAsync<Material>("TabletUnlit");
        }

        [IsolationEventHandler(IsolationEvent.Power)]
        private void OnPowerStateChanged(bool state) {
            _meshRenderer.sharedMaterial = state ? _litMaterial : _unlitMaterial;
        }

        public void OnMouseDown() {
            if (GameState.IsPowerOn.Value) {
                _door.ToggleDoorState();
            }
        }

    }
    
}