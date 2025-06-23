using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupMemberLocationsData : IGroupMemberLocationsData
    {
        private readonly ISQLDataAccess _db;

        public GroupMemberLocationsData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<GroupMemberLocationsModel>> GetAllGroupMemberLocations()
        {
            string sql = "SELECT * FROM GroupMemberLocations";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { });
        }

        public async Task<List<GroupMemberLocationsModel>> GetGroupMemberLocation(int tripID)
        {
            string sql = @"SELECT * FROM GroupMemberLocations WHERE  TripID = @TripID";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new {TripID = tripID});
        }

        public async Task UpdateGroupMemberLocation(GroupMemberLocationsModel location)
        {
            string sql = @"UPDATE GroupMemberLocations SET UserId = @UserID, TripID = @TripID, Latitude = @Latitude, Longitude = @Longitude, Timestamp = @Timestamp WHERE LocationID = @LocationId";
            await _db.SaveData(sql,location );
            //new { UserId = location.UserId, GroupId = location.GroupId, Latitude = location.Latitude, Longitude = location.Longitude, TimeStap = location.Timestamp }
        }

        public async Task DeleteGroupMemberLocation(int locationId)
        {
            string sql = "DELETE FROM GroupMemberLocations WHERE LocationID = @LocationId";
            await _db.SaveData(sql, new { LocationId = locationId });
        }
        public async Task AddGroupMemberLocation(GroupMemberLocationsModel location)
        {
            string sql = @"INSERT INTO GroupMemberLocations (UserId, Latitude, Longitude, Timestamp, TripID) 
                   VALUES (@UserID, @Latitude, @Longitude, CURRENT_TIMESTAMP , @TripID);";
            await _db.SaveData(sql, new {UserID = location.UserID, TripID = location.TripID, Latitude = location.Latitude, Longitude = location.Longitude });
        }

        public async Task<List<GroupMemberLocationsModel>> GetDriverLocations(int tripID)
        {
            string sql = @"SELECT GML.UserID, GML.TripID, GML.Latitude, GML.Longitude, GML.Timestamp, GML.LocationID
                            FROM GroupMemberLocations GML
                            WHERE GML.TripID = @TripID;";
            return await _db.LoadData<GroupMemberLocationsModel, dynamic>(sql, new { TripID = tripID });

        }
    }
}
