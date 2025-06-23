using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupRecommendationData
    {
        Task<List<RecomendedGroup>> GetRecommendedGroupsForTimePeriod(Week week);
        Task InsertRecommendedGroups(List<RecomendedGroup> recommendedGroups);
        Task InsertRecommendedGroups(RecomendedGroup recommendedGroup);
        Task<List<RecomendedGroup>> GetUsersUpcomingRecommendations(int userID);
        Task AcceptGroupRec(RecomendedGroup group, int UserID);
        Task DeclineGroupRec(RecomendedGroup group, int UserID);
        Task<List<TripModel>> GetTripModelsForHomePage(int UserID);
        Task ConfirmTripAsRider(TripModel trip, int userID);
        Task ConfirmTripAsDriver(TripModel trip, int userID);
        Task RemoveUserFromTripAsync(int tripId, int userId);
        Task SetUserStatusToDeclineAsync(int tripId, int userId);
        Task SetUserStatusToDeclineAndSetDriverNull(int tripId, int userId);
        Task<TripModel> GetTripWithMems(int TripID);
        Task StartTrip(int TripID); 
        Task EndTrip(int TripID);
        Task DeleteRecommendedGroups(List<RecomendedGroup> groupsToDelete);

    }
}