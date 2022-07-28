using System.Reflection;
using UnityEngine;

namespace Isolation.Scripts {

    public struct IsolationEvents {

        public delegate void HuntEvent(bool isHunting);

        private static HuntEvent _huntEvent;
        public static event HuntEvent HuntStateChangedEvent {
            add => _huntEvent += value;
            remove => _huntEvent -= value;
        }
        
        // --------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetState() {
            var type = typeof(IsolationEvents);
            type.GetEvents(BindingFlags.Static | BindingFlags.Public)
                .ForEach(e => {
                    type.GetField(e.Name, BindingFlags.Static | BindingFlags.NonPublic)
                        ?.SetValue(null, null);
                });
        }

    }

}