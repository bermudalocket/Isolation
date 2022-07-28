using System;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace Isolation.Scripts {

    public class NetworkedString: NetworkVariableBase {

        private string? _value;

        public string? Value {
            get => _value;
            set => _value = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref _value);
        }

        public override void WriteDelta(FastBufferWriter writer) {
            WriteField(writer);
        }

        public override void WriteField(FastBufferWriter writer) {
            var bytes = PushToBuffer();
            if (writer.TryBeginWrite(bytes + sizeof(int))) {
                writer.WriteValue(bytes);
                writer.WriteBytes(buffer, bytes, 0);
            } else {
                Debug.Log("Failed to write string to writer.");
            }
        }

        public override void ReadField(FastBufferReader reader) {
            ReadDelta(reader, false);
        }

        public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta) {
            reader.ReadValueSafe(out int bytes);
            if (reader.TryBeginRead(bytes)) {
                reader.ReadBytes(ref buffer, bytes, 0);
                _value = Encoding.UTF8.GetString(buffer, 0, bytes);
            } else {
                Debug.Log("Failed to read string from reader.");
            }
        }
        
        internal static byte[] buffer = new byte[4096];

        unsafe int PushToBuffer() {
            if (_value == null)
                return 0;

            var bytesWritten = default(int);
            var encoder = Encoding.UTF7.GetEncoder();
            var charStep = 50;
            var byteStep = Encoding.UTF7.GetMaxByteCount(charStep);

            var remainingChars = Value.Length;
            var remainingBytes = buffer.Length;

            fixed (byte* bytes = buffer)
            fixed (char* chars = _value) {
                while (remainingChars > 0 && remainingBytes > byteStep) {
                    var result = encoder.GetBytes(
                        chars + (_value.Length - remainingChars), // start of char array
                        Math.Min(charStep, remainingChars), // num of chars to write
                        bytes + (buffer.Length - remainingBytes), // start of byte array          
                        remainingBytes, // max bytes to write
                        true
                    );

                    bytesWritten += result;
                    remainingBytes -= result;
                    remainingChars -= charStep;
                }
            }

            return bytesWritten;
        }

    }

}