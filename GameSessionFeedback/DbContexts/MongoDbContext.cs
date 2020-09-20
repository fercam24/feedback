using GameSessionFeedback.Models;
using MongoDB.Driver;

namespace GameSessionFeedback.DbContexts
{
    public class MongoDbContext : IMongoDbContext
    {
        public MongoDbContext(IFeedbackDatabaseSettings dbSettings,
            IGameSessionFeedbackProperties gameSessionFeedbackProperties)
        {
            _client = new MongoClient(dbSettings.ConnectionString);
            _database = _client.GetDatabase(gameSessionFeedbackProperties.GameKey + "_" +
                                            gameSessionFeedbackProperties.ServiceName);
        }

        private IMongoDatabase _database { get; }
        private MongoClient _client { get; }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                return null;
            }
            return _database.GetCollection<T>(collectionName);
        }
    }
}