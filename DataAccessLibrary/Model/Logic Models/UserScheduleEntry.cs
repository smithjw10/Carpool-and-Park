using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class UserScheduleEntry
    {
        public int UserID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Direction { get; set; }
        public UserScheduleEntry() { }

        public UserScheduleEntry(int UserID, string Start, string End, string Direction)
        {
            this.UserID = UserID;
            this.Start = DateTime.Parse(Start);
            this.End = DateTime.Parse(End);
            this.Direction = Direction;
        }
        public UserScheduleEntry(long UserID, DateTime Start, DateTime End, string Direction)
        {
            this.UserID = (int)UserID;
            this.Start = Start;
            this.End = End;
            this.Direction = Direction;
        }

    }
}
