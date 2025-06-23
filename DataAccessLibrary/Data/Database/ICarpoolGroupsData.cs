using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface ICarpoolGroupsData
    {
        Task CreateNewGroup(CarpoolGroupsModel carpoolGroup);
        Task DeleteCarpoolGroup(CarpoolGroupsModel group);
        List<UserInfoModel> GenerateTestUserInfoModels(int ListSize);
        Task<List<CarpoolGroupsModel>> GetAllCarpoolGroups();
        Task<List<RecomendedGroup>> GetAvailableGroups(int GoalUserID, List<string> Days);
        Task<List<CarpoolGroupsModel>> GetCarpoolGroup(int groupId);
        Task<List<(int, string, int, int)>> GetCurrentGroups(int GoalUserID);
        Task<List<CarpoolGroupsModel>> GetDriverGroups(int driverId);
        Task<int> GetGroupNumber(string GroupName, int CreatorID);
        Task<List<RecomendedGroup>> GetRecomendedGroups(int GoalUserID, string Direction);
        Task<List<RecomendedGroup>> GetRecommendGroups(int GoalUserID, List<string> Days, string TravelDirection, int DistanceWeight, int PreferenceWeight);
        Task<List<CarpoolGroupsModel>> GetRiderGroups(int userID);
        Task<List<CarpoolGroupsModel.RiderModel>> GetRiders(int groupId, int driverID);
        Task<RecomendedGroup> GetSingleGroup(int GroupID, int RequestingUserID);
        Task<List<int>> GetUserIds(int GroupID);
        Task JoinGroup(int GoalUserID, int GoalGroupID);
        Task RemoveGroupMember(int GoalUserID, int GoalGroupID);
        Task UpdateCarpoolGroup(CarpoolGroupsModel group);
    }
}