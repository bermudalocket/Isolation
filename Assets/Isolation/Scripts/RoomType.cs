using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Isolation.Scripts {

    public enum RoomType {
        Start,
        Power,
        Dorm,
        Bathroom,
        Kitchen,
        Plus,
        
        I_EW,
        I_NS,
        
        L_NW,
        L_NE,
        L_SE,
        L_SW,
        
        T_EWN,
        T_EWS,
        T_NSE,
        T_NSW,
    }

    public enum RoomCategory {
        Hallway,
        Room,
        Special
    }

    public static class RoomTypeExtensions {

        public static string AddressKey(this RoomType type) {
            var key = type switch {
                RoomType.Start => "StartingRoom",
                RoomType.Power => "PowerRoom",
                RoomType.Dorm => "Dorm",
                RoomType.Kitchen => "Kitchen",
                RoomType.Bathroom => "Bathroom",
                RoomType.Plus => "Plus",

                RoomType.I_EW => "I-EW",
                RoomType.I_NS => "I-NS",
                RoomType.L_NW => "L-NW",
                RoomType.L_NE => "L-NE",
                RoomType.L_SE => "L-SE",
                RoomType.L_SW => "L-SW",
                RoomType.T_EWN => "T-EWN",
                RoomType.T_EWS => "T-EWS",
                RoomType.T_NSE => "T-NSE",
                RoomType.T_NSW => "T-NSW",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            return key;
        }

        public static List<Vector3> Exits(this RoomType type) {
            return type switch {
                RoomType.Start => new List<Vector3> { Vector3.right },
                RoomType.Power => new List<Vector3> { Vector3.back },
                RoomType.I_NS => new List<Vector3> { Vector3.forward, Vector3.back },
                RoomType.I_EW => new List<Vector3> { Vector3.left, Vector3.right },
                RoomType.Plus => new List<Vector3> { Vector3.left, Vector3.right, Vector3.forward, Vector3.back },
                RoomType.T_EWN => new List<Vector3> { Vector3.left, Vector3.right, Vector3.forward },
                RoomType.T_EWS => new List<Vector3> { Vector3.left, Vector3.right, Vector3.back },
                RoomType.T_NSE => new List<Vector3> { Vector3.right, Vector3.forward, Vector3.back },
                RoomType.T_NSW => new List<Vector3> { Vector3.left, Vector3.forward, Vector3.back },
                RoomType.L_NE => new List<Vector3> { Vector3.forward, Vector3.right },
                RoomType.L_NW => new List<Vector3> { Vector3.left, Vector3.forward },
                RoomType.L_SE => new List<Vector3> { Vector3.right, Vector3.back },
                RoomType.L_SW => new List<Vector3> { Vector3.left, Vector3.back },
                RoomType.Dorm => new List<Vector3> { Vector3.forward },
                RoomType.Bathroom => new List<Vector3> { Vector3.back },
                RoomType.Kitchen => new List<Vector3> { Vector3.back },
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public static int Limit(this RoomType type) {
            return type switch {
                RoomType.Power => 1,
                _ => int.MaxValue
            };
        }

    }

    public static class RoomTypeHelper {

        public static readonly List<RoomType> SpecialRoomTypes = new List<RoomType> {
            RoomType.Dorm, RoomType.Bathroom, RoomType.Kitchen
        };

        private static readonly RoomType[] RoomTypes = {
            RoomType.Power,
            RoomType.Dorm,
            RoomType.Bathroom,
            RoomType.Kitchen
        };

        private static readonly RoomType[] HallwayTypes = {
            RoomType.L_NW, RoomType.L_NE, RoomType.L_SW, RoomType.L_SE,
            RoomType.I_NS, RoomType.I_EW,
            RoomType.Plus,
            RoomType.T_EWN, RoomType.T_EWS, RoomType.T_NSW, RoomType.T_NSE
        };

        public static IEnumerable<RoomType> ByCategory(RoomCategory category) {
            return category switch {
                RoomCategory.Room => RoomTypes,
                RoomCategory.Hallway => HallwayTypes,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
            };
        }

        public static IEnumerable<RoomType> RoomsThatOpen(Vector3 direction, RoomCategory category = RoomCategory.Hallway) {
            return ByCategory(category).Where(type => type.Exits().Contains(direction));
        }

    }
}
