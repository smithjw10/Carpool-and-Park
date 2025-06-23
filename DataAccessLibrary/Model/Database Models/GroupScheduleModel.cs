using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class GroupScheduleModel
    {
        public int GroupID { get; set; }
        public int GroupScheduleID { get; set; }
        public string Day { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Direction { get; set; } = "";
        public GroupScheduleModel() { }
        public GroupScheduleModel(int groupID, int groupScheduleID, string day, DateTime start, DateTime end, string direction)
        {
            GroupID = groupID;
            GroupScheduleID = groupScheduleID;
            Day = day;
            Start = start;
            End = end;
            Direction = direction;
        }
    }
}
