using UnityEngine;

namespace Isolation.Scripts {

    public static class PlayerState {

        // --------------------------------------------------------------------
        public static int Experience {
            get => PlayerPrefs.GetInt("exp", 0);
            set => PlayerPrefs.SetInt("exp", value);
        }
        public delegate void PlayerExperienceChanged(int change, int total);
        public static event PlayerExperienceChanged PlayerExperienceChangedEvent;

        // --------------------------------------------------------------------
        public static int Cash {
            get => PlayerPrefs.GetInt("cash", 0);
            set {
                var current = PlayerPrefs.GetInt("cash", 0);
                var change = value - current;
                PlayerPrefs.SetInt("cash", value);
                PlayerCashChangedEvent?.Invoke(change, value);
            }
        }
        public delegate void PlayerCashChanged(int change, int newCash);
        public static event PlayerCashChanged PlayerCashChangedEvent;

        // --------------------------------------------------------------------
        public static int UpgradeChips {
            get => PlayerPrefs.GetInt("upgrade-chips", 0);
            set {
                PlayerPrefs.SetInt("upgrade-chips", value);
                PlayerUpgradeChipsChangedEvent?.Invoke();
            }
        }
        public delegate void PlayerUpgradeChipsChanged();
        public static event PlayerUpgradeChipsChanged PlayerUpgradeChipsChangedEvent;

        // --------------------------------------------------------------------
        public delegate void PlayerEquipmentChanged();
        public static event PlayerEquipmentChanged PlayerEquipmentChangedEvent;
        
        public static void SetEquipment(EquipmentType equipmentType, int level) {
            PlayerPrefs.SetInt(equipmentType.DisplayName(), level);
            var result = Services.Network.GetLocalNetworkState();
            switch (result) {
                case Success<PlayerNetworkState> success:
                    success.Value.SetEquipmentLevel(equipmentType, level);
                    PlayerEquipmentChangedEvent?.Invoke();
                    break;
                
                case Failure<PlayerNetworkState> failure:
                    Debug.Log($"Failed to set equipment level: {failure.Message}");
                    break;
            }
        }

        public static int GetEquipment(EquipmentType equipmentType) {
            return PlayerPrefs.GetInt(equipmentType.DisplayName(), 1);
        }

        public static void AddEquipment(EquipmentType equipmentType, int count) {
            for (var i = 0; i < count; i++) {
                PlayerPrefs.SetInt(equipmentType.DisplayName(), GetEquipment(equipmentType) + 1);
                PlayerEquipmentChangedEvent?.Invoke();
            }
        }

        // --------------------------------------------------------------------
        /// Static event handlers must be manually reset when Domain Reload is off.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Init() {
            PlayerCashChangedEvent = null;
            PlayerUpgradeChipsChangedEvent = null;
            PlayerEquipmentChangedEvent = null;
            PlayerExperienceChangedEvent = null;
        }

    }

}