using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Isolation.Scripts {
    
    public class LobbyPlayerBoardManager: MonoBehaviour {

        public TextMeshProUGUI CashLabel;

        public VerticalLayoutGroup Container;

        private void Start() {
            CashLabel.text = "$" + PlayerState.Cash;
            OnEquipmentChangedEvent();
            PlayerState.PlayerCashChangedEvent += OnPlayerCashChangedEvent;
            PlayerState.PlayerEquipmentChangedEvent += OnEquipmentChangedEvent;
        }
        
        private void OnDestroy() {
            PlayerState.PlayerCashChangedEvent -= OnPlayerCashChangedEvent;
            PlayerState.PlayerEquipmentChangedEvent -= OnEquipmentChangedEvent;
        }

        private void OnPlayerCashChangedEvent(int _, int total) {
            if (CashLabel) CashLabel.text = "$" + total;
        }

        private void OnEquipmentChangedEvent() {
            var t = Container.transform;
            for (var i = 0; i < t.childCount; i++) {
                Destroy(t.GetChild(i).gameObject);
            }

            foreach (var type in EquipmentTypeExtensions.All) {
                var count = PlayerState.GetEquipment(type);
                if (count == 0) continue;
                var label = new GameObject(type.DisplayName());
                label.transform.SetParent(t);
                var text = label.AddComponent<TextMeshProUGUI>();
                text.transform.localRotation = Quaternion.Euler(Vector3.zero);
                text.transform.localScale = Vector3.one;
                var transformLocalPosition = text.transform.localPosition;
                transformLocalPosition.z = 0f;
                text.transform.localPosition = transformLocalPosition;
                text.text = $"{count}x {type.DisplayName()}" + (count > 1 ? "s" : "");
            }
        }

    }

}