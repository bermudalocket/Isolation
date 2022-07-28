using System;

namespace Isolation.Scripts {

    public abstract class Result {
        public bool Success { get; protected init; }
    }

    public abstract class Result<T>: Result {

        // source of truth
        private readonly T _value;

        // backed by _value
        public T Value {
            get {
                if (Success) return _value;
                throw new Exception(
                    $"You can't access .{nameof(Value)} when .{nameof(Success)} is false."
                );
            }
            init => _value = value;
        }

        protected Result(T value) {
            Value = value;
        }

    }

    public sealed class Success<T>: Result<T> {
        
        public Success(T value): base(value) {
            Success = true;
        }

    }
    
    public sealed class Failure<T>: Result<T> {

        public Failure(string message): base(default) {
            Message = message;
            Success = false;
        }

        public string Message { get; }

    }

}