using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Isolation.Scripts.Equips {

    public abstract class Grabbable: NetworkBehaviour {

        /// <summary>
        /// The sound to play when this item is "used".
        /// </summary>
        public AssetReferenceT<AudioClip> UseSound;

        internal void PlayUseSound() {
            AudioSource.PlayClipAtPoint(UseSound.editorAsset, transform.position);
        }

        /// <summary>
        /// The player currently holding this grabbable.
        /// </summary>
        private InventoryHolder Holder { get; set; }

        public GameObject GhostProjection;

        /// True if this object is in someone's inventory.
        protected bool IsHeld => Holder != null;

        /// <summary>
        /// True if this object is being actively held.
        /// </summary>
        protected bool IsActive => IsHeld && gameObject.activeSelf;

        public Rigidbody Rigidbody;

        public bool Grab(InventoryHolder inventoryHolder) {
            if (IsHeld) {
                return false;
            }
            Holder = inventoryHolder;
            SetLayer(LayerMask.NameToLayer("FirstPersonWeapon"));
            Rigidbody.isKinematic = true;
            ReparentServerRPC(inventoryHolder.HeldItemSlot.GetComponent<NetworkObject>().NetworkObjectId);
            return true;
        }

        [ServerRpc]
        private void ReparentServerRPC(ulong destination) {
            var tdest = NetworkManager.Singleton.SpawnManager.SpawnedObjects[destination];
            transform.SetParent(tdest.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void Show() {
            OnShow();
            SetVisibleServerRPC(true);
        }
        private protected virtual void OnShow() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            OnHide();
            SetVisibleServerRPC(false);
        }
        private protected virtual void OnHide() {
            gameObject.SetActive(false);
        }

        [ServerRpc]
        private void SetVisibleServerRPC(bool state) {
            SetVisibleClientRPC(state);
        }

        [ClientRpc]
        private void SetVisibleClientRPC(bool state) {
            if (state) OnShow(); else OnHide();
        }

        public void Place(Vector3 worldPosition) {
            SetLayer(LayerMask.NameToLayer("Grabbables"));
            var t = transform;
            t.SetParent(null);
            t.position = worldPosition;
            transform.localRotation = Quaternion.Euler(0f, t.localRotation.y, 0f);
            Holder = null;
        }

        public void Throw() {
            SetLayer(LayerMask.NameToLayer("Grabbables"));
            var t = transform;
            t.SetParent(null);

            Rigidbody.isKinematic = false;
            Rigidbody.AddForce(t.forward * 8f, ForceMode.Impulse);
            Rigidbody.AddForce(t.up * 0.75f, ForceMode.Impulse);
            Rigidbody.AddTorque(
                5 * new Vector3(
                    Random.value, Random.value, Random.value
                )
            );

            Holder = null;
        }

        private void SetLayer(int layer) {
            gameObject.layer = layer;
            for (var i = 0; i < transform.childCount; i++) {
                if (transform.GetChild(i) is { gameObject: { } go }) {
                    go.layer = layer;
                }
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            var t = transform;
            Gizmos.DrawRay(t.position, t.forward);
        }
        #endif

    }

}