using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.RatingsModel;

namespace DataAccessLibrary.Data.Database
{
    public class RatingsData : IRatingsData
    {
        private readonly ISQLDataAccess _db;

        public RatingsData(ISQLDataAccess db)
        {
            _db = db;
        }

        // Create a new rating
        public async Task CreateNewRating(RatingsModel rating)
        {
            string sql = @"INSERT INTO Ratings (RatedUserID, Rating) 
                           VALUES (@RatedUserID, @Rating)";
            await _db.SaveData(sql, rating);
        }

        // Delete an existing rating
        public async Task DeleteRating(RatingsModel rating)
        {
            string sql = @"DELETE FROM Ratings WHERE RatingID = @RatingID";
            await _db.SaveData(sql, new { rating.RatingID });
        }

        // Retrieve all ratings for a given user (RatedUserID)
        public async Task<double> GetRatings(int RatedUserID)
        {
            string sql = @"SELECT Rating FROM Ratings WHERE RatedUserID = @RatedUserID";

            var ratings = await _db.LoadData<int, dynamic>(sql, new { RatedUserID });

            return ratings.Any() ? ratings.Average() : 0.0;
        }

    }
}
