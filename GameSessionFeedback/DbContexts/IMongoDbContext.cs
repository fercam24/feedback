using MongoDB.Driver;

namespace GameSessionFeedback.DbContexts
{
    /// <summary>
    /// Returns a collection. Creates it if doesn't exist
    /// </summary>
    /// <param name="collectionName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>IMongoCollection for the specified type</returns>
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}