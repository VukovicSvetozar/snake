namespace Snake.Models
{
    public enum SnakeDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public enum GameEndReason
    {
        Collision,
        TimeUp,
        Victory
    }

    public static class GameEndReasonExtensions
    {
        public static string GetMessage(this GameEndReason reason)
        {
            return reason switch
            {
                GameEndReason.Collision => "Game over!\nYou collided with a wall or yourself.",
                GameEndReason.TimeUp => " Game over!\nTime's up.",
                GameEndReason.Victory => "Congratulations! You won the game!",
                _ => "Game over!"
            };
        }
    }

}