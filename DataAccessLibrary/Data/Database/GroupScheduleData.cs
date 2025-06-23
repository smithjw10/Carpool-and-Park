using DataAccessLibrary.Model;
using DataAccessLibrary.Model.Database_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccessLibrary.Data.Database
{
    public class GroupScheduleData : IGroupScheduleData
    {
        private readonly ISQLDataAccess _db;

        public GroupScheduleData(ISQLDataAccess db)
        {
            _db = db;
        }

        public async Task<List<GroupScheduleModel>> GetScheduleForGroup(int givenGroup)
        {
            string sql = "SELECT * FROM GroupSchedule where GroupID = @groupID";
            return await _db.LoadData<GroupScheduleModel, dynamic>(sql, new { groupID = givenGroup });

        }
        public async Task AddSchedule(SchedulesModel schedule, int groupID)
        {
            string sql = @"INSERT INTO GroupSchedule (GroupID, Day, Start, End, Direction) 
                   VALUES (@GroupID, @Day, @Start, @End, @Direction)";
            await _db.SaveData(sql, new { Day = schedule.Day, Start = schedule.Start, End = schedule.End, Direction = schedule.Text, GroupID = groupID });
        }
        public async Task DeleteSchedule(int scheduleId)
        {
            string sql = "DELETE FROM GroupSchedule WHERE GroupScheduleID = @ScheduleId";
            await _db.SaveData(sql, new { GroupScheduleID = scheduleId });
        }
        public async Task UpdateSchedule(SchedulesModel schedule)
        {
            string sql = @"UPDATE GroupSchedule SET GroupID = @GroupID, Day = @Day, Start = @Start, End = @End, Direction = @Direction WHERE GroupScheduleID = @GroupScheduleId";
            await _db.SaveData(sql, new { Day = schedule.Day, Start = schedule.Start, End = schedule.End, Direction = schedule.Text, GroupScheduleID = schedule.ScheduleId });
        }
    }
}
