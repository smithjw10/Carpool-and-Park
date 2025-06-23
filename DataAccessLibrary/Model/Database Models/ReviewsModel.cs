using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class ReviewsModel
    {
        public int ReviewID { get; set; }
        public int ReviewedUserID { get; set; }
        public int ReviewerUserID { get; set; }
        public string ReviewText { get; set; }

        public ReviewsModel() { }

        public ReviewsModel(int reviewId, int reviewedUserId, int reviewerUserId, string reviewText)
        {
            ReviewID = reviewId;
            ReviewedUserID = reviewedUserId;
            ReviewerUserID = reviewerUserId;
            ReviewText = reviewText;
        }

    }

}
