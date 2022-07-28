using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Isolation.Scripts.View_Controllers {
    
    public class VendingMachineViewController: ViewController {

        public AudioSource AudioSource;
        public AudioClip PurchaseClip;
        public AudioClip FailedClip;

        public VerticalLayoutGroup ButtonContainer;
        public VerticalLayoutGroup InventoryContainer;

        public TextMeshProUGUI CashLabel;
        public Image ShopBackground;
        
        public VendingMachineViewController(): base(ScreenType.Shop) { }

        private async void Awake() {
            var parent = ButtonContainer.transform;
            foreach (var type in EquipmentTypeExtensions.All) {
                var go = await Addressables.InstantiateAsync("Button", Vector3.zero, Quaternion.identity, parent);
                go.name = $"Buy{type.SimpleName()}Button";
                var button = go.GetComponent<Button>();
                var text = button.GetComponentInChildren<Text>();
                text.fontSize = 36;
                text.text = $"{type.DisplayName()} (${type.Cost()})";
                button.onClick.AddListener(() => Purchase(type));       
            }
        }

        private void OnEnable() {
            OnEquipmentChanged();
            CashLabel.text = "$" + PlayerState.Cash;
            PlayerState.PlayerEquipmentChangedEvent += OnEquipmentChanged;
            PlayerState.PlayerCashChangedEvent += OnPlayerCashChanged;
        }

        private void OnDisable() {
            PlayerState.PlayerEquipmentChangedEvent -= OnEquipmentChanged;
            PlayerState.PlayerCashChangedEvent -= OnPlayerCashChanged;
        }

        private void OnEquipmentChanged() {
            var t = InventoryContainer.transform;
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

        private void OnPlayerCashChanged(int change, int total) {
            CashLabel.text = "$" + total;
        }

        private void Purchase(EquipmentType type) {
            Debug.Log($"[VendingMachineViewController] Purchase requested: <b>{type.DisplayName()}</b>");
            if (PlayerState.Cash < type.Cost()) {
                PurchaseFailed();
                return;
            }
            PlayerState.Cash -= type.Cost();
            PlayerState.AddEquipment(type, 1);
            AudioSource.PlayOneShot(PurchaseClip);
        }

        private void PurchaseFailed() {
            AudioSource.PlayOneShot(FailedClip);
            ShopBackground.color = new Color(0.68f, 0.18f, 0.21f, 0.34f);
            CashLabel.color = Color.red;
            UniTask.Run(async () => {
                await UniTask.Delay(2000);
                CashLabel.color = Color.white;
                ShopBackground.color = new Color(0.18f, 0.69f, 0.23f, 0.34f);
            });
        }

    }

}