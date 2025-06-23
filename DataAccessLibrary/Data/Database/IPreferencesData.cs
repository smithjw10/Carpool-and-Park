using DataAccessLibrary.Model;

namespace DataAccessLibrary.Data.Database
{
    public interface IPreferencesData
    {
        Task AddUser(int userId);
        Task DeletePreference(int userId);
        Task<List<PreferencesModel>> GetAllPreferences();
        Task<List<PreferencesModel>> GetPreference(int userId);
        Task UpdatePreference(PreferencesModel preference);
    }
}