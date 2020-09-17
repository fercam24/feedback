using GameSessionFeedback.DbContexts;
using GameSessionFeedback.Models;
using Microsoft.Win32.SafeHandles;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace GameSessionFeedback.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IMongoDbContext _dbContext;
        private IMongoCollection<SessionFeedback> _sessionFeedbacks;

        public FeedbackService(IMongoDbContext dbContext, IFeedbackDatabaseSettings dbSettings)
        {
            _dbContext = dbContext;
            _sessionFeedbacks = _dbContext.GetCollection<SessionFeedback>(dbSettings.SessionFeedbacksCollectionName);
        }

        public async Task<IEnumerable<SessionFeedback>> GetSessionFeedbacks(int? rating) {
            try {
                var builder = Builders<SessionFeedback>.Filter;
                var filter = builder.Empty;
                if (rating != null) {
                    filter = builder.Eq(sf => sf.Rate, rating);
                }
                
                var feedbacks = await _sessionFeedbacks.Find(filter).Limit(15).ToListAsync(); 
                return feedbacks; 
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}