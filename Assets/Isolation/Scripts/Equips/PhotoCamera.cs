using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Isolation.Scripts.InGame;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Isolation.Scripts.Equips {

    public class PhotoCamera: AbstractEquipment {
        
        public readonly List<RenderTexture> Photos = new();

        private bool _isTakingPicture;

        public Camera Camera;
        
        public Light Light;

        public RawImage CameraDisplay;
        
        private RenderTexture _cameraDisplayTexture;

        public void Awake() {
            _cameraDisplayTexture = new RenderTexture(512, 512, 100);
            Camera!.targetTexture = _cameraDisplayTexture;
            CameraDisplay!.texture = _cameraDisplayTexture;
        }

        private void Start() {
            UniTask.Void(UpdateLoop, this.GetCancellationTokenOnDestroy());
        }

        private async UniTaskVoid UpdateLoop(CancellationToken token) {
            await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate()) {
                if (token.IsCancellationRequested) {
                    break;
                }
                if (_isTakingPicture) continue;
                if (IsActive && Mouse.current.rightButton.wasPressedThisFrame) {
                    await TakePicture();
                }
            }
        }

        public async UniTask<RenderTexture> TakePicture() {
            _isTakingPicture = true;
            Light.enabled = true;
            AudioSource.PlayClipAtPoint(UseSound.editorAsset, transform.position);

            await UniTask.Delay(500);

            var texture = new RenderTexture(1280, 800, 24);
            Camera.targetTexture = texture;
            Camera.Render();
            RenderTexture.active = texture;
            var photo = new Texture2D(1280, 800, TextureFormat.RGB24, false);
            photo.ReadPixels(new Rect(0, 0, 1280, 800), 0, 0);
            Camera.targetTexture = _cameraDisplayTexture;
            RenderTexture.active = null;
            CameraDisplay.texture = texture;
            Photos.Add(texture);

            await UniTask.Delay(500);

            Light.enabled = false;

            await UniTask.Delay(1500);

            CameraDisplay.texture = _cameraDisplayTexture;
            _isTakingPicture = false;

            return texture;
        }


        private protected override void OnShow() {
            gameObject.SetActive(true);
        }

        private protected override void OnHide() {
            gameObject.SetActive(false);
        }

    }
}