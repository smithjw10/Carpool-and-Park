using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IFriendsData
    {
        Task DeleteFriend(int friendshipId);
        Task<List<FriendsModel>> GetAllFriends();
        Task<List<FriendsModel>> GetFriends(int userId);
        Task<FriendsModel> GetFriend(int friendshipId);
        Task UpdateFriend(FriendsModel friend);
        Task AddFriend(FriendsModel friend);
    }
}