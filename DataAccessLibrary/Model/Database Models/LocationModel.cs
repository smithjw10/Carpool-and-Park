using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class LocationModel
    {
        public int UserID {  get; set; }
        public double PickupLongitude { get; set; }
        public double PickupLatitude { get; set; }
        public double DropoffLongitude { get; set; }
        public double DropoffLatitude { get; set; }
        public LocationModel() { }
        public LocationModel(int userID, double pickupLongitude, double pickupLatitude, double dropoffLongitude, double dropoffLatitude)
        {
            UserID = userID;
            PickupLongitude = pickupLongitude;
            PickupLatitude = pickupLatitude;
            DropoffLongitude = dropoffLongitude;
            DropoffLatitude = dropoffLatitude;
        }
    }
}
