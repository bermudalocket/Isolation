using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Isolation.Scripts.InGame {
    public class Clock: MonoBehaviour {

        private TextMeshPro _text;

        private void Start() {
            _text = GetComponentInChildren<TextMeshPro>();
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown() {
            while (true) {
                var time = GameState.SafeTimeRemaining;
                if (time < 0) break;
                var minutes = Math.Floor(time / 60.0);
                var seconds = time % 60;
                var secondsString = seconds < 10 ? $"0{seconds}" : $"{seconds}";
                _text.text = $"{minutes}:{secondsString}";
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }

    }
}