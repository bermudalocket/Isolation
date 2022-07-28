using System;
using UnityEngine;
using UnityEngine.Events;

namespace Isolation.Scripts {

    public class CursorModifier: MonoBehaviour {

        public HUDManager.ReticleType Type;

        public string Title;

        public string Subtitle;

        [Range(0.1f, 20f)]
        public float MaxDistance = 10f;

        public UnityEvent Action;

        public Outline Outline;

        public DateTime LastLook;

        private void Update() {
            if (DateTime.Now - LastLook > TimeSpan.FromMilliseconds(50)) {
                SetOutlineVisible(false);
            }
        }

        public void SetOutlineVisible(bool visible) {
            if (visible) LastLook = DateTime.Now;
            if (Outline) Outline.enabled = visible;
        }

    }

}