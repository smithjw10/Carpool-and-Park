using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class LocationData : ILocationData
    {
        private readonly ISQLDataAccess _db;

        public LocationData(ISQLDataAccess db)
        {
            _db = db;
        }

        // Update Pickup Location
        public async Task UpdatePickupLocation(int userId, double longitude, double latitude)
        {
            string sql = @"UPDATE Locations 
                           SET PickupLongitude = @PickupLongitude, PickupLatitude = @PickupLatitude
                           WHERE UserID = @UserID";
            await _db.SaveData(sql, new { UserID = userId, PickupLongitude = longitude, PickupLatitude = latitude });
        }

        // Update Dropoff Location
        public async Task UpdateDropoffLocation(int userId, double longitude, double latitude)
        {
            string sql = @"UPDATE Locations 
                           SET DropoffLongitude = @DropoffLongitude, DropoffLatitude = @DropoffLatitude
                           WHERE UserID = @UserID";
            await _db.SaveData(sql, new { UserID = userId, DropoffLongitude = longitude, DropoffLatitude = latitude });
        }

        // Add Pickup and DropOff Location
        public async Task AddPickupandDropOff(int userId, double pickupLongitude, double pickupLatitude, double dropoffLongitude, double dropoffLatitude)
        {
            string sql = @"INSERT INTO Locations (UserID, PickupLongitude, PickupLatitude, DropoffLongitude, DropoffLatitude)
                           VALUES (@UserID, @PickupLongitude, @PickupLatitude, @DropoffLongitude, @DropoffLatitude)";
            await _db.SaveData(sql, new
            {
                UserID = userId,
                PickupLongitude = pickupLongitude,
                PickupLatitude = pickupLatitude,
                DropoffLongitude = dropoffLongitude,
                DropoffLatitude = dropoffLatitude
            });
        }

        // Delete Carpool Group
        public async Task DeleteUserLocation(int userId)
        {
            string sql = @"DELETE FROM Locations WHERE UserID = @UserID";
            await _db.SaveData(sql, new { UserID = userId });
        }
    }
}
