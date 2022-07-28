using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Isolation.Scripts {
    
    public class HUDManager: MonoBehaviour {

        public TextMeshProUGUI TooltipTitle;
        public TextMeshProUGUI TooltipSubtitle;
        public Camera Camera;
        public Image Crosshairs;
        public Image CircularReticle;
        public Image GrabbableReticle;

        public enum ReticleType { Crosshairs, Circular, Grabbable }

        private readonly RaycastHit[] _raycasts = new RaycastHit[10];

        private void Update() {
            if (!Camera) {
                return;
            }
            var eyes = Camera!.transform;
            var ray = new Ray {
                origin = eyes.position,
                direction = eyes.forward
            };
            var didChangeReticle = false;
            var hitCount = Physics.RaycastNonAlloc(ray, _raycasts, 20f);
            for (var i = 0; i < hitCount; i++) {
                var hit = _raycasts[i];
                if (!hit.collider || !hit.collider.TryGetComponent(out CursorModifier cursorModifier)) {
                    continue;
                }
                var distanceSqr = (hit.point - eyes.position).sqrMagnitude;
                if (distanceSqr > Math.Pow(cursorModifier.MaxDistance, 2)) {
                    continue;
                }
                cursorModifier.SetOutlineVisible(true);
                SetReticle(cursorModifier.Type);
                didChangeReticle = true;
                if (cursorModifier.Title is { } title && TooltipTitle is not null) {
                    TooltipTitle.text = title;
                }
                if (cursorModifier.Subtitle is { } subtitle && TooltipSubtitle is not null) {
                    TooltipSubtitle.text = subtitle;
                }
                if (Mouse.current.leftButton.wasPressedThisFrame && !UIManager.Instance.AnyScreenOpen) {
                    if (cursorModifier.Action is { } action) {
                        action.Invoke();
                        Debug.Log($"[HUDManager] Click detected on <b>{hit.collider.gameObject.name}</b> to invoke <b>{action.GetPersistentMethodName(0)}</b>");
                    
                    }
                }
            }
            if (!didChangeReticle && !Crosshairs.enabled) {
                SetReticle(ReticleType.Crosshairs);
                TooltipTitle.text = "";
                TooltipSubtitle.text = "";
            }
        }

        private void SetReticle(ReticleType type) {
            switch (type) {
                case ReticleType.Crosshairs:
                    Crosshairs.enabled = true;
                    GrabbableReticle.enabled = false;
                    CircularReticle.enabled = false;
                    break;

                case ReticleType.Circular:
                    CircularReticle.enabled = true;
                    Crosshairs.enabled = false;
                    GrabbableReticle.enabled = false;
                    break;

                case ReticleType.Grabbable:
                    GrabbableReticle.enabled = true;
                    Crosshairs.enabled = false;
                    CircularReticle.enabled = false;
                    break;

                default:
                    Debug.Log("Unknown reticle type: " + type);
                    break;
            }
        }

    }
}