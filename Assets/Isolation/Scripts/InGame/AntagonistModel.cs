using System.Collections.Generic;

namespace Isolation.Scripts.InGame {

    public readonly struct AntagonistModel {

        public static readonly AntagonistModel Crawler = new AntagonistModel("Crawler");
        public static readonly AntagonistModel Stomper = new AntagonistModel("Stomper");
        public static readonly AntagonistModel Ghoul = new AntagonistModel("Ghoul");

        public static AntagonistModel Random() => All.RandomElement();
        public static readonly List<AntagonistModel> All = new List<AntagonistModel> {
            Crawler, Stomper, Ghoul
        };

        public readonly string AddressKey;
        
        private AntagonistModel(string key) {
            AddressKey = key;
        }
        
        public static bool operator ==(AntagonistModel lhs, AntagonistModel rhs) {
            return lhs.AddressKey == rhs.AddressKey;
        }

        public static bool operator !=(AntagonistModel lhs, AntagonistModel rhs) {
            return !(lhs == rhs);
        }
        
        public bool Equals(AntagonistModel other) {
            return AddressKey == other.AddressKey;
        }

        public override bool Equals(object obj) {
            return obj is AntagonistModel other && Equals(other);
        }

        public override int GetHashCode() {
            return (AddressKey != null ? AddressKey.GetHashCode() : 0);
        }

    }

}