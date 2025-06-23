
namespace DataAccessLibrary.Data.Database
{
    public interface ILocationData
    {
        Task AddPickupandDropOff(int userId, double pickupLongitude, double pickupLatitude, double dropoffLongitude, double dropoffLatitude);
        Task DeleteUserLocation(int userId);
        Task UpdateDropoffLocation(int userId, double longitude, double latitude);
        Task UpdatePickupLocation(int userId, double longitude, double latitude);
    }
}