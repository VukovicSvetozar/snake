using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.ComponentModel;
using Snake.Services;
using Snake.Command;
using Snake.Models;

namespace Snake.Views
{
    public partial class GameWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly GameState _gameState;
        private readonly GameService _gameService;
        private readonly LeaderboardService _leaderboardService;
        private readonly DispatcherTimer _gameTickTimer;
        private readonly DispatcherTimer _countdownTimer;
        private String _currentInformation;
        private bool _isPaused;

        public string ElapsedTime => GameConfig.IsTimedMode
            ? TimeSpan.FromSeconds(_gameService.RemainingTime).ToString(@"mm\:ss")
            : "No time limit";
        public string Information => _currentInformation.ToString();
        public Difficulty Difficulty => _gameService.Difficulty;

        public int CurrentScore => _gameService.CurrentScore;
        public int CurrentLevel => _gameService.CurrentLevel;

        public ICommand StartGameCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LeaderboardCommand { get; }
        public ICommand ExitCommand { get; }

        public GameWindow(GameService gameService, LeaderboardService leaderboardService, GameState gameState)
        {
            InitializeComponent();

            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _gameService.GameEnded += EndGame;

            _leaderboardService = leaderboardService ?? throw new ArgumentNullException(nameof(leaderboardService));

            _gameState = gameState;

            _gameTickTimer = new DispatcherTimer();
            _gameTickTimer.Tick += GameTickTimer_Tick;
            _gameTickTimer.Interval = GameConfig.TimerInterval;

            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);

            _currentInformation = String.Empty;
            _isPaused = false;

            StartGameCommand = new AsyncRelayCommand(async _ => await StartNewGame(), _ => !_gameService.IsRunning);
            SettingsCommand = new RelayCommand(_ => ShowSettings(), _ => !_gameService.IsRunning);
            LeaderboardCommand = new RelayCommand(_ => ShowLeaderboard(), _ => !_gameService.IsRunning);
            ExitCommand = new RelayCommand(_ => ShowExitDialog());

            DataContext = this;
        }

        private async Task StartNewGame()
        {
            await Task.Delay(1000);

            _currentInformation = String.Empty;

            _gameTickTimer.IsEnabled = true;
            _countdownTimer.IsEnabled = true;

            _gameService.IsRunning = true;
            _gameService.InitializeGameGrid(GameArea);
            _gameService.StartNewGame();
            _gameService.DrawGameArea();

            _gameTickTimer.Interval = GameConfig.TimerInterval;

            OnPropertyChanged(nameof(CurrentLevel));
            OnPropertyChanged(nameof(CurrentScore));
            OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(Difficulty));
            OnPropertyChanged(nameof(Information));
        }

        private void ShowSettings()
        {
            SettingWindow settingsWindow = new();
            bool? result = settingsWindow.ShowDialog();

            if (result == true)
            {
                InfoDialog infoDialog = new("✅", "Settings updated!");
                infoDialog.ShowDialog();

                _gameService.Difficulty = GameConfig.Difficulty;
                _gameService.RemainingTime = GameConfig.GameTimeLimit;

                OnPropertyChanged(nameof(ElapsedTime));
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        private void ShowLeaderboard()
        {
            var leaderboardWindow = new LeaderboardWindow(_gameState.Players);
            leaderboardWindow.ShowDialog();
        }

        private static void ShowExitDialog()
        {
            ExitDialog exitDialog = new();
            bool? result = exitDialog.ShowDialog();

            if (result == true)
            {
                Application.Current.Shutdown();
            }
        }

        private void GameTickTimer_Tick(object? sender, EventArgs e)
        {
            _gameService.MoveSnake();

            if (_gameService.CheckCollision())
            {
                EndGame(GameEndReason.Collision);
                return;
            }

            var eatenFood = _gameService.CheckFoodCollision();
            if (eatenFood != null)
            {
                _gameService.EatFood(eatenFood);
                _gameTickTimer.Interval = GameConfig.TimerInterval;
                OnPropertyChanged(nameof(CurrentScore));
                OnPropertyChanged(nameof(CurrentLevel));
            }

            _gameService.DrawGameArea();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (GameConfig.IsTimedMode)
            {
                _gameService.RemainingTime--;
                OnPropertyChanged(nameof(ElapsedTime));

                if (_gameService.RemainingTime <= 0)
                {
                    EndGame(GameEndReason.TimeUp);
                }

            }
        }

        private void EndGame(GameEndReason reason)
        {
            _gameService.IsRunning = false;
            OnPropertyChanged(nameof(Information));
            _gameTickTimer.IsEnabled = false;
            _countdownTimer.IsEnabled = false;
            _gameService.IsRunning = false;

            InfoDialog endDialog = new("🏁", reason.GetMessage());
            endDialog.ShowDialog();

            if (_leaderboardService.IsPlayerEligibleForLeaderboard(_gameService.CurrentScore))
            {
                NameInputDialog inputDialog = new();
                if (inputDialog.ShowDialog() == true)
                {
                    string? playerName = inputDialog.PlayerName;
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        _leaderboardService.AddPlayerToLeaderBoard(playerName, _gameService.CurrentScore);
                    }
                }
            }

            ((AsyncRelayCommand)StartGameCommand).RaiseCanExecuteChanged();
        }

        private void TogglePause()
        {
            if (!_gameService.IsRunning)
            {
                return;
            }
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                _gameTickTimer.IsEnabled = false;
                _countdownTimer.IsEnabled = false;
                _currentInformation = "Pause";
            }
            else
            {
                _gameTickTimer.IsEnabled = true;
                _countdownTimer.IsEnabled = GameConfig.IsTimedMode;
                _currentInformation = String.Empty;
            }

            OnPropertyChanged(nameof(Information));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: _gameService.ChangeDirection(SnakeDirection.Up); break;
                case Key.Down: _gameService.ChangeDirection(SnakeDirection.Down); break;
                case Key.Left: _gameService.ChangeDirection(SnakeDirection.Left); break;
                case Key.Right: _gameService.ChangeDirection(SnakeDirection.Right); break;
                case Key.Space:
                    TogglePause();
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double tileSize = Math.Min(GameArea.ActualWidth / GameConfig.GridSize, GameArea.ActualHeight / GameConfig.GridSize);
            GameConfig.TileSize = (int)Math.Floor(tileSize);
            if (_gameService.GameArea != null)
                _gameService.DrawGameArea();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
                e.Handled = true;
            }
        }

        private void ToggleFullScreen()
        {
            if (this.WindowStyle == WindowStyle.None && this.WindowState == WindowState.Maximized)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Normal;
                this.Left = SystemParameters.WorkArea.Left + (SystemParameters.WorkArea.Width - this.Width) / 2;
                this.Top = SystemParameters.WorkArea.Top + (SystemParameters.WorkArea.Height - this.Height) / 2;
            }
            else
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}