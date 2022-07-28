using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Isolation.Scripts {

    public class UIManager: MonoBehaviour {

        private static UIManager _instance;
        public static UIManager Instance {
            get {
                if (!_instance) {
                    var go = new GameObject("UIManager.Instance");
                    _instance = go.AddComponent<UIManager>();
                }
                return _instance;
            }
        }

        private Dictionary<ScreenType, UIScreen> _screens;

        private readonly Dictionary<ScreenType, bool> _screenStatus = new();

        private readonly LinkedList<ScreenType> _viewStack = new();

        public bool AnyScreenOpen => _screenStatus.Values.Any(v => v);
        public bool IsOpen(ScreenType type) => _screenStatus.ContainsKey(type) && _screenStatus[type];

        private void Awake() {
            if (_instance && _instance != this) {
                Debug.Log($"[UIManager] Only one instance can exist. Destroying {GetInstanceID()}.");
                Destroy(this);
            } else {
                if (!_instance) {
                    _instance = this;
                    Debug.Log($"[UIManager] Primary instance assigned.");
                }
                DontDestroyOnLoad(this);
                SceneManager.activeSceneChanged += delegate {
                    Debug.Log($"[UIManager] Scene change detected. Looking for new screens...");
                    Reload();
                };
            }
        }

        private void Start() => Reload();

        public void Reload() {
            _screens = new Dictionary<ScreenType, UIScreen>();
            foreach (var uiScreen in FindObjectsOfType<UIScreen>()) {
                _screens[uiScreen.ScreenType] = uiScreen;
                _screenStatus[uiScreen.ScreenType] = false;
            }

            var list = _screens.Keys.Select(s => $"{s}").Aggregate((a, b) => a + b);
            Debug.Log($"[UIManager] Found <b>{_screens.Count}</b> screens: {list}");
        }

        public void Open(ScreenType type) {
            if (!_screens.TryGetValue(type, out var screen)) {
                Debug.Log($"[UIManager] The screen <b>{type}</b> was requested but was not found.");
                return;
            }
            if (_viewStack.Last is { } node && _screens[node.Value] is { } parentScreen) {
                parentScreen.IsVisible = false;
            }
            Debug.Log($"[UIManager] Opening screen <b>{type}</b>.");
            screen.Open();
            _viewStack.AddLast(type);
            _screenStatus[type] = true;
            FreeCursor();
        }

        public void Close(ScreenType type) {
            if (!_screens.TryGetValue(type, out var screen)) {
                Debug.Log($"[UIManager] The screen <b>{type}</b> was requested but was not found.");
                return;
            }
            Debug.Log($"[UIManager] Closing screen <b>{type}</b>.");
            screen.Close();
            _screenStatus[type] = false;
            _viewStack.Remove(type);
            LockCursor();

            if (_viewStack.Count > 0) {

                // check if a there's a screen still on the view stack
                var prev = _viewStack.Last;
                var prevType = prev.Value;
                if (_screens[prevType] is not { IsVisible: false } parentScreen) return;

                parentScreen.IsVisible = true;
                FreeCursor();
            }

        }

        public void Toggle(ScreenType type) {
            if (_screenStatus.TryGetValue(type, out var state)) {
                if (state) {
                    Close(type);
                } else {
                    Open(type);
                }
            } else {
                Open(type);
            }
        }

        public static void FreeCursor() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void LockCursor() {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
    }
}