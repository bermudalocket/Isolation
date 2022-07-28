using UnityEngine;

namespace Isolation.Scripts {
    public class UIScreen: MonoBehaviour {

        public ScreenType ScreenType;
        
        private protected Canvas canvas;

        public bool IsVisible {
            get => canvas.enabled;
            set => canvas.enabled = value;
        }

        private void Awake() {
            canvas = GetComponent<Canvas>();
            UIManager.Instance.Reload();
        }

        public virtual void Open() {
            Debug.Log("[UIScreen] Opened screen " + ScreenType);
            if (ScreenType == ScreenType.Lobby) {
                return;
            }
            if (!canvas.enabled) {
                canvas.enabled = true;
            }
        }

        public virtual void Close() {
            Debug.Log("[UIScreen] Closed screen " + ScreenType);
            if (ScreenType == ScreenType.Lobby) {
                return;
            }
            if (canvas.enabled) {
                Debug.Log($"[UIScreen] Disabled canvas.");
                canvas.enabled = false;
            }
        }

    }
}