using System;
using UnityEngine;

namespace Isolation.Scripts.InGame {

    public class LightAlarm: MonoBehaviour {

        public new Light light;

        private float _intensityGoal = 0f;

        private void Awake() {
            light.intensity = _intensityGoal;
        }

        private void Update() {
            light.intensity = Mathf.Lerp(light.intensity, _intensityGoal, Time.deltaTime);
            if (Math.Abs(light.intensity - _intensityGoal) < 0.01f) {
                _intensityGoal = (_intensityGoal == 0f) ? 5f : 0f;
            }
        }

    }

}