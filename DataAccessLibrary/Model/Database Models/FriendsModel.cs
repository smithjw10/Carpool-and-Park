using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model.Database_Models
{
    public class FriendsModel
    {
        public int FriendshipId { get; set; }
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public string Status { get; set; } // Consider using an enum for the status
        public DateTime CreatedDate { get; set; }

        public FriendsModel() { }

        public FriendsModel(int friendshipId, int userId1, int userId2, string status, DateTime createdDate)
        {
            FriendshipId = friendshipId;
            UserId1 = userId1;
            UserId2 = userId2;
            Status = status;
            CreatedDate = createdDate;
        }
    }
}
