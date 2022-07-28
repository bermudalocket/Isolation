using System;

namespace Isolation.Scripts {

    [Serializable]
    public enum EquipmentType {
        Flashlight,
        VideoCamera,
        PhotoCamera,
        Thermometer,
        UVLight
    }

    public static class EquipmentTypeExtensions {

        public static readonly EquipmentType[] All = {
            EquipmentType.Flashlight,
            EquipmentType.VideoCamera,
            EquipmentType.PhotoCamera,
            EquipmentType.Thermometer,
            EquipmentType.UVLight
        };

        public static string Key(this EquipmentType type) => SimpleName(type);

        public static string DisplayName(this EquipmentType type) {
            return type switch {
                EquipmentType.Flashlight => "Flashlight",
                EquipmentType.Thermometer => "Thermometer",
                EquipmentType.PhotoCamera => "PhotoCamera",
                EquipmentType.VideoCamera => "VideoCamera",
                EquipmentType.UVLight => "UV Light",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public static string SimpleName(this EquipmentType type) {
            return DisplayName(type).Replace(" ", "");
        }
        
        public static int Cost(this EquipmentType type) {
            return type switch {
                EquipmentType.Flashlight => 40,
                EquipmentType.Thermometer => 20,
                EquipmentType.PhotoCamera => 50,
                EquipmentType.VideoCamera => 80,
                EquipmentType.UVLight => 60,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

    }

}


