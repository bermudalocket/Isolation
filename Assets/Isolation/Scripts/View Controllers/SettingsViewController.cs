using UnityEngine;
using UnityEngine.UI;

namespace Isolation.Scripts.View_Controllers {

    public class SettingsViewController: MonoBehaviour {

        public Slider MouseSensitivitySlider;
        
        private void OnEnable() {
            MouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            MouseSensitivitySlider.value = IsolationPreferences.MouseSensitivity;
        }

        private void OnDisable() {
            MouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChanged);
        }

        private static void OnMouseSensitivityChanged(float value) {
            IsolationPreferences.MouseSensitivity = value;
        }

        public void OnCloseButtonClicked() {
            UIManager.Instance.Close(ScreenType.Settings);
        }

    }

}
