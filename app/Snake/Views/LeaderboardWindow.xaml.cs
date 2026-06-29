using System.Windows;
using System.Windows.Controls;
using Snake.Models;

namespace Snake.Views
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow(List<Player> players)
        {
            InitializeComponent();

            var rankedPlayers = players
              .OrderByDescending(p => p.Points)
              .Select((player, index) => new
              {
                  Rank = index + 1,
                  player.Name,
                  player.Points
              })
              .ToList();

            DataContext = new { Players = rankedPlayers };
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double totalWidth = LeaderboardListView.ActualWidth - 8;
            if (totalWidth > 0)
            {
                ((GridView)LeaderboardListView.View).Columns[0].Width = totalWidth * 0.15;
                ((GridView)LeaderboardListView.View).Columns[1].Width = totalWidth * 0.70;
                ((GridView)LeaderboardListView.View).Columns[2].Width = totalWidth * 0.15;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

}