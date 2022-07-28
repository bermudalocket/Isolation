using UnityEngine;

namespace Isolation.Scripts {

    public class Room: MonoBehaviour {

        public RoomType Type;

        public float i, j;
        
        public Vector3 WorldPos => 12f * new Vector3(i, 0, j);

    }
    
}