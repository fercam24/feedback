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
        private readonly IMongoCollection<SessionFeedback> _sessionFeedbacks;

        public FeedbackService(IFeedbackDatabaseSettings dbSettings, IGameSessionFeedbackProperties gameSessionFeedbackProperties)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(gameSessionFeedbackProperties.GameKey + "_" + gameSessionFeedbackProperties.ServiceName);

            _sessionFeedbacks = database.GetCollection<SessionFeedback>(dbSettings.SessionFeedbacksCollectionName);
        }

        public async Task<IEnumerable<SessionFeedback>> GetSessionFeedbacks(int? rating) {
            try {
                var feedbacks = await _sessionFeedbacks.Find(sf => sf.Rate == rating).Limit(15).ToListAsync(); 
                return feedbacks; 
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}