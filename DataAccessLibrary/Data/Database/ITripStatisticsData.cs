using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface ITripStatisticsData
    {
        Task DeleteTripStatistic(int statId);
        Task<List<TripStatisticsModel>> GetAllTripStatistics();
        Task<TripStatisticsModel> GetTripStatistic(int statId);
        Task UpdateTripStatistic(TripStatisticsModel tripStatistic);
    }
}