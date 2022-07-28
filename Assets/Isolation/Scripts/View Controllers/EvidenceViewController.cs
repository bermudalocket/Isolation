using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Isolation.Scripts.View_Controllers {

    public class EvidenceViewController: MonoBehaviour {

        public Toggle[] Toggles;
        [SerializeField] private Text antagonistNameTMP;

        private void Awake() {
            foreach (var toggle in Toggles) {
                toggle.onValueChanged.AddListener(DoToggle);
            }
        }

        private void DoToggle(bool isOn) {
            var chosenEvidences = new List<Evidence>();
            foreach (var toggle in Toggles) {
                var evidence = toggle.name switch {
                    "AlphaRadiation" => Evidence.AlphaRadiation,
                    "BetaRadiation" => Evidence.BetaRadiation,
                    "GammaRadiation" => Evidence.GammaRadiation,
                    "FreezingTemps" => Evidence.Freezing,
                    "Voices" => Evidence.Voices,
                    "Residue" => Evidence.Residue,
                    "Electromagnetic" => Evidence.ElectromagneticDisturbances,
                    "Gravity" => Evidence.GravitationalDisplacement,
                    "Lifesigns" => Evidence.Lifesigns,
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (toggle.isOn) {
                    chosenEvidences.Add(evidence);
                } else {
                    chosenEvidences.Remove(evidence);
                }
            }
            if (chosenEvidences.Count > 3) {
                antagonistNameTMP.text = "Ambiguity\nthreshold\nexceeded";
                antagonistNameTMP.color = Color.red;
                return;
            }
            if (chosenEvidences.Count < 3) {
                antagonistNameTMP.text = "Ambiguity\nthreshold\nexceeded";
                antagonistNameTMP.color = Color.red;
                return;
            }

            foreach (var antagonist in AntagonistType.AllTypes) {
                var matchCount = antagonist
                    .RequiredEvidence
                    .Count(requiredEvidence =>
                        chosenEvidences.Contains(requiredEvidence)
                    );
                if (matchCount != 3) continue;
                antagonistNameTMP.text = antagonist.DisplayName;
                antagonistNameTMP.color = Color.green;

                switch (Services.Network.GetLocalNetworkState()) {
                    case Success<PlayerNetworkState> success:
                        success.Value.Guess.Value = antagonist.NetworkTyped();
                        break;
                    
                    case Failure<PlayerNetworkState> failure:
                        Debug.Log($"[EvidenceViewController] Failed to set guess: {failure.Message}.");
                        break;
                }
                return;
            }
            antagonistNameTMP.text = "No Known\nMatches";
            antagonistNameTMP.color = Color.red;
        }

    }

}