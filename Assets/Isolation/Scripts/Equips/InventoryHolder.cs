using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Isolation.Scripts.Equips {

    public sealed class InventoryHolder: MonoBehaviour {
        
        public Camera Camera;
        
        public Transform HeldItemSlot;
        
        public TextMeshProUGUI[] HUDInventoryLabels;
        
        // --------------------------------------------------------------------

        private GameObject _placementIndicator;

        private int _activeItemSlot;

        private readonly Grabbable[] _inventory = new Grabbable[3];

        public int Count => _inventory.Count(o => o != null);

        [CanBeNull]
        private Grabbable HeldItem => _inventory[_activeItemSlot];

        private int Add(Grabbable grabbable) {
            for (var i = 0; i < _inventory.Length; i++) {
                if (!_inventory[i]) {
                    _inventory[i] = grabbable;
                    return i;
                }
            }
            return -1;
        }

        private void Remove(Grabbable grabbable) {
            for (var i = 0; i < _inventory.Length; i++) {
                if (_inventory[i] == grabbable) {
                    _inventory[i] = null;
                    break;
                }
            }
        }

        private void ChangeSlot(int newSlot) {
            if (HeldItem is { } heldItem) {
                heldItem.Hide();
            }
            _activeItemSlot = newSlot;
            if (_inventory[newSlot] is { } newItem) {
                newItem.Show();
            }
        }

        private void TryPickUpItem() {
            if (Count >= 3) {
                return;
            }
            var ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit)) {
                return;
            }
            if (!hit.collider.gameObject.TryGetComponent<Grabbable>(out var grabbable)) {
                return;
            }
            if (!grabbable.Grab(this)) {
                return;
            }
            var slot = Add(grabbable);
            if (slot != _activeItemSlot) {
                grabbable.Hide();
            }
        }

        private void TryPlaceHeldItem(Vector3 position) {
            if (HeldItem is { } heldItem) {
                heldItem.Place(position);
            }
            _inventory[_activeItemSlot] = null;
        }


        private void Update() {
            // update HUD
            for (var i = 0; i < _inventory.Length; i++) {
                var text = (i + 1) + " ";
                if (_inventory[i] is { } grabbable) {
                    text += grabbable.name.Replace("(Clone)", "");
                }
                HUDInventoryLabels[i].text = text;
            }

            var point = Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f));
            if (HeldItem is { } heldItem) {
                var t = heldItem.transform;
                t.LookAt(point);

                // t.localPosition = item.offset;
                // t.localRotation = Quaternion.Euler(item.offsetRotation);

                if (Keyboard.current.leftAltKey.isPressed) {
                    _placementIndicator = heldItem.GhostProjection;
                    if (!_placementIndicator.activeSelf) {
                        _placementIndicator.SetActive(true);
                    }
                    var cameraPoint = Camera.transform.position;
                    var dir = (point - cameraPoint).normalized;
                    if (Physics.Raycast(cameraPoint, dir, out var hit, 10f,
                        LayerMask.NameToLayer("PlaceableSurface"))) {
                        _placementIndicator.transform.position = hit.point;
                        if (Mouse.current.leftButton.wasPressedThisFrame) {
                            TryPlaceHeldItem(hit.point);
                        }
                    }
                } else {
                    if (_placementIndicator && _placementIndicator.activeSelf) {
                        _placementIndicator.SetActive(false);
                    }
                }

                if (Keyboard.current.gKey.wasPressedThisFrame) {
                    Remove(heldItem);
                    heldItem.transform.localRotation = Quaternion.identity;
                    heldItem.Throw();
                    return;
                }
                //
                // // sway
                // var delta = Mouse.current.delta.ReadValue();
                // var sway = 0.1f * new Vector3(-delta.x, -delta.y, 0);
                // heldItem.transform.localPosition = Vector3.Lerp(transform.localPosition, sway, Time.deltaTime * 4f);
            } else {
                if (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed) {
                    var cameraPoint = Camera.transform.position;
                    var dir = (point - cameraPoint).normalized;
                    if (Physics.Raycast(cameraPoint, dir, out var hit, 4f, LayerMask.GetMask("Grabbables"))) {
                        Debug.DrawRay(cameraPoint, 4f * dir, Color.green, 10f);
                        var rotationDirection = Keyboard.current.leftCtrlKey.isPressed ? 1 : -1;
                        hit.transform.localRotation *= Quaternion.Euler(0f, rotationDirection * 30f * Time.deltaTime, 0f);
                    }
                }
                if (_placementIndicator && _placementIndicator.activeSelf) {
                    _placementIndicator.SetActive(false);
                }
            }

            if (Keyboard.current.eKey.wasPressedThisFrame) {
                TryPickUpItem();
                return;
            }

            if (Keyboard.current.digit1Key.wasPressedThisFrame) {
                ChangeSlot(0);
            } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
                ChangeSlot(1);
            } else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
                ChangeSlot(2);
            }
        }

    }

}