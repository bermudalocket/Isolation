using UnityEngine;

namespace Isolation.Scripts.InGame {
    public class NightVision: MonoBehaviour {

        private bool _isActive = false;

        public Shader shader;

        public Material material;

        public void Toggle() {
            _isActive = !_isActive;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            if (!_isActive) {
                Graphics.Blit(src, dest);
                return;
            }
            Graphics.Blit(src, dest, material);
        }

    }
}