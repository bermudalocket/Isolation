using UnityEngine;

namespace Isolation.Scripts.InGame {
    
    public sealed class PowerBox: MonoBehaviour {

        public AudioClip PowerUpSound;
        
        public AudioClip PowerDownSound;

        private void OnEnable() {
            GameState.IsPowerOn.OnValueChanged += OnPowerStateChanged;
        }
        
        private void OnDisable() {
            GameState.IsPowerOn.OnValueChanged -= OnPowerStateChanged;
        }

        private void OnPowerStateChanged(bool previousvalue, bool newvalue) {
            AudioSource.PlayClipAtPoint(
                GameState.IsPowerOn.Value ? PowerUpSound : PowerDownSound,
                transform.position
            );
        }

        public void TogglePower() {
            GameState.IsPowerOn.Value = !GameState.IsPowerOn.Value;
        }

    }
    
}