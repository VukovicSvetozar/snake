using System.Windows.Media;
using Snake.Models;

namespace Snake.Services
{
    public static class GameConfig
    {
        public static int GridSize { get; set; } = 20;
        public static int TileSize { get; set; } = 20;
        public static int SnakeStartLength { get; set; } = 3;
        public static int SnakeStartSpeed { get; set; } = 400;
        public static int SnakeSpeedThreshold { get; set; } = 80;
        public static int InitialLevel { get; set; } = 1;
        public static int FoodPerLevel { get; set; } = 10;
        public static int MaxFoodCount { get; set; } = 3;
        public static Difficulty Difficulty { get; set; } = Difficulty.Easy;
        public static bool IsTimedMode { get; set; } = false;
        public static int GameTimeLimit { get; set; } = 360;
        public static bool AreObstaclesEnabled { get; set; } = false;
        public static int ObstacleCount { get; set; } = 5;

        public static TimeSpan TimerInterval { get; set; } = TimeSpan.FromMilliseconds(400);

        public static readonly Brush SnakeBodyColor = Brushes.Green;
        public static readonly Brush ObstacleColor = Brushes.Gray;

        public static readonly string SnakeHeadImageUp = "pack://application:,,,/Resources/Images/Snake_head_up.jpg";
        public static readonly string SnakeHeadImageDown = "pack://application:,,,/Resources/Images/Snake_head_down.jpg";
        public static readonly string SnakeHeadImageLeft = "pack://application:,,,/Resources/Images/Snake_head_left.jpg";
        public static readonly string SnakeHeadImageRight = "pack://application:,,,/Resources/Images/Snake_head_right.jpg";
        public static readonly string SnakeBodyImage = "pack://application:,,,/Resources/Images/Snake_body.jpg";

        public static readonly string ObstacleImage = "pack://application:,,,/Resources/Images/Obstacle.jpg";

        public static readonly List<string> FoodImagePaths =
        [
            "pack://application:,,,/Resources/Images/Food_1.jpg",
            "pack://application:,,,/Resources/Images/Food_2.jpg",
            "pack://application:,,,/Resources/Images/Food_3.jpg",
            "pack://application:,,,/Resources/Images/Food_4.jpg",
            "pack://application:,,,/Resources/Images/Food_5.jpg"
        ];

    }
}