using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Isolation.Scripts {

    public class PlayerController: NetworkBehaviour {

        public Camera Camera;
        public Animator Animator;
        public GameObject Spine;
        public Transform NeckAim;
        
        public float WalkSpeed = 3f;
        private float RunSpeed => WalkSpeed * 1.25f;
        private CharacterController _characterController;

        private float _horizontalAngle;
        private float _verticalAngle;
        
        private static readonly int HorizontalAnimationKey = Animator.StringToHash("h");
        private static readonly int VerticalAnimationKey = Animator.StringToHash("v");

        private void Awake() {
            DontDestroyOnLoad(transform.root.gameObject);
            _horizontalAngle = transform.localRotation.eulerAngles.y;
            _verticalAngle = Camera.transform.localRotation.eulerAngles.x;
            TryGetComponent(out _characterController);
            Animator = GetComponentInChildren<Animator>();
            UIManager.LockCursor();
        }

        private void Update() {
            if (!IsOwner) {
                return;
            }

            if (Keyboard.current.f1Key.wasPressedThisFrame) {
                ScreenCapture.CaptureScreenshot(Random.Range(1_000_000, 9_999_999) + ".png");
            }

            if (UIManager.Instance.AnyScreenOpen && !UIManager.Instance.IsOpen(ScreenType.Pause)) {
                return;
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame) {
                UIManager.Instance.Toggle(ScreenType.Pause);
                return;
            }

            if (UIManager.Instance.IsOpen(ScreenType.Pause)) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && SceneManager.GetActiveScene().name == "Map") {
                UIManager.Instance.Open(ScreenType.Evidence);
                return;
            }

            var move = new Vector3(0f, 0f, 0f);
            if (Keyboard.current.wKey.isPressed) {
                move += transform.forward * Time.deltaTime;
            }

            if (Keyboard.current.aKey.isPressed) {
                move += -transform.right * Time.deltaTime;
            }

            if (Keyboard.current.sKey.isPressed) {
                move += -transform.forward * Time.deltaTime;
            }

            if (Keyboard.current.dKey.isPressed) {
                move += transform.right * Time.deltaTime;
            }

            var speed = Keyboard.current.shiftKey.isPressed ? RunSpeed : WalkSpeed;
            if (_characterController) {
                _characterController.Move(speed * move);
            }
            Animator.SetFloat(HorizontalAnimationKey, move.x);
            Animator.SetFloat(VerticalAnimationKey, move.z);
        }
        
        private void LateUpdate() {
            if (UIManager.Instance.AnyScreenOpen) return;
            
            var delta = Mouse.current.delta.ReadValue();
            var accel = IsolationPreferences.MouseSensitivity;

            // horizontal
            var turn = delta.x;
            _horizontalAngle += turn;
            if (_horizontalAngle > 360) {
                _horizontalAngle -= 360;
            } else if (_horizontalAngle < 0) {
                _horizontalAngle += 360;
            }

            transform.localRotation = Quaternion.AngleAxis(_horizontalAngle, Vector3.up);

            // vertical
            var look = -delta.y;
            _verticalAngle = Mathf.Clamp(look + _verticalAngle, -75f, 75f);
            var rotation = Quaternion.AngleAxis(_verticalAngle, Vector3.right);
            
            Spine.transform.localRotation = rotation;
            NeckAim.transform.localRotation = rotation;
            Camera.transform.localRotation = rotation;
        }

    }
    
}