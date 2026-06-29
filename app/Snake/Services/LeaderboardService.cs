using System.IO;
using System.Text.Json;
using Snake.Models;

namespace Snake.Services
{
    public class LeaderboardService
    {
        private readonly GameState _gameState;
        private readonly string filePath = "Resources/Data/leaderboard.json";

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public LeaderboardService(GameState gameState)
        {
            _gameState = gameState;
            gameState.Players = LoadPlayersFromFile();
        }

        public bool IsPlayerEligibleForLeaderboard(int points)
        {
            var sortedPlayers = _gameState.Players.OrderByDescending(p => p.Points).ToList();

            if (_gameState.Players.Count < 8)
            {
                return true;
            }

            if (sortedPlayers.Last().Points < points)
            {
                return true;
            }

            return false;
        }

        public void AddPlayerToLeaderBoard(string name, int points)
        {
            _gameState.Players.Add(new Player { Name = name, Points = points });

            _gameState.Players = _gameState.Players.OrderByDescending(p => p.Points).Take(8).ToList();

            SavePlayersToFile(_gameState.Players);
        }

        private List<Player> LoadPlayersFromFile()
        {
            if (!File.Exists(filePath))
            {
                return [];
            }
            string json = File.ReadAllText(filePath);
            var loadedPlayers = JsonSerializer.Deserialize<List<Player>>(json);

            return [.. loadedPlayers];
        }

        private void SavePlayersToFile(List<Player> players)
        {
            string json = JsonSerializer.Serialize(players, jsonSerializerOptions);
            File.WriteAllText(filePath, json);
        }

    }
}