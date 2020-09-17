using MongoDB.Driver;

namespace GameSessionFeedback.DbContexts
{
    public interface IMongoDbContext
    {
         IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}