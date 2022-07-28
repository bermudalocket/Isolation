using Unity.Netcode;

namespace Isolation.Scripts.InGame {

    public abstract class Singleton<T>: NetworkBehaviour where T: new() {

        private T _instance;

        public T Instance {
            get {
                if (_instance is null) {
                    _instance = new T();
                }
                return _instance;
            }
        }

        private void Awake() {
            if (_instance is { } instance && !Equals(instance, this)) {
                Destroy(this);
                return;
            }
        }

    }

}