using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;

namespace DataAccessLibrary.Data.Database
{
    public interface IGroupScheduleData
    {
        Task AddSchedule(SchedulesModel schedule, int groupID);
        Task DeleteSchedule(int scheduleId);
        Task<List<GroupScheduleModel>> GetScheduleForGroup(int givenGroup);
        Task UpdateSchedule(SchedulesModel schedule);
    }
}