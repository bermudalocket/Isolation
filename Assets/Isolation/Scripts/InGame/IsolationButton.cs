using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Isolation.Scripts.InGame {
    
    [AddComponentMenu("Isolation/UI/Button")]
    public class IsolationButton: Button {

        private static AudioSource _audioSource;
        private static AudioSource AudioSource {
            get {
                if (!_audioSource) {
                    _audioSource = new GameObject("GlobalAudio").AddComponent<AudioSource>();
                    _audioSource.volume = 0.4f;
                }
                return _audioSource;
            }
        }
        
        // --------------------------------------------------------------------

        protected override void Awake() {
            base.Awake();
            UniTask.Void(Setup);
        }

        private async UniTaskVoid Setup() {
            _hoverSound = await Addressables.LoadAssetAsync<AudioClip>("Sound/UI/ButtonHover");
            _clickSound = await Addressables.LoadAssetAsync<AudioClip>("Sound/UI/ButtonClick");
        }

        // --------------------------------------------------------------------
        
        private AudioClip _hoverSound;
        private AudioClip _clickSound;
        
        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            if (interactable) {
                AudioSource.PlayOneShot(_hoverSound);
            }
        }
        
        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            if (interactable) {
                AudioSource.PlayOneShot(_clickSound);
            }
        }
        
    }
    
}