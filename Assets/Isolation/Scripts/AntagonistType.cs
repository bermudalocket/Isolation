using System;

namespace Isolation.Scripts {

    public enum NetworkAntagonistType {
        Charra, Konzi, Irax, Okon, Ttaloo, Brathu, Zutu, Xagor, Nyri, Ruma, VidaGulu, Pox
    }

    public readonly struct AntagonistType: IEquatable<string> {

        private static readonly AntagonistType Charra = new(
            "Charra", 
            Evidence.AlphaRadiation,
            new [] { Evidence.Residue, Evidence.GravitationalDisplacement }
        );

        private static readonly AntagonistType Konzi = new(
            "Konzi", 
            Evidence.AlphaRadiation,
            new [] { Evidence.ElectromagneticDisturbances, Evidence.Freezing }
        );

        private static readonly AntagonistType Irax = new(
            "Irax", 
            Evidence.AlphaRadiation,
            new [] { Evidence.Residue, Evidence.Voices}
        );

        private static readonly AntagonistType Okon = new(
            "Okon", 
            Evidence.AlphaRadiation,
            new [] { Evidence.GravitationalDisplacement, Evidence.ElectromagneticDisturbances }
        );

        private static readonly AntagonistType TTaloo = new(
            "TTaloo", 
            Evidence.BetaRadiation,
            new [] { Evidence.Residue, Evidence.Lifesigns }
        );

        private static readonly AntagonistType Brathu = new(
            "Brathu", 
            Evidence.BetaRadiation,
            new [] { Evidence.ElectromagneticDisturbances, Evidence.Freezing }
        );

        private static readonly AntagonistType Zutu = new(
            "Zutu", 
            Evidence.BetaRadiation,
            new [] { Evidence.GravitationalDisplacement, Evidence.Lifesigns }
        );

        private static readonly AntagonistType Xagor = new(
            "Xagor", 
            Evidence.BetaRadiation,
            new [] { Evidence.GravitationalDisplacement, Evidence.Freezing }
        );

        private static readonly AntagonistType Nyri = new(
            "Nyri", 
            Evidence.GammaRadiation,
            new [] { Evidence.Voices, Evidence.GravitationalDisplacement }
        );

        private static readonly AntagonistType Ruma = new(
            "Ruma", 
            Evidence.GammaRadiation,
            new [] { Evidence.ElectromagneticDisturbances, Evidence.Freezing }
        );

        private static readonly AntagonistType VidaGulu = new(
            "VidaGulu", 
            Evidence.GammaRadiation,
            new [] { Evidence.Voices, Evidence.Lifesigns }
        );

        private static readonly AntagonistType Pox = new(
            "Pox", 
            Evidence.GammaRadiation,
            new [] { Evidence.Residue, Evidence.Voices }
        );

        public static AntagonistType Random() => AllTypes.RandomElement();

        public static readonly AntagonistType[] AllTypes = {
            Charra, Konzi, Irax, Okon, TTaloo, Brathu, Zutu, Xagor, Nyri, Ruma, VidaGulu, Pox,
        };

        // --------------------------------------------------------------------

        public string DisplayName { get; }
        public Evidence RadiationType { get; }
        public Evidence[] RequiredEvidence { get; }

        private AntagonistType(string displayName, Evidence radiationType, Evidence[] requiredEvidence) {
            DisplayName = displayName;
            RadiationType = radiationType;
            RequiredEvidence = requiredEvidence;
        }

        public static AntagonistType FromString(string str) {
            foreach (var type in AllTypes) {
                if (type.Equals(str)) {
                    return type;
                }
            }
            return default;
        }

        public static AntagonistType FromNetworkType(NetworkAntagonistType type) {
            return type switch {
                NetworkAntagonistType.Brathu => Brathu,
                NetworkAntagonistType.Charra => Charra,
                NetworkAntagonistType.Irax => Irax,
                NetworkAntagonistType.Konzi => Konzi,
                NetworkAntagonistType.Nyri => Nyri,
                NetworkAntagonistType.Okon => Okon,
                NetworkAntagonistType.Pox => Pox,
                NetworkAntagonistType.Ttaloo => TTaloo,
                NetworkAntagonistType.Ruma => Ruma,
                NetworkAntagonistType.Xagor => Xagor,
                NetworkAntagonistType.Zutu => Zutu,
                NetworkAntagonistType.VidaGulu => VidaGulu,
            };
        }


        public bool Equals(AntagonistType other) => DisplayName == other.DisplayName;
        public bool Equals(string other) => DisplayName == other;
        public override bool Equals(object obj) => obj is AntagonistType other && Equals(other);

        public override int GetHashCode() => DisplayName != null ? DisplayName.GetHashCode() : 0;

        public static bool operator ==(AntagonistType a, AntagonistType b) => a.Equals(b);
        public static bool operator !=(AntagonistType a, AntagonistType b) => !(a == b);

        public NetworkAntagonistType NetworkTyped() {
            if (this == Brathu) {
                return NetworkAntagonistType.Brathu;
            } else if (this == Charra) {
                return NetworkAntagonistType.Charra;
            } else if (this == Irax) {
                return NetworkAntagonistType.Irax;
            } else if (this == Konzi) {
                return NetworkAntagonistType.Konzi;
            } else if (this == Nyri) {
                return NetworkAntagonistType.Nyri;
            } else if (this == Okon) {
                return NetworkAntagonistType.Okon;
            } else if (this == Pox) {
                return NetworkAntagonistType.Pox;
            } else if (this == TTaloo) {
                return NetworkAntagonistType.Ttaloo;
            } else if (this == Ruma) {
                return NetworkAntagonistType.Ruma;
            } else if (this == Xagor) {
                return NetworkAntagonistType.Xagor;
            } else if (this == Zutu) {
                return NetworkAntagonistType.Zutu;
            } else {
                return NetworkAntagonistType.VidaGulu;
            }
        }
    }

}