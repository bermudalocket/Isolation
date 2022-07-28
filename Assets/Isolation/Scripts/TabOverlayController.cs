using System.Linq;
using Isolation.Scripts.Equips;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Isolation.Scripts {
    public class TabOverlayController: MonoBehaviour {

        public Canvas tabOverlayCanvas;

        public RawImage[] photoDestinations;

        private void Update() {
            var tabKeyPressed = Keyboard.current.tabKey.isPressed;
            if (tabKeyPressed && !tabOverlayCanvas.enabled) {
                UpdatePhotos();
                tabOverlayCanvas.enabled = true;
            }
            if (!tabKeyPressed && tabOverlayCanvas.enabled) {
                tabOverlayCanvas.enabled = false;
            }
        }

        private void UpdatePhotos() {
            var allPhotoCameras = FindObjectsOfType<PhotoCamera>();
            var allPhotos = allPhotoCameras.SelectMany(camera => camera.Photos);
            var textures = allPhotos as RenderTexture[] ?? allPhotos.ToArray();
            for (var i = 0; i < textures.Length && i < photoDestinations.Length; i++) {
                photoDestinations[i].texture = textures[i];
                photoDestinations[i].color = Color.white;
            }
        }

    }
}