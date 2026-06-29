using System.Windows;

namespace Snake.Models
{
    public class Food
    {
        public Point Position { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}