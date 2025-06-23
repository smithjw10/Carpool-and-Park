using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class RolesData : IRolesData
    {
        private readonly ISQLDataAccess _db;

        public RolesData(ISQLDataAccess db)
        {
            _db = db;
        }
        public async Task<List<RolesModel>> GetUserRoles()
        {
            string sql = @"SELECT AspNetUsers.Id AS ID, AspNetUsers.UserName, AspNetRoles.Name AS RoleName, 
                   AspNetRoles.Id AS RoleID
                   FROM AspNetUserRoles
                   JOIN AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id
                   JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id;";
            return await _db.LoadData<RolesModel, dynamic>(sql, new { });
        }

        public async Task<RolesModel> GetUserRole(string username)
        {
            string sql = @"SELECT AspNetUsers.Id AS ID, AspNetUsers.email, AspNetRoles.Name AS RoleName, 
                   AspNetRoles.Id AS RoleID
                   FROM AspNetUserRoles
                   JOIN AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id
                   JOIN AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id
                   WHERE AspNetUsers.email = @Username;";
            var result = await _db.LoadData<RolesModel, dynamic>(sql, new { Username = username });
            return result.FirstOrDefault();
        }

        public async Task AddAdmin(RolesModel user)
        {
            string adminRoleId = await GetAdminID();
            string sql = @"INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);";
            await _db.SaveData(sql, new { UserId = user.ID, RoleId = adminRoleId });
        }
        public async Task<string> GetAdminID()
        {

            string sql = @"SELECT Id FROM AspNetRoles WHERE Name = @Name;";

            var result = await _db.LoadData<string, dynamic>(sql, new { Name = "Admin" });
            return result.FirstOrDefault();
        }
        public async Task DeleteUserRole(RolesModel user)
        {
            Console.WriteLine("In Delete");
            string sql = "DELETE FROM AspNetUserRoles WHERE UserID = @ID;";
            Console.WriteLine(user.ID);
            await _db.SaveData(sql, user);
        }
        public async Task<List<AccountModel>> GetAccounts()
        {
            string sql = "select AspNetUsers.Id, AspNetUsers.UserName from AspNetUsers;";
            return await _db.LoadData<AccountModel, dynamic>(sql, new { });

        }
    }
}
