using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Logic_Models
{
    public class Week
    {
        public DateTime StartDate { get; set; } // Monday
        public DateTime EndDate { get; set; }   // Friday
        public string DisplayString { get; set; }
    }
}
