using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class SchedulesModel
    {
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public string Day { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }

        public SchedulesModel() { }

        public SchedulesModel(int scheduleId, int userId, string day, DateTime startTime, DateTime endTime)
        {
            ScheduleId = scheduleId;
            UserId = userId;
            Day = day;
            Start = startTime;
            End = endTime;
        }
        public override string ToString()
        {
            // StringBuilder is used for efficient string concatenation
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"ScheduleId: {ScheduleId}");
            stringBuilder.AppendLine($"UserId: {UserId}");

            // For string properties, check if they are null
            stringBuilder.AppendLine($"Day: {Day ?? "N/A"}");
            stringBuilder.AppendLine($"Start: {Start}");
            stringBuilder.AppendLine($"End: {End}");
            stringBuilder.AppendLine($"Text: {Text ?? "N/A"}");

            return stringBuilder.ToString();
        }
    }

}
