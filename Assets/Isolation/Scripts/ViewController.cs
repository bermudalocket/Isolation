using UnityEngine;

namespace Isolation.Scripts {

    public class ViewController: MonoBehaviour {

        private readonly ScreenType _type;

        public bool IsOpen => UIManager.Instance.IsOpen(_type);

        public ViewController(ScreenType type) {
            _type = type;
        }

        public virtual void Open() => UIManager.Instance.Open(_type);

        public virtual void Close() => UIManager.Instance.Close(_type);

    }

}