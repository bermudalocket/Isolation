using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Isolation.Scripts.View_Controllers {
    public class CrewBoardViewController: MonoBehaviour {

        public TextMeshProUGUI[] ShipLabels;
        public TextMeshProUGUI[] StatusLabels;

        public void Start() {
            UniTask.Void(DoUpdate, this.GetCancellationTokenOnDestroy());
        }

        private async UniTaskVoid DoUpdate(CancellationToken token) {
            while (isActiveAndEnabled) {
                if (token.IsCancellationRequested) {
                    break;
                }

                for (var i = 0; i < ShipLabels.Length; i++) {
                    ShipLabels[i].text = Random.Range(100_000, 999_999).ToString();
                    StatusLabels[i].text = Random.value switch {
                        < 0.15f => "Safe",
                        < 0.4f => "Lost Contact",
                        _ => "In Isolation"
                    };
                    StatusLabels[i].color = StatusLabels[i].text switch {
                        "Safe" => Color.white,
                        "Lost Contact" => Color.red,
                        "In Isolation" => Color.green,
                        _ => Color.white
                    };
                }
                await UniTask.Delay(4_000, cancellationToken: token);
            }
        }

    }
}