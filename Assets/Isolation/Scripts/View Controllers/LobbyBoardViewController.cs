using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Isolation.Scripts.View_Controllers {

    public class LobbyBoardViewController: ViewController {

        public Text[] PlayerNames;
        public Text[] PlayerLevels;
        public Text[] PlayerReadys;

        public GameObject LoadingSpinner;
        public GameObject[] OtherBoardPieces;

        public Dropdown DifficultyDropdown;

        public Text RoomCodeLabel;

        public Button StartButton;

        public LobbyBoardViewController(): base(ScreenType.Lobby) {
            Debug.Log($"[LobbyBoardViewController] Init.");
        }

        private void OnEnable() {
            DifficultyDropdown.onValueChanged.AddListener(OnDifficultyChange);
            Services.Network.OnPlayerJoined += UpdateBoard;
            Services.Network.OnPlayerLeft += UpdateBoard;
        }

        private void OnDisable() {
            DifficultyDropdown.onValueChanged.RemoveListener(OnDifficultyChange);
            Services.Network.OnPlayerJoined -= UpdateBoard;
            Services.Network.OnPlayerLeft -= UpdateBoard;
        }

        private void Start() {
            UniTask.Void(async () => {
                await UniTask.Delay(1_000);
                UpdateBoard();
            });
        }

        private void Update() {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) {
                ToggleAttention();
            }
        }

        private void ToggleAttention() {
            var localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var camera = localPlayer.GetComponentInChildren<Camera>();
            camera.enabled = !camera.enabled;
            if (camera.enabled) UIManager.LockCursor(); else UIManager.FreeCursor();
        }

        private void UpdateBoard(ulong uid) => UpdateBoard();
        
        private void UpdateBoard() {
            var i = 0;
            var isEveryoneReady = true;
            foreach (var client in Services.Network.Connections()) {
                if (!client.PlayerObject) {
                    Debug.Log($"Player has no object: {client.ClientId}");
                    continue;
                }
                var playerState = client.PlayerObject.GetComponentInChildren<PlayerNetworkState>();
                if (playerState.DisplayName is { Value: { } username }) {
                    PlayerNames[i].text = username;
                }
                var isReady = playerState.IsReady.Value;
                PlayerReadys[i].text = isReady ? "READY" : "NOT READY";
                isEveryoneReady &= isReady;

                playerState.IsReady.OnValueChanged += UpdateBoard;
                
                if (++i == 4) break;
            }
            StartButton.interactable = Services.Network.IsHost && isEveryoneReady;
            DifficultyDropdown.interactable = Services.Network.IsHost;
            RoomCodeLabel.text = Services.Unity.Lobby.Name;
        }

        private void UpdateBoard(bool previousvalue, bool newvalue) {
            UpdateBoard();
        }

        public void OnLeaveClicked() {
            Services.Network.Quit();
            SceneManager.LoadScene("Launcher");
        }

        public void OnStartClicked() {
            ToggleAttention();
            foreach (var otherBoardPiece in OtherBoardPieces) {
                otherBoardPiece.SetActive(false);
            }
            LoadingSpinner.SetActive(true);
            SceneManager.LoadScene("Map");
        }

        public void OnReadyClicked() {
            switch (Services.Network.GetLocalNetworkState()) {
                case Success<PlayerNetworkState> result:
                    var state = result.Value;
                    state.IsReady.Value = !state.IsReady.Value;
                    break;
                
                case Failure<PlayerNetworkState> failure:
                    Debug.Log($"Failed to set ready state: {failure.Message}");
                    break;
            }
        }

        private void OnDifficultyChange(Int32 newValue) {
            DifficultyDropdown.value = newValue;
            GameState.Difficulty.Value = (DifficultyLevel) newValue;
        }

    }
    
}