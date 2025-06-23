using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupMemberLocationsData
    {
        Task DeleteGroupMemberLocation(int locationId);
        Task<List<GroupMemberLocationsModel>> GetAllGroupMemberLocations();
        Task<List<GroupMemberLocationsModel>> GetGroupMemberLocation(int tripID);
        Task UpdateGroupMemberLocation(GroupMemberLocationsModel location);
        Task AddGroupMemberLocation(GroupMemberLocationsModel location);
        Task<List<GroupMemberLocationsModel>> GetDriverLocations(int tripID);
    }
}