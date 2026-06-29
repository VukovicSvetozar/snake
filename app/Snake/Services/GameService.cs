using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Snake.Models;

namespace Snake.Services
{
    public class GameService
    {

        public Grid? GameArea;

        public event Action<GameEndReason>? GameEnded;

        public bool IsRunning { get; set; }
        public Difficulty Difficulty { get; set; }
        public int CurrentScore { get; private set; }
        public int CurrentLevel { get; private set; }

        public SnakeDirection CurrentDirection { get; private set; }
        public int SnakeLength { get; private set; }

        private readonly GameState _gameState;
        private readonly Random _random;
        private int _foodEatenInCurrentLevel;

        private int FreeSpaces
        {
            get
            {
                var occupiedPositions = GetOccupiedPositions();
                return GameConfig.GridSize * GameConfig.GridSize - occupiedPositions.Count;
            }
        }

        private int _remainingTime;
        public int RemainingTime
        {
            get => _remainingTime;
            set
            {
                if (_remainingTime != value)
                {
                    _remainingTime = value;
                }
            }
        }

        public GameService(GameState gameState)
        {
            Difficulty = GameConfig.Difficulty;
            _gameState = gameState;
            _random = new();

            if (GameConfig.IsTimedMode)
            {
                ResetTimer();
            }
        }

        public void InitializeGameGrid(Grid gameArea)
        {
            GameArea = gameArea;
            GameArea.Children.Clear();
            GameArea.RowDefinitions.Clear();
            GameArea.ColumnDefinitions.Clear();

            for (int i = 0; i < GameConfig.GridSize; i++)
            {
                GameArea.RowDefinitions.Add(new RowDefinition());
                GameArea.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        public void DrawGameArea()
        {
            GameArea?.Children.Clear();

            DrawElements(GameArea!, _gameState.SnakeParts, CreateSnakePartElement);
            DrawElements(GameArea!, _gameState.FoodItems, CreateFoodElement);
            DrawElements(GameArea!, _gameState.Obstacles, CreateObstacleElement);
        }

        private static void DrawElements<T>(Grid gameArea, IEnumerable<T> items, Func<T, FrameworkElement> createElement)
        {
            foreach (var item in items)
            {
                var element = createElement(item);
                if (item is SnakePart part)
                {
                    AddElementToGrid(gameArea, element, (int)part.Position.Y, (int)part.Position.X);
                }
                else if (item is Food food)
                {
                    AddElementToGrid(gameArea, element, (int)food.Position.Y, (int)food.Position.X);
                }
                else if (item is Obstacle obstacle)
                    AddElementToGrid(gameArea, element, (int)obstacle.Position.Y, (int)obstacle.Position.X);
            }
        }

        private static void AddElementToGrid(Grid gameArea, UIElement element, int row, int column)
        {
            Grid.SetRow(element, row);
            Grid.SetColumn(element, column);
            gameArea.Children.Add(element);
        }

        private FrameworkElement CreateSnakePartElement(SnakePart part)
        {
            return part.IsHead
                ? new Image
                {
                    Source = new BitmapImage(new Uri(_gameState.HeadImagePaths[CurrentDirection]))
                }
                : new Image
                {
                    Source = new BitmapImage(new Uri(GameConfig.SnakeBodyImage))
                };
        }

        private FrameworkElement CreateFoodElement(Food food)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(food.ImagePath))
            };
        }

        private FrameworkElement CreateObstacleElement(Obstacle obstacle)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(obstacle.ImagePath))
            };
        }

        public void StartNewGame()
        {
            _gameState.SnakeParts.Clear();
            _gameState.FoodItems.Clear();
            _gameState.Obstacles.Clear();

            IsRunning = true;
            CurrentScore = 0;
            CurrentLevel = GameConfig.InitialLevel;
            CurrentDirection = SnakeDirection.Right;
            SnakeLength = GameConfig.SnakeStartLength;
            _foodEatenInCurrentLevel = 0;
            GameConfig.TimerInterval = TimeSpan.FromMilliseconds(GameConfig.SnakeStartSpeed);

            if (GameConfig.IsTimedMode)
            {
                ResetTimer();
            }

            int startX = 10;
            int startY = 10;
            _gameState.SnakeParts.Add(new SnakePart { Position = new Point(startX, startY), IsHead = true });

            for (int i = 0; i < GameConfig.MaxFoodCount; i++)
            {
                _gameState.FoodItems.Add(new Food
                {
                    Position = GetNextFoodPosition(),
                    ImagePath = GetRandomFoodImage()
                });
            }

            if (GameConfig.AreObstaclesEnabled)
            {
                GenerateObstacles();
            }

        }

        public void GenerateObstacles()
        {
            _gameState.Obstacles.Clear();

            for (int i = 0; i < GameConfig.ObstacleCount; i++)
            {
                _gameState.Obstacles.Add(new Obstacle
                {
                    Position = GetNextObstaclePosition(),
                    ImagePath = GameConfig.ObstacleImage
                });
            }
        }

        private Point GetNextFoodPosition()
        {
            var occupiedPositions = GetOccupiedPositions();

            Point position;
            do
            {
                position = new Point(_random.Next(0, GameConfig.GridSize), _random.Next(0, GameConfig.GridSize));
            }
            while (occupiedPositions.Contains(position));

            return position;
        }

        private string GetRandomFoodImage()
        {
            int index = _random.Next(GameConfig.FoodImagePaths.Count);
            return GameConfig.FoodImagePaths[index];
        }

        private Point GetNextObstaclePosition()
        {
            Point position;
            do
            {
                position = new Point(_random.Next(0, GameConfig.GridSize), _random.Next(0, GameConfig.GridSize));
            }
            while (_gameState.SnakeParts.Exists(part => part.Position == position) ||
                   _gameState.FoodItems.Exists(food => food.Position == position) ||
                   _gameState.Obstacles.Exists(obstacle => obstacle.Position == position));

            return position;
        }

        public void ChangeDirection(SnakeDirection newDirection)
        {
            if (newDirection == SnakeDirection.Left && CurrentDirection != SnakeDirection.Right ||
               newDirection == SnakeDirection.Right && CurrentDirection != SnakeDirection.Left ||
               newDirection == SnakeDirection.Up && CurrentDirection != SnakeDirection.Down ||
               newDirection == SnakeDirection.Down && CurrentDirection != SnakeDirection.Up)
            {
                CurrentDirection = newDirection;
            }
        }

        public void MoveSnake()
        {
            while (_gameState.SnakeParts.Count >= SnakeLength)
            {
                _gameState.SnakeParts.RemoveAt(0);
            }

            foreach (var snakePart in _gameState.SnakeParts)
            {
                snakePart.IsHead = false;
            }

            var head = _gameState.SnakeParts[^1];
            int nextX = (int)head.Position.X;
            int nextY = (int)head.Position.Y;

            switch (CurrentDirection)
            {
                case SnakeDirection.Left: nextX--; break;
                case SnakeDirection.Right: nextX++; break;
                case SnakeDirection.Up: nextY--; break;
                case SnakeDirection.Down: nextY++; break;
            }

            _gameState.SnakeParts.Add(new SnakePart
            {
                Position = new Point(nextX, nextY),
                IsHead = true
            });
        }

        public bool CheckCollision()
        {
            var head = _gameState.SnakeParts[^1];

            if (head.Position.X < 0 || head.Position.Y < 0 ||
                head.Position.X >= GameConfig.GridSize || head.Position.Y >= GameConfig.GridSize)
            {
                return true;
            }

            for (int i = 0; i < _gameState.SnakeParts.Count - 1; i++)
            {
                if (_gameState.SnakeParts[i].Position == head.Position)
                {
                    return true;
                }
            }

            foreach (var obstacle in _gameState.Obstacles)
            {
                if (obstacle.Position == head.Position)
                {
                    return true;
                }
            }

            return false;
        }

        public Food? CheckFoodCollision()
        {
            var head = _gameState.SnakeParts[^1];
            foreach (var food in _gameState.FoodItems)
            {
                if (head.Position == food.Position)
                {
                    return food;
                }
            }
            return null;
        }

        public void EatFood(Food eatenFood)
        {
            SnakeLength++;
            CurrentScore += CalculateScore();
            UpdateFoodPositions(eatenFood);

            _foodEatenInCurrentLevel++;

            if (_foodEatenInCurrentLevel >= GameConfig.FoodPerLevel)
            {
                IncreaseLevel();
                _foodEatenInCurrentLevel = 0;
            }
        }

        private int CalculateScore()
        {
            int baseScore = 10;
            int difficultyMultiplier = GameConfig.Difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Normal => 2,
                Difficulty.Hard => 3,
                _ => 1
            };
            int timedModeBonus = GameConfig.IsTimedMode ? 2 : 1;

            return baseScore * difficultyMultiplier * CurrentLevel * timedModeBonus;
        }

        public void IncreaseLevel()
        {
            CurrentLevel++;
            int newSpeed = GameConfig.SnakeStartSpeed - (CurrentLevel - 1) * 40;
            GameConfig.TimerInterval = TimeSpan.FromMilliseconds(Math.Max(newSpeed, GameConfig.SnakeSpeedThreshold));
        }

        private void UpdateFoodPositions(Food eatenFood)
        {
            _gameState.FoodItems.Remove(eatenFood);

            if (FreeSpaces > 0)
            {
                _gameState.FoodItems.Add(new Food
                {
                    Position = GetNextFoodPosition(),
                    ImagePath = GetRandomFoodImage()
                });
            }
            else
            {
                GameEnded?.Invoke(GameEndReason.Victory);
            }
        }

        private HashSet<Point> GetOccupiedPositions()
        {
            return new HashSet<Point>(
                _gameState.SnakeParts.Select(part => part.Position)
                .Concat(_gameState.FoodItems.Select(food => food.Position))
                .Concat(_gameState.Obstacles.Select(obstacle => obstacle.Position))
            );
        }

        private void ResetTimer()
        {
            if (GameConfig.IsTimedMode)
            {
                _remainingTime = GameConfig.GameTimeLimit;
            }
        }

    }
}