using Dapper;
using Microsoft.Data.Sqlite; // Make sure to add NuGet package Microsoft.Data.Sqlite
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    public class SQLDataAccess : ISQLDataAccess
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SQLDataAccess> _logger;

        public string ConnectionStringName { get; set; } = "DefaultConnection";

        public SQLDataAccess(IConfiguration config, ILogger<SQLDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            return _config.GetConnectionString(ConnectionStringName);
        }

        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            string connectionString = GetConnectionString();

            using (IDbConnection connection = new SqliteConnection(connectionString))
            {
                _logger.LogTrace("Loading data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                try
                {
                    var data = await connection.QueryAsync<T>(sql, parameters);
                    return data.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _logger.LogError(ex, "Error loading data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                    throw;
                }
            }
        }

        public async Task SaveData<T>(string sql, T parameters)
        {
            string connectionString = GetConnectionString();
            using (IDbConnection connection = new SqliteConnection(connectionString))
            {
                _logger.LogTrace("Saving data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                try
                {
                    await connection.ExecuteAsync(sql, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    _logger.LogError(ex, "Error saving data using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                    throw;
                }
            }

        }
        public async Task<int> SaveDataAndGetLastId<T>(string sql, T parameters)
        {
            string connectionString = GetConnectionString();
            using (IDbConnection connection = new SqliteConnection(connectionString))
            {
                _logger.LogTrace("Saving data and retrieving last inserted ID using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                try
                {
                    await connection.ExecuteAsync(sql, parameters);

                    // Retrieve the last inserted ID
                    string getLastIdQuery = "SELECT last_insert_rowid();";
                    int lastId = await connection.ExecuteScalarAsync<int>(getLastIdQuery);

                    return lastId;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _logger.LogError(ex, "Error saving data and retrieving last inserted ID using SQL query: {SqlQuery} with: {parameters}", sql, parameters);
                    throw;
                }
            }
        }
    }
}
