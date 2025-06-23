using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class CarpoolGroupsModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int DriverId { get; set; }
        public string Destination { get; set; } = "";
        public string DriverName {  get; set; }
        public List<RiderModel> Riders { get; set; }
        public int CreatorID { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLongitude { get; set; }

        public CarpoolGroupsModel() { }

        public CarpoolGroupsModel(int groupId, string groupName, int driverId, string destination, int CreatorId)
        {
            GroupId = groupId;
            GroupName = groupName;
            DriverId = driverId;
            Destination = destination;
            CreatorID = CreatorId;
        }
        public string ListRiderNames()
        {
            if(Riders.Count == 0)
            {
                return "";
            }
            return String.Join(", ", Riders.Select(p => p.Name));
        }

        public class RiderModel
        {
            public int Id { get; set; } = 0;
            public string Name { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public RiderModel() { }
        }
    }

}
