using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class TripStatisticsData : ITripStatisticsData
    {
        private readonly ISQLDataAccess _db;

        public TripStatisticsData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<TripStatisticsModel>> GetAllTripStatistics()
        {
            string sql = "SELECT * FROM TripStatistics";
            return await _db.LoadData<TripStatisticsModel, dynamic>(sql, new { });
        }

        public async Task<TripStatisticsModel> GetTripStatistic(int statId)
        {
            string sql = "SELECT * FROM TripStatistics WHERE StatID = @StatId";
            var result = await _db.LoadData<TripStatisticsModel, dynamic>(sql, new { StatId = statId });
            return result.FirstOrDefault();
        }

        public async Task UpdateTripStatistic(TripStatisticsModel tripStatistic)
        {
            string sql = @"UPDATE TripStatistics SET UserID = @UserId, MilesDriven = @MilesDriven, MilesRidden = @MilesRidden, FeedbackScore = @FeedbackScore WHERE StatID = @StatId";
            await _db.SaveData(sql, tripStatistic);
        }

        public async Task DeleteTripStatistic(int statId)
        {
            string sql = "DELETE FROM TripStatistics WHERE StatID = @StatId";
            await _db.SaveData(sql, new { StatId = statId });
        }
    }
}
