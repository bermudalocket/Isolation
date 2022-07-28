namespace Isolation.Scripts {

    public enum DifficultyLevel {
        Easy, Medium, Hard
    }

    public static class DifficultyLevelExtensions {

        public static int SafeTime(this DifficultyLevel level) {
            return level switch {
                DifficultyLevel.Easy => 300,
                DifficultyLevel.Medium => 150,
                DifficultyLevel.Hard => 0,
                _ => 0,
            };
        }
        
        public static int MaxRooms(this DifficultyLevel level) {
            return level switch {
                DifficultyLevel.Easy => 25,
                DifficultyLevel.Medium => 50,
                DifficultyLevel.Hard => 75,
                _ => 25,
            };
        }
        
        public static int ExpMultiplier(this DifficultyLevel level) {
            return level switch {
                DifficultyLevel.Easy => 1,
                DifficultyLevel.Medium => 2,
                DifficultyLevel.Hard => 4,
                _ => 1,
            };
        }

    }

}