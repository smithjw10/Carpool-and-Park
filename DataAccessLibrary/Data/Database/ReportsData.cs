using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using DataAccessLibrary.Model.Logic_Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DataAccessLibrary.Model.ReportsModel;

namespace DataAccessLibrary.Data.Database
{
    public class ReportsData : IReportsData
    {
        private readonly ISQLDataAccess _db;

        public ReportsData(ISQLDataAccess db)
        {
            _db = db;
        }

        // Create a new report
        public async Task CreateNewReport(ReportsModel report)
        {
            string sql = @"INSERT INTO Reports (ReportedUserID, SubmittedUserID, ReportText, ReportReason) 
                           VALUES (@ReportedUserID, @SubmittedUserID, @ReportText, @ReportReason)";
            await _db.SaveData(sql, report);
        }

        // Delete an existing report
        public async Task DeleteReport(ReportsModel report)
        {
            string sql = @"DELETE FROM Reports WHERE ReportID = @ReportID";
            await _db.SaveData(sql, new { report.ReportID });
        }

        // Retrieve all reports text for a given user (ReportedUserID)
        public async Task<List<string>> GetReportsText(int ReportedUserID)
        {
            string sql = @"SELECT ReportText FROM Reports WHERE ReportedUserID = @ReportedUserID";

            var result = await _db.LoadData<string, dynamic>(sql, new { ReportedUserID });

            return result;
        }

        public async Task<List<string>> GetReportsReason(int ReportedUserID)
        {
            string sql = @"SELECT ReportReason FROM Reports WHERE ReportedUserID = @ReportedUserID";

            var result = await _db.LoadData<string, dynamic>(sql, new { ReportedUserID });

            return result;
        }
    }
}
