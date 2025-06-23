using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class RatingsModel
    {
        public int RatingID { get; set; }
        public int RatedUserID { get; set; }
        public double Rating { get; set; }

        public RatingsModel() { }

        public RatingsModel(int ratingId, int ratedUserId, double rating)
        {
            RatingID = ratingId;
            RatedUserID = ratedUserId;
            Rating = rating;
        }

    }

}
