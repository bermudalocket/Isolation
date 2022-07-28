using Isolation.Scripts.Equips;
using UnityEngine;

namespace Isolation.Scripts {
    
    public class GrabbableProp: Grabbable {

        private MeshRenderer _renderer;

        private void Awake() {
            TryGetComponent(out _renderer);
        }

        private protected override void OnShow() {
            _renderer.enabled = true;
        }

        private protected override void OnHide() {
            _renderer.enabled = false;
        }

    }
    
}