using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IRolesData
    {
        Task AddAdmin(RolesModel user);
        Task DeleteUserRole(RolesModel user);
        Task<string> GetAdminID();
        Task<RolesModel> GetUserRole(string username);
        Task<List<RolesModel>> GetUserRoles();
        Task<List<AccountModel>> GetAccounts();

    }
}