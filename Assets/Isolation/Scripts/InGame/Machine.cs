using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts.InGame {
    
    public class Machine: MonoBehaviour {

        private AudioSource _audioSource;

        private async UniTaskVoid Start() {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.volume = 0.25f;
            _audioSource.clip = await Addressables.LoadAssetAsync<AudioClip>("HumSound");
        }

        private void OnEnable() {
            GameState.IsPowerOn.OnValueChanged += OnPowerChanged;
        }
        
        private void OnDisable() {
            GameState.IsPowerOn.OnValueChanged -= OnPowerChanged;
        }

        private void OnPowerChanged(bool state, bool newValue) {
            if (state) {
                _audioSource.Play();
            } else {
                _audioSource.Stop();
            }
        }

    }
    
}