using System.Windows;
using System.Diagnostics;
using Snake.Models;
using Snake.Services;
using Snake.Views;

namespace Snake
{
    public partial class App : Application
    {
        public static AppSettings Settings { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var gameState = new GameState();
                var gameService = new GameService(gameState);
                var leaderboardService = new LeaderboardService(gameState);
                var gameWindow = new GameWindow(gameService, leaderboardService, gameState);

                gameWindow.Show();
            }
            catch (Exception ex)
            {
                InfoDialog errorDialog = new("⚠", "Application startup error.");
                errorDialog.ShowDialog();
                Debug.WriteLine($"[ERROR] {ex}");
                Shutdown();
            }
        }
    }

    public class AppSettings
    {
    }

}