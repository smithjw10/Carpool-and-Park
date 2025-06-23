using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupMembersData
    {
        Task DeleteGroupMember(int groupId, int userId);
        Task<List<GroupMembersModel>> GetAllGroupMembers();
        Task<List<GroupMembersModel>> GetGroupMember(int groupId, int userId);
    }
}