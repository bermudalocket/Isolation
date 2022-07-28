using UnityEngine;

namespace Isolation.Scripts {
    
    public static class IsolationPreferences {

        static IsolationPreferences() {
            _mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 10f);
        }

        private static float _mouseSensitivity;
        
        public static float MouseSensitivity {
            get => Mathf.Clamp(_mouseSensitivity, 10f, 100f);
            set {
                _mouseSensitivity = value;
                PlayerPrefs.SetFloat("MouseSensitivity", value);
            }
        }

    }

}
