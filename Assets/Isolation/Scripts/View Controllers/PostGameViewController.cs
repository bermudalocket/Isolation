using TMPro;
using UnityEngine;

namespace Isolation.Scripts.View_Controllers {
    
    public class PostGameViewController: MonoBehaviour {

        public TextMeshProUGUI MissionText;
        public TextMeshProUGUI StatusText;
        public TextMeshProUGUI LifeformText;
        public TextMeshProUGUI CashRewardText;
        public TextMeshProUGUI ExperienceRewardText;

        private void OnEnable() {
            if (GameState.Stage.Value != GameStage.PostGame) return;

            var result = Services.Network.GetLocalNetworkState();
            switch (result) {
                case Success<PlayerNetworkState> success:
                    break;
                
                case Failure<PlayerNetworkState> failure:
                    Debug.Log($"Failed to get player state: {failure.Message}");
                    ClosePostGamePanel();
                    return;
            }
            var state = result.Value;
            
            var answer = AntagonistType.FromNetworkType(GameState.Answer.Value);
            var guess = AntagonistType.FromNetworkType(state.Guess.Value);
            LifeformText.text = answer.DisplayName;

            var cash = guess == answer ? (int) RandomGaussian.Range(0, 200) : 0;
            PlayerState.Cash += cash;
            CashRewardText.text = $"${cash}";

            var xp = guess == answer ? 50 * GameState.Difficulty.Value.ExpMultiplier() : 0;
            PlayerState.Experience += xp;
            ExperienceRewardText.text = $"{xp}";

            MissionText.text = Services.Unity.Lobby?.Name ?? "--";

            StatusText.text = guess == answer ? "COMPLETE" : "UNSUCCESSFUL";
            StatusText.color = guess == answer ? Color.green : Color.gray;
            
            if (!state.IsAlive.Value) {
                StatusText.text = "FAILED";
                StatusText.color = Color.red;
            }
        }

        public void ClosePostGamePanel() {
            GameState.Stage.Value = GameStage.Lobby;
            UIManager.Instance.Close(ScreenType.PostGame);
        }

    }
    
}