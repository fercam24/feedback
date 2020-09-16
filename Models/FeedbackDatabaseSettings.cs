namespace feedback.Models
{
    public class FeedbackDatabaseSettings : IFeedbackDatabaseSettings {
        public string ConnectionString { get; set; }
    }

    public interface IFeedbackDatabaseSettings
    {
         string ConnectionString { get; set; }
    }
}