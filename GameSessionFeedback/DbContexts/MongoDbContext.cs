using GameSessionFeedback.Models;
using MongoDB.Driver;

namespace GameSessionFeedback.DbContexts
{
    public class MongoDbContext : IMongoDbContext
    {
        private IMongoDatabase _database { get; set; }
        private MongoClient _client { get; set; }
    
        public MongoDbContext(IFeedbackDatabaseSettings dbSettings, IGameSessionFeedbackProperties gameSessionFeedbackProperties)
        {
            _client = new MongoClient(dbSettings.ConnectionString);
            _database = _client.GetDatabase(gameSessionFeedbackProperties.GameKey + "_" + gameSessionFeedbackProperties.ServiceName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName) {
            return _database.GetCollection<T>(collectionName);
        }
    }
}