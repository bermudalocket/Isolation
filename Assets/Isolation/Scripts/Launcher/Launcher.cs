using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Isolation.Scripts.Launcher {

    public class Launcher: MonoBehaviour {

        public Text StatusText;
        
        public GameObject LoadingSpinner;

        public GameObject MainPanel;
        public GameObject JoinPanel;
        public GameObject SettingsPanel;
        
        public Button HostButton;
        public Button JoinButton;

        public TMP_InputField RoomCodeInputField;

        private void Awake() {
            StatusText.text = "Connecting to game server...";
            UniTask.Void(SignIn);
        }

        private async UniTaskVoid SignIn() {
            switch (await Services.Auth.SignIn()) {
                case Success<bool>:
                    HostButton.interactable = true;
                    JoinButton.interactable = true;
                    StatusText.text = $"Welcome back, <b>{Services.Auth.Username}</b>.";
                    break;
                
                case Failure<bool> failure:
                    StatusText.text = $"<color=red>{failure.Message}</color>";
                    break;
            }
        }

        public void OnJoinClicked() {
            var code = RoomCodeInputField.text;
            UniTask.Void(async () => {
                var result = await Services.Unity.JoinRelayServer(code);
                if (!result) {
                    StatusText.text = "<color=red>Failed to join relay server</color>";
                }
            });
        }

        private async UniTaskVoid JoinGameTask(string code) {
            var result = await Services.Unity.JoinRelayServer(code);
            if (!result) {
                StatusText.text = "<color=red>Failed to join relay server</color>";
            }
        }

        public void OnQuitButtonClicked() {
            Application.Quit();
        }

        public void BackToMainMenu() {
            JoinPanel.SetActive(false);
            SettingsPanel.SetActive(false);
            MainPanel.SetActive(true);
        }

        public void OnHostGameClicked() {
            UniTask.Void(HostGame);
        }

        private async UniTaskVoid HostGame() {
            if (await Services.Unity.HostRelayServer()) {
                MainPanel.SetActive(false);
                LoadingSpinner.SetActive(true);
                UniTask.Void(LoadLobby);
            } else {
                StatusText.text = "<color=red>Failed to host relay server</color>";
            }
        }

        public void OnSettingsClicked() {
            MainPanel.SetActive(false);
            SettingsPanel.SetActive(true);
        }

        public void CloseSettingsView() {
            SettingsPanel.SetActive(false);
            MainPanel.SetActive(true);
        }

        public void OnJoinGameClicked() {
            MainPanel.SetActive(false);
            JoinPanel.SetActive(true);
        }

        private async UniTaskVoid LoadLobby() {
            var progress = SceneManager.LoadSceneAsync("Lobby");
            while (!progress.isDone && StatusText) {
                var percent = Math.Round(progress.progress * 100);
                StatusText.text = $"Loading lobby ({percent}%)";
                await UniTask.Yield();
            }
        }

    }
}