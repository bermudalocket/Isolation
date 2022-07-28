using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Isolation.Scripts.InGame {

    public class ColdSpot: MonoBehaviour {

        private async UniTaskVoid Start() {
            await UniTask.Delay(10_000);
            Destroy(gameObject);
        }

    }

}