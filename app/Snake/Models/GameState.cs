using Snake.Services;

namespace Snake.Models
{
    public class GameState
    {
        public List<SnakePart> SnakeParts { get; set; } = [];

        public List<Food> FoodItems { get; set; } = [];

        public List<Obstacle> Obstacles { get; set; } = [];

        public List<Player> Players { get; set; } = [];

        public readonly Dictionary<SnakeDirection, string> HeadImagePaths = new()
        {
                { SnakeDirection.Up, GameConfig.SnakeHeadImageUp },
                { SnakeDirection.Down, GameConfig.SnakeHeadImageDown },
                { SnakeDirection.Left, GameConfig.SnakeHeadImageLeft },
                { SnakeDirection.Right, GameConfig.SnakeHeadImageRight }
        };

    }
}