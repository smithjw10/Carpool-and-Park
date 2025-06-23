using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IReportsData
    {
        Task CreateNewReport(ReportsModel report); 
        Task DeleteReport(ReportsModel report);
        Task<List<string>> GetReportsText(int ReportedUserID);
        Task<List<string>> GetReportsReason(int ReportedUserID);

    }
}