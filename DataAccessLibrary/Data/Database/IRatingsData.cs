using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IRatingsData
    {
        Task CreateNewRating(RatingsModel rating);
        Task DeleteRating(RatingsModel rating);
        Task<double> GetRatings(int RatedUserID);

    }
}