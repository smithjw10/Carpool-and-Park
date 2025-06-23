using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Logic_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface ISchedulesData
    {
        Task AddSchedule(SchedulesModel schedule);
        Task DeleteSchedule(int scheduleId);
        Task<List<SchedulesModel>> GetAllSchedules();
        Task<List<SchedulesModel>> GetSchedule(int userID);
        Task UpdateSchedule(SchedulesModel schedule);
        Task<List<UserScheduleEntry>> GetScheduleEntries(DateTime startDateTime, DateTime endDateTime);
    }
}