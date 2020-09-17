namespace GameSessionFeedback.Models
{
    public class FeedbackDatabaseSettings : IFeedbackDatabaseSettings {
        public string ConnectionString { get; set; }
        public string SessionFeedbacksCollectionName { get; set; }
        public string GameSessionsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
    }

    public interface IFeedbackDatabaseSettings
    {
         string ConnectionString { get; set; }
         string SessionFeedbacksCollectionName { get; set; }
         string GameSessionsCollectionName { get; set; }
         string UsersCollectionName { get; set; }
    }
}