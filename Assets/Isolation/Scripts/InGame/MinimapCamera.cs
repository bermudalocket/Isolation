using System.Linq;
using UnityEngine;

namespace Isolation.Scripts.InGame {

    public class MinimapCamera: MonoBehaviour {

        public static MinimapCamera Instance { get; private set; }

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(this);
            } else {
                Instance = this;
            }
        }

        public void FindCentralPosition() {
            var rooms = FindObjectsOfType<Room>();
            if (rooms.Length == 0) {
                return;
            }
            var average = rooms.Aggregate(Vector3.zero, (agg, room) => agg + room.WorldPos) / rooms.Length;
            transform.position = new Vector3(average.x, transform.position.y, average.z);
        }

    }
}