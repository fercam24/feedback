namespace GameSessionFeedback.Models
{
    public class GameSessionFeedbackProperties : IGameSessionFeedbackProperties
    {
        public string GameKey { get; set; }
        public string ServiceName { get; set; }
    }

    public interface IGameSessionFeedbackProperties
    {
        string GameKey { get; set; }
        string ServiceName { get; set; }
    }
}