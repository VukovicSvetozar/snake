using System.Windows;

namespace Snake.Models
{
    public class Obstacle
    {
        public Point Position { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}