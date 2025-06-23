using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class TimeWindow
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Direction { get; set; }
        public TimeWindow(DateTime start, DateTime end, string direction)
        {
            Start = start;
            End = end;
            Direction = direction;
        }
    }
}
