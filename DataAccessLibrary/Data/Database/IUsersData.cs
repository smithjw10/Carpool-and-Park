using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IUsersData
    {
        Task AddUser(UsersModel user);
        Task DeleteAccount(string email);
        Task DeleteUser(int id);
        Task<List<UsersModel>> GetAllUser();
        Task<List<UsersModel>> GetUser(int id);
        Task<List<UsersModel>> GetUser(string email);
        Task<UserInfoModel> GetUserInfoModel(int GoalUserID);
        Task UpdateRating(int userId, int rating);
        Task UpdateUser(UsersModel user);
        Task UpdateUserCarPicture(int userId, byte[] carPicture);
        Task UpdateUserLicensePicture(int userId, byte[] licensePicture);
        Task UpdateUserProfilePicture(int userId, byte[] profilePicture);
        Task<List<UserInfoModel>> GetListUserInfoModel(List<int> userIds);
    }
}