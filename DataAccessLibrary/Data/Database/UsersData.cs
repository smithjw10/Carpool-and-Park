using DataAccessLibrary.Data.API;
using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class UsersData : IUsersData
    {
        private readonly ISQLDataAccess _db;
        private readonly IGMapsAPI _googleAPI;
        private readonly ILocationData _dbLocation;



        public UsersData(ISQLDataAccess db, IGMapsAPI gMapsAPI, ILocationData locationData)
        {
            _db = db;
            _googleAPI = gMapsAPI;
            _dbLocation = locationData;
        }

        public async Task<List<UsersModel>> GetAllUser()
        {
            string sql = @"SELECT * FROM Users";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { });
        }

        public async Task<List<UsersModel>> GetUser(string email)
        {
            string sql = @"SELECT * FROM Users WHERE Users.Email = @Email OR Users.Phone = @Email";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { Email = email });
        }

        public async Task<List<UsersModel>> GetUser(int id)
        {
            string sql = @"SELECT * FROM Users WHERE Users.UserId = @ID";
            return await _db.LoadData<UsersModel, dynamic>(sql, new { ID = id });
        }

        public async Task UpdateUser(UsersModel user)
        {
            string sql = @"UPDATE Users 
                           SET Email = @Email, FirstName = @FirstName, LastName = @LastName, Phone = @Phone, 
                               UserType = @UserType, UserLocation = @UserLocation, PickupLocation = @PickupLocation, 
                               DropoffLocation = @DropoffLocation, DrivingDistance = @DrivingDistance, PhonePrivacy = @PhonePrivacy, 
                               Gender = @Gender, AddressPrivacy = @AddressPrivacy, 
                               BeltCount = @BeltCount, MakeModel = @MakeModel, VehicleColor = @VehicleColor, 
                               LicensePlate = @LicensePlate, AllowEatDrink = @AllowEatDrink, AllowSmokeVape = @AllowSmokeVape, Rating = @Rating
                           WHERE UserId = @UserId";
            (double latitude, double longitude) = await _googleAPI.GetCoordinatesAsync(user.PickupLocation);
            (double dropLat, double dropLon) = await _googleAPI.GetCoordinatesAsync(user.DropoffLocation);
            await _dbLocation.UpdatePickupLocation(user.UserId, longitude, latitude);
            await _dbLocation.UpdateDropoffLocation(user.UserId, dropLon, dropLat);

            await _db.SaveData(sql, user);

        }

        public async Task DeleteUser(int id)
        {
            string sql = @"DELETE FROM Users WHERE UserId = @ID";
            await _db.SaveData(sql, new { ID = id });
        }

        public async Task DeleteAccount(string email)
        {
            string sql = @"DELETE FROM AspNetUsers WHERE Email = @Email;";
            await _db.SaveData(sql, new { Email = email });
        }

        // Add a new user
        public async Task AddUser(UsersModel user)
        {


            string sql = @"INSERT INTO Users (Email, FirstName, LastName, Phone, UserType, UserLocation, 
                                               PickupLocation, DropoffLocation, DrivingDistance, PhonePrivacy, Gender, 
                                               ProfilePicture, AddressPrivacy, BeltCount, MakeModel, VehicleColor, 
                                               LicensePlate, LicensePicture, CarPicture, AllowEatDrink, AllowSmokeVape, Rating) 
                           VALUES (@Email, @FirstName, @LastName, @Phone, @UserType, @UserLocation, 
                                   @PickupLocation, @DropoffLocation, @DrivingDistance, @PhonePrivacy, @Gender, 
                                   @ProfilePicture, @AddressPrivacy, @BeltCount, @MakeModel, @VehicleColor, 
                                   @LicensePlate, @LicensePicture, @CarPicture, @AllowEatDrink, @AllowSmokeVape, @Rating)";
            await _db.SaveData(sql, user);
        }
        public async Task UpdateUserProfilePicture(int userId, byte[] profilePicture)
        {
            string sql = "UPDATE Users SET ProfilePicture = @ProfilePicture WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, ProfilePicture = profilePicture });
        }
        public async Task UpdateUserLicensePicture(int userId, byte[] licensePicture)
        {
            string sql = "UPDATE Users SET LicensePicture = @LicensePic WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, LicensePic = licensePicture });
        }
        public async Task UpdateUserCarPicture(int userId, byte[] carPicture)
        {
            string sql = "UPDATE Users SET CarPicture = @CarPic WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, CarPic = carPicture });
        }

        public async Task UpdateRating(int userId, int rating)
        {
            string sql = "UPDATE Users SET Rating = @Rating WHERE UserId = @UserId";
            await _db.SaveData(sql, new { UserId = userId, Rating = rating });
        }
        public async Task<UserInfoModel> GetUserInfoModel(int GoalUserID)
        {
            string sql = $@"SELECT 
                u.UserID, 
                           u.FirstName, 
                           u.LastName, 
                           u.UserType, 
                           u.Email as 'UserName',
                           u.PickupLocation, 
                           u.DropoffLocation, 
                           u.DrivingDistance, 
                           u.Gender, 
                           u.BeltCount, 
                           u.AllowEatDrink, 
                           u.AllowSmokeVape, 
                           p.GenderPreference, 
                           p.EatingPreference, 
                           p.SmokingPreference, 
                           p.TemperaturePreference, 
                           p.MusicPreference,
                           l.PickupLatitude,
                           l.PickupLongitude, 
                           l.DropoffLongitude, 
                           l.DropoffLatitude, 
                           u.PhonePrivacy, 
                           u.AddressPrivacy, 
                           u.MakeModel, 
                           u.VehicleColor, 
                           u.LicensePlate, 
                           u.LicensePicture, 
                           u.CarPicture,
                           u.ProfilePicture, 
                           u.Rating
                        FROM Users u
                        JOIN Locations l ON u.UserID = l.UserID
                        JOIN Preferences p ON u.UserID = p.UserID
                        WHERE u.UserID = @UserId;";
            List<UserInfoModel> FoundUsers = await _db.LoadData<UserInfoModel, dynamic>(sql, new { UserId = GoalUserID });
            if (!FoundUsers.Any())
            {
                Console.WriteLine("Goal user not found " + GoalUserID);
                var defaultPreference = new PreferencesModel(GoalUserID);
                string sql2 = @"INSERT INTO Preferences (UserID, GenderPreference, EatingPreference, SmokingPreference, TemperaturePreference, MusicPreference) VALUES (@UserId, @GenderPreference, @EatingPreference, @SmokingPreference, @TemperaturePreference, @MusicPreference)";
                await _db.SaveData(sql2, defaultPreference);
                return await GetUserInfoModel(GoalUserID);

            }
            return FoundUsers.FirstOrDefault();

        }
        public async Task<List<UserInfoModel>> GetListUserInfoModel(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
            {
                return new List<UserInfoModel>();
            }

            // Create a parameterized SQL query with an IN clause
            string sql = $@"
                SELECT 
                    u.UserID, 
                    u.FirstName, 
                    u.LastName, 
                    u.UserType, 
                    u.Email as 'UserName',
                    u.PickupLocation,  
                    u.DropoffLocation, 
                    u.DrivingDistance, 
                    u.Gender, 
                    u.BeltCount, 
                    u.AllowEatDrink, 
                    u.AllowSmokeVape, 
                    p.GenderPreference, 
                    p.EatingPreference, 
                    p.SmokingPreference, 
                    p.TemperaturePreference, 
                    p.MusicPreference,
                    l.PickupLatitude,
                    l.PickupLongitude, 
                    l.DropoffLongitude, 
                    l.DropoffLatitude, 
                    u.PhonePrivacy, 
                    u.AddressPrivacy, 
                    u.MakeModel, 
                    u.VehicleColor, 
                    u.LicensePlate, 
                    u.LicensePicture, 
                    u.CarPicture,
                    u.ProfilePicture, 
                    u.Rating
                FROM Users u
                JOIN Locations l ON u.UserID = l.UserID
                JOIN Preferences p ON u.UserID = p.UserID
                WHERE u.UserID IN @UserIds;";

            // Query the database for users matching the given IDs
            List<UserInfoModel> foundUsers = await _db.LoadData<UserInfoModel, dynamic>(sql, new { UserIds = userIds });

            // Return the list of UserInfoModel objects
            return foundUsers;
        }
    }
}