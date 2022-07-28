using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Isolation.Scripts {

    public class Publisher<T> {

        private T _value;

        public T Value => _value;

        public Publisher(T defaultValue) {
            _value = defaultValue;
        }

        public void Send(T value) {
            _value = value;
            _callbacks.ForEach(c => c.Invoke(value));
        }

        private List<Action<T>> _callbacks;

        public void Subscribe(Action<T> callback) {
            _callbacks.Add(callback);
        }

        public void Unsubscribe(Action<T> callback) {
            _callbacks.Remove(callback);
        }

        public void Subscribe(IsolationEventHandler handler) {
            
        }
        
    }

    public enum IsolationEvent {
        Power
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class IsolationEventHandler: Attribute {

        private IsolationEvent _type;

        public IsolationEventHandler(IsolationEvent type) {
            _type = type;
        }

    }

    public sealed class GameState: NetworkBehaviour {

        public static GameStage Stage = GameStage.Lobby;

        public static DifficultyLevel Difficulty = DifficultyLevel.Easy;

        public static int SafeTimeRemaining = DifficultyLevel.Easy.SafeTime();

        public static readonly Publisher<bool> IsPowerOn = new(false);

        public static NetworkAntagonistType Answer = NetworkAntagonistType.Brathu;

    }

}