using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class DirectionsStop
    {
        public int Id { get; set; } = -1;
        public string Address { get; set; } = "";
        public DirectionsStop(int id, string add)
        {
            Id = id;
            Address = add;
        }
        public DirectionsStop() { }
    }
}
