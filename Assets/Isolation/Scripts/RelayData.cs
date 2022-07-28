using System;

namespace Isolation.Scripts {

    public struct RelayData {
        public Guid AllocationID;
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public byte[] AllocationIDBytes;
        public byte[] Key;
        public byte[] HostConnectionData;
        public byte[] ConnectionData;
    }

}