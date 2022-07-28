using System;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using Isolation.Scripts.InGame;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Isolation.Scripts.Equips {

    public class Thermometer: AbstractEquipment {

        public TextMeshPro Display;

        private readonly Collider[] _resultsBag = new Collider[10];

        private void Awake() {
            var token = this.GetCancellationTokenOnDestroy();
            UniTask.Void(UpdateLoop, token);
        }

        private protected override void OnShow() {
            gameObject.SetActive(true);
        }

        private protected override void OnHide() {
            gameObject.SetActive(false);
        }

        public void TakeReading() {
            var baseTemp = 40f;
            if (Physics.OverlapSphereNonAlloc(transform.position, 3f, _resultsBag, LayerMask.GetMask("ColdSpot")) > 0) {
                baseTemp = 0f;
            }
            var temp = Math.Round(baseTemp + Random.Range(-3f, 3f));
            Display.text = temp.ToString(CultureInfo.InvariantCulture);
        }

        private async UniTaskVoid UpdateLoop(CancellationToken cancellationToken) {
            while (true) {
                if (cancellationToken.IsCancellationRequested) {
                    Debug.Log("- stopping TempUpdateLoop on Thermometer");
                    break;
                }
                TakeReading();
                await UniTask.Delay(1500, cancellationToken: cancellationToken);
            }
        }

    }

}