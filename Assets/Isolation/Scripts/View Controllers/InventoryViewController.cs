using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Isolation.Scripts.View_Controllers {
    
    public class InventoryViewController: MonoBehaviour {

        public VerticalLayoutGroup Container;

        private void OnEnable() {
            Refresh();
            PlayerState.PlayerEquipmentChangedEvent += Refresh;
        }

        private void OnDisable() {
            PlayerState.PlayerEquipmentChangedEvent -= Refresh;
        }

        private void Refresh() {
            ClearContainer();
            foreach (var type in EquipmentTypeExtensions.All) {
                var label = new GameObject($"Label - ${type}");
                var text = label.AddComponent<TextMeshPro>();
                text.text = $"{type.DisplayName()} - lv {PlayerState.GetEquipment(type)}";
            }
        }

        private void ClearContainer() {
            var t = Container.transform;
            for (var i = 0; i < t.childCount; i++) {
                Destroy(t.GetChild(i));
            }
        }

    }
    
}