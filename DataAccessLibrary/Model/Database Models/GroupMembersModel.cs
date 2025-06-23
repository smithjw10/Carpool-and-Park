using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class GroupMembersModel
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinDate { get; set; }

        public GroupMembersModel() { }

        public GroupMembersModel(int groupId, int userId, DateTime joinDate)
        {
            GroupId = groupId;
            UserId = userId;
            JoinDate = joinDate;
        }
    }

}
