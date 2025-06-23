using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupMembersData : IGroupMembersData
    {
        private readonly ISQLDataAccess _db;

        public GroupMembersData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<GroupMembersModel>> GetAllGroupMembers()
        {
            string sql = "SELECT * FROM GroupMembers";
            return await _db.LoadData<GroupMembersModel, dynamic>(sql, new { });
        }

        public async Task<List<GroupMembersModel>> GetGroupMember(int groupId, int userId)
        {
            string sql = @"SELECT * FROM GroupMembers WHERE GroupID = @GroupId AND UserID = @UserId";
            return await _db.LoadData<GroupMembersModel, dynamic>(sql, new { GroupId = groupId, UserId = userId });
        }

        public async Task DeleteGroupMember(int groupId, int userId)
        {
            string sql = @"DELETE FROM GroupMembers WHERE GroupID = @GroupId AND UserID = @UserId";
            await _db.SaveData(sql, new { GroupId = groupId, UserId = userId });
        }

    }
}
