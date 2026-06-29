using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Snake.Models;
using Snake.Services;

namespace Snake.Views
{
    public partial class SettingWindow : Window, INotifyPropertyChanged
    {
        private int _snakeStartLength;
        private int _snakeStartSpeed;
        private int _maxFoodCount;
        private bool _isTimedMode;
        private int _gameTimeLimit;
        private bool _areObstaclesEnabled;
        private int _obstacleCount;
        private Difficulty _difficulty;

        public int SnakeStartLength
        {
            get => _snakeStartLength;
            set => UpdateAndNotify(ref _snakeStartLength, value);
        }

        public int SnakeStartSpeed
        {
            get => _snakeStartSpeed;
            set => UpdateAndNotify(ref _snakeStartSpeed, value);
        }

        public int MaxFoodCount
        {
            get => _maxFoodCount;
            set => UpdateAndNotify(ref _maxFoodCount, value);
        }

        public bool IsTimedMode
        {
            get => _isTimedMode;
            set => UpdateAndNotify(ref _isTimedMode, value);
        }

        public int GameTimeLimit
        {
            get => _gameTimeLimit;
            set => UpdateAndNotify(ref _gameTimeLimit, value);
        }

        public bool AreObstaclesEnabled
        {
            get => _areObstaclesEnabled;
            set => UpdateAndNotify(ref _areObstaclesEnabled, value);
        }

        public int ObstacleCount
        {
            get => _obstacleCount;
            set => UpdateAndNotify(ref _obstacleCount, value);
        }

        public Difficulty Difficulty
        {
            get => _difficulty;
            private set => SetField(ref _difficulty, value);
        }

        public SettingWindow()
        {
            InitializeComponent();

            DataContext = this;

            SnakeStartLength = GameConfig.SnakeStartLength;
            SnakeStartSpeed = MapSpeedToSliderValue(GameConfig.SnakeStartSpeed);
            MaxFoodCount = GameConfig.MaxFoodCount;
            IsTimedMode = GameConfig.IsTimedMode;
            GameTimeLimit = GameConfig.GameTimeLimit;
            AreObstaclesEnabled = GameConfig.AreObstaclesEnabled;
            ObstacleCount = GameConfig.ObstacleCount;
            Difficulty = GameConfig.Difficulty;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            GameConfig.SnakeStartLength = SnakeStartLength;
            GameConfig.SnakeStartSpeed = MapSliderValueToSpeed(SnakeStartSpeed);
            GameConfig.MaxFoodCount = MaxFoodCount;
            GameConfig.IsTimedMode = IsTimedMode;
            GameConfig.GameTimeLimit = GameTimeLimit;
            GameConfig.AreObstaclesEnabled = AreObstaclesEnabled;
            GameConfig.ObstacleCount = ObstacleCount;
            GameConfig.Difficulty = Difficulty;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static int MapSliderValueToSpeed(int sliderValue) => 800 - (sliderValue * 200);
        private static int MapSpeedToSliderValue(int speed) => (800 - speed) / 200;

        private void UpdateDifficulty()
        {
            int totalPoints = 0;

            switch (SnakeStartLength)
            {
                case 3: totalPoints += 1; break;
                case 4: totalPoints += 2; break;
                case 5: totalPoints += 3; break;
                case 6: totalPoints += 4; break;
                case 7: totalPoints += 5; break;
            }

            switch (SnakeStartSpeed)
            {
                case 1: totalPoints += 0; break;
                case 2: totalPoints += 15; break;
                case 3: totalPoints += 30; break;
            }

            switch (MaxFoodCount)
            {
                case 1: totalPoints += 10; break;
                case 2: totalPoints += 8; break;
                case 3: totalPoints += 6; break;
                case 4: totalPoints += 4; break;
                case 5: totalPoints += 2; break;
            }

            if (IsTimedMode)
            {
                switch (GameTimeLimit)
                {
                    case 120: totalPoints += 25; break;
                    case 240: totalPoints += 20; break;
                    case 360: totalPoints += 15; break;
                    case 480: totalPoints += 10; break;
                    case 600: totalPoints += 5; break;
                }
            }

            if (AreObstaclesEnabled)
            {
                switch (ObstacleCount)
                {
                    case 5: totalPoints += 5; break;
                    case 6: totalPoints += 10; break;
                    case 7: totalPoints += 15; break;
                    case 8: totalPoints += 20; break;
                    case 9: totalPoints += 25; break;
                    case 10: totalPoints += 30; break;
                }
            }

            if (totalPoints <= 33)
            {
                Difficulty = Difficulty.Easy;
            }
            else if (totalPoints > 33 && totalPoints <= 66)
            {
                Difficulty = Difficulty.Normal;
            }
            else
            {
                Difficulty = Difficulty.Hard;
            }
        }

        private bool UpdateAndNotify<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (SetField(ref field, value, propertyName))
            {
                UpdateDifficulty();
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}