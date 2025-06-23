using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class AccountModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public AccountModel() { }
        public override string ToString()
        {
            return UserName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (!(obj is AccountModel)) return false;

            return this.Id == ((AccountModel)obj).Id;
        }

        public override int GetHashCode()
        {
            // Returns the Id as the hash code for this instance.
            return Id.GetHashCode();
        }
    }
}
