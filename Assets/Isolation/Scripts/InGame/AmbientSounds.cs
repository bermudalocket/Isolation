using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Isolation.Scripts.InGame {
    
    [RequireComponent(typeof(AudioSource))]
    public class AmbientSounds: NetworkBehaviour {

        [FormerlySerializedAs("minDelaySeconds")] [Range(1f, 10f)]
        public float MinDelaySeconds = 8f;

        [FormerlySerializedAs("maxDelaySeconds")] [Range(10f, 30f)]
        public float MaxDelaySeconds = 25f;

        private AudioSource _audioSource;
        private static readonly List<AudioClip> AudioClips = new();
        private static bool _audioClipsAsyncLoadHandle;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            if (_audioClipsAsyncLoadHandle) return;
            Addressables.LoadAssetsAsync<AudioClip>("Ambient Sounds", AudioClips.Add);
            _audioClipsAsyncLoadHandle = true;
        }

        private void Start() {
            if (Services.Network.IsHost) {
                StartCoroutine(SoundLoop());
            }
        }

        private IEnumerator SoundLoop() {
            while (isActiveAndEnabled) {
                if (GameState.SafeTimeRemaining > 0 || Random.value < 0.50) {
                    // do nothing
                } else if (AudioClips.Count > 0) {
                    var sound = AudioClips.RandomElement();
                    _audioSource.PlayOneShot(sound);
                }
                var wait = RandomGaussian.Range(MinDelaySeconds, MaxDelaySeconds);
                Debug.Log($"[AmbientSounds] Waiting {wait} seconds");
                yield return new WaitForSeconds(wait);
            }
        }

    }
}