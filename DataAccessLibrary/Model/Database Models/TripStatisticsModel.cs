using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class TripStatisticsModel
    {
        public int StatId { get; set; }
        public int UserId { get; set; }
        public float MilesDriven { get; set; }
        public float MilesRidden { get; set; }
        public float FeedbackScore { get; set; }

        public TripStatisticsModel() { }

        public TripStatisticsModel(int statId, int userId, float milesDriven, float milesRidden, float feedbackScore)
        {
            StatId = statId;
            UserId = userId;
            MilesDriven = milesDriven;
            MilesRidden = milesRidden;
            FeedbackScore = feedbackScore;
        }
    }

}
