using UnityEngine;

namespace Isolation.Scripts.InGame {
    public class LeaveConsole: MonoBehaviour {

        public void OnMouseUpAsButton() {
            GameState.Stage.Value = GameStage.PostGame;
            Services.Network.Load("Lobby");
        }

    }
}