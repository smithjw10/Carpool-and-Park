using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.ReviewsModel;

namespace DataAccessLibrary.Data.Database
{
    public class ReviewsData : IReviewsData
    {
        private readonly ISQLDataAccess _db;

        public ReviewsData(ISQLDataAccess db)
        {
            _db = db;
        }

        // Create a new review
        public async Task CreateNewReview(ReviewsModel review)
        {
            string sql = @"INSERT INTO Reviews (ReviewedUserID, ReviewerUserID, ReviewText) 
                           VALUES (@ReviewedUserID, @ReviewerUserID, @ReviewText)";
            await _db.SaveData(sql, review);
        }

        // Delete an existing review
        public async Task DeleteReview(ReviewsModel review)
        {
            string sql = @"DELETE FROM Reviews WHERE ReviewID = @ReviewID";
            await _db.SaveData(sql, new { review.ReviewID });
        }

        // Retrieve all reviews text for a given user (ReviewedUserID)
        public async Task<List<string>> GetReviewsText(int ReviewedUserID)
        {
            string sql = @"SELECT ReviewText FROM Reviews WHERE ReviewedUserID = @ReviewedUserID";

            var result = await _db.LoadData<string, dynamic>(sql, new { ReviewedUserID });

            return result;
        }

    }
}
