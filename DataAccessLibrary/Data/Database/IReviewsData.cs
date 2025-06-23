using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IReviewsData
    {
        Task CreateNewReview(ReviewsModel review);
        Task DeleteReview(ReviewsModel review);
        Task<List<string>> GetReviewsText(int ReviewedUserID);

    }
}