using UnityEngine;

namespace Isolation.Scripts.Equips {

    public class RevealEffector: MonoBehaviour {

        public Transform effector;
        private Renderer _rend;
        private static readonly int Effectorpos = Shader.PropertyToID("_effectorpos");

        private void Start() {
            _rend = GetComponent<Renderer>();
            effector = GameObject.FindGameObjectWithTag("RevealEffector").transform;
        }

        private void Update() {
            _rend.sharedMaterial.SetVector(Effectorpos, effector.position);
        }

    }

}