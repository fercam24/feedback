using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace GameSessionFeedback.Services
{
    public class ConfigureDbIndexesService : IHostedService
    {
        private readonly IMongoDbContext _dbContext;
        private readonly IFeedbackDatabaseSettings _dbSettings;

        public ConfigureDbIndexesService(IMongoDbContext dbContext, ILogger<ConfigureDbIndexesService> logger, IFeedbackDatabaseSettings dbSettings)
        {
            _dbContext = dbContext;
            _dbSettings = dbSettings;
        }
        
        // Configure task to be executed on startup lifecycle to ensure unique index is set.
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var collection = _dbContext.GetCollection<SessionFeedback>(_dbSettings.SessionFeedbacksCollectionName);
            var indexKeysDef = Builders<SessionFeedback>.IndexKeys.Ascending(feedback => feedback.UserId).Ascending(feedback => feedback.SessionId);
            
            var options = new CreateIndexOptions() { Unique = true };

            await collection.Indexes.CreateOneAsync(new CreateIndexModel<SessionFeedback>(indexKeysDef, options), cancellationToken : cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}