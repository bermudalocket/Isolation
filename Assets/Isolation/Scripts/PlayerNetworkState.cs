#nullable enable
using System;
using Unity.Netcode;
using UnityEngine;

namespace Isolation.Scripts {

    public struct NetworkedPlayerPrefInt {

        public readonly NetworkVariable<int> NetworkWrapper;

        public NetworkedPlayerPrefInt(string key) {
            NetworkWrapper = new NetworkVariable<int>();
            NetworkWrapper.Value = PlayerPrefs.GetInt(key, 1);
            NetworkWrapper.OnValueChanged += (_, value) => PlayerPrefs.SetInt(key, value);
        }

    }

    public class PlayerNetworkState: NetworkBehaviour {

        public NetworkedString DisplayName = new();

        public NetworkVariable<bool> IsReady = new(false);

        public NetworkVariable<bool> IsSafe = new(true);
        
        public NetworkVariable<bool> IsAlive = new(true);

        public NetworkVariable<int> OxygenLevel = new(100);

        public NetworkVariable<NetworkAntagonistType> Guess = new();

        public NetworkedPlayerPrefInt PhotoCameraLevel;
        public NetworkedPlayerPrefInt VideoCameraLevel;
        public NetworkedPlayerPrefInt ThermometerLevel;
        public NetworkedPlayerPrefInt FlashlightLevel;
        public NetworkedPlayerPrefInt UVLightLevel;

        public NetworkVariable<int> GetLevel(EquipmentType type) {
            var container = type switch {
                EquipmentType.Flashlight => FlashlightLevel,
                EquipmentType.Thermometer => ThermometerLevel,
                EquipmentType.PhotoCamera => PhotoCameraLevel,
                EquipmentType.VideoCamera => VideoCameraLevel,
                EquipmentType.UVLight => UVLightLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            return container.NetworkWrapper;
        }

        public void SetEquipmentLevel(EquipmentType type, int level) {
            GetLevel(type).Value = level;
        }

        ~PlayerNetworkState() {
            Debug.Log($"[PlayerNetworkState] <color=red>Deinit</color>.");
        }
        
        private void OnEnable() {
            if (!IsOwner) {
                return;
            }
            PhotoCameraLevel = new NetworkedPlayerPrefInt("PhotoCamera");
            VideoCameraLevel = new NetworkedPlayerPrefInt("VideoCamera");
            ThermometerLevel = new NetworkedPlayerPrefInt("Thermometer");
            FlashlightLevel = new NetworkedPlayerPrefInt("Flashlight");
            UVLightLevel = new NetworkedPlayerPrefInt("UVLight");
            DisplayName.Value = Services.Auth.Username ?? "unset";
        }

        private void OnDisable() {
            Debug.Log($"[PlayerNetworkState] <color=gray>OnDisable</color>");
            if (!IsOwner) {
                return;
            }
            IsReady.OnValueChanged = null;
        }
        
    }

}