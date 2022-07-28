using UnityEngine;

namespace Isolation.Scripts.InGame {

    [RequireComponent(typeof(AudioSource))]
    public class RandomSound: MonoBehaviour {

        public AudioClip[] Clips;
        public float PitchMin = 0.9f;
        public float PitchMax = 1.2f;

        private AudioSource _source;

        private void Awake() {
            _source = GetComponent<AudioSource>();
        }

        private AudioClip GetRandomClip() {
            return Clips[Random.Range(0, Clips.Length)];
        }

        public void PlayRandom() {
            if (Clips.Length == 0)
                return;

            PlayClip(GetRandomClip(), PitchMin, PitchMax);
        }

        // animation event
        public void Step() {
            PlayRandom();
        }

        public void Footstep() => Step();

        public void PlayClip(AudioClip clip, float pitchMin, float pitchMax) {
            _source.volume = 0.25f;
            _source.pitch = Random.Range(pitchMin, pitchMax);
            _source.PlayOneShot(clip);
        }

    }

}