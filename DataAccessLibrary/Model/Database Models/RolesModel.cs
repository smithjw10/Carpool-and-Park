using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class RolesModel
    {
        public string ID { get; set; } = "";
        public string UserName { get; set; } = "";
        public string RoleName { get; set; } = "";
        public string RoleID { get; set; } = "";
        public RolesModel() { }
        public RolesModel(string iD, string userName, string roleName, string roleID)
        {
            this.ID = iD;
            this.UserName = userName;
            this.RoleName = roleName;
            this.RoleID = roleID;
        }
    }
}
