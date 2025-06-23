using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class TripModel
    {
        public int ID { get; set; }
        public int? DriverID { get; set; }
        public int GroupID { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; } = string.Empty;
        public string UserTripStatus { get; set; } = string.Empty;
        public List<UserInfoModel> ConfirmedUsers { get; set; } = new List<UserInfoModel>();
        public List<UserInfoModel> PendingUsers { get; set; } = new List<UserInfoModel>();
        public List<UserInfoModel> DeclinedUsers { get; set; } = new List<UserInfoModel>();

    }
}
