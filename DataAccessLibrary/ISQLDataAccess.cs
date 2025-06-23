
namespace DataAccessLibrary
{
    public interface ISQLDataAccess
    {
        string ConnectionStringName { get; set; }

        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
        Task<int> SaveDataAndGetLastId<T>(string sql, T parameters);

    }
}