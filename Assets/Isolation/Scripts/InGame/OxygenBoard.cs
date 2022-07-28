using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;

namespace Isolation.Scripts.InGame {
    
    public class OxygenBoard: MonoBehaviour {

        public TextMeshProUGUI[] PlayerNames;
        public TextMeshProUGUI[] OxygenLabels;

        private void Start() {
            UniTask.Void(UpdateBoard, this.GetAsyncDestroyTrigger().CancellationToken);
        }

        private async UniTaskVoid UpdateBoard(CancellationToken cancellationToken) {
            while (isActiveAndEnabled) {
                if (cancellationToken.IsCancellationRequested) {
                    Debug.Log($"[OxygenBoard] Stopping UpdateBoard task.");
                    break;
                }
                var players = Services.Network.Connections();
                switch (players.Count) {
                    case 1:
                        PlayerNames[1].text = "";
                        OxygenLabels[1].text = "";
                        PlayerNames[2].text = "";
                        OxygenLabels[2].text = "";
                        PlayerNames[3].text = "";
                        OxygenLabels[3].text = "";
                        break;
                    case 2:
                        PlayerNames[2].text = "";
                        OxygenLabels[2].text = "";
                        PlayerNames[3].text = "";
                        OxygenLabels[3].text = "";
                        break;
                    case 3:
                        PlayerNames[3].text = "";
                        OxygenLabels[3].text = "";
                        break;
                }
                for (var i = 0; i < Constants.MaxPlayers && i < players.Count; i++) {
                    var player = players[i];
                    var result = player.GetNetworkState();
                    if (result is not { } state) {
                        Debug.Log("Failed to get player details");
                        continue;
                    }
                    PlayerNames[i].text = state.DisplayName.Value;
                    OxygenLabels[i].text = $"{state.OxygenLevel.Value}%";

                    if (!state.IsAlive.Value) {
                        PlayerNames[i].color = Color.gray;
                        PlayerNames[i].fontStyle = FontStyles.Strikethrough;
                        OxygenLabels[i].text = "";
                    }
                }
                await UniTask.Delay(1_000, cancellationToken: cancellationToken);
            }
        }

    }
}