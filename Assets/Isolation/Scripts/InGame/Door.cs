using Unity.Netcode;
using UnityEngine;

namespace Isolation.Scripts.InGame {

    public class Door: NetworkBehaviour {

        public AudioSource AudioSource;

        public Animator Animator;
        
        private static readonly int DoorOpenProperty = Animator.StringToHash("character_nearby");

        public bool IsOpen { get; private set; }

        public void ToggleDoorState() {
            IsOpen = !IsOpen;
            Animator.SetBool(DoorOpenProperty, IsOpen);
            AudioSource.Play();
            SetStateServerRPC(IsOpen);
        }

        [ClientRpc]
        private void SetStateClientRPC(bool isOpen) {
            Animator.SetBool(DoorOpenProperty, isOpen);
            AudioSource.Play();
        }

        [ServerRpc]
        private void SetStateServerRPC(bool isOpen) => SetStateClientRPC(isOpen);

    }
}