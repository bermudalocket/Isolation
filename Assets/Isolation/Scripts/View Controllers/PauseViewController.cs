using UnityEngine.SceneManagement;

namespace Isolation.Scripts.View_Controllers {

    public class PauseViewController: ViewController {

        public PauseViewController(): base(ScreenType.Pause) { }

        public static void OnQuitButtonPressed() {
            switch (SceneManager.GetActiveScene().name) {
                case "Map":
                    Services.Network.Quit();
                    SceneManager.LoadScene("Lobby");
                    break;

                case "Lobby":
                    SceneManager.LoadScene("Launcher");
                    break;
            }
        }

        public static void OnSettingsButtonPressed() {
            UIManager.Instance.Open(ScreenType.Settings);
        }

    }

}