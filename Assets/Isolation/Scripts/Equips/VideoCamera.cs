using Isolation.Scripts.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace Isolation.Scripts.Equips {

    public partial class VideoCamera {
        private static int _totalSpawned;
    }

    public partial class VideoCamera: AbstractEquipment {

        private Camera _camera;

        private RenderTexture _output;
        private Material _material;
        private RawImage _display;

        public void Awake() {
            _camera = GetComponentInChildren<Camera>();
            _display = GetComponentInChildren<RawImage>();

            _output = new RenderTexture(
                width: 512, 
                height: 512, 
                depth: 16, 
                RenderTextureFormat.RGB111110Float
            );
            _camera.targetTexture = _output;

            // --------------------------------------------------------------------
            // display
            _display.texture = _output;

            // --------------------------------------------------------------------
            // Camboard
            var displayTag = _totalSpawned switch {
                0 => "Camera1",
                1 => "Camera2",
                2 => "Camera3",
                3 => "Camera4",
                _ => "Camera1",
            };
            var output = GameObject.FindGameObjectWithTag(displayTag);
            if (output && output.TryGetComponent<RawImage>(out var rawImage)) {
                rawImage.texture = _output;
                _totalSpawned += 1;
            }
        }

        private protected override void OnShow() {
            gameObject.SetActive(true);
        }

        private protected override void OnHide() {
            gameObject.SetActive(false);
        }

    }
}