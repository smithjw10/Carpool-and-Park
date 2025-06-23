using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Model
{
    public class ReportsModel
    {
        public int ReportID { get; set; }
        public int ReportedUserID { get; set; }
        public int SubmittedUserID { get; set; }
        public string ReportText { get; set; }
        public string ReportReason { get; set; }

        public ReportsModel() { }

        public ReportsModel(int reportId, int reportedUserId, int submittedUserId, string reportText, string reportReason)
        {
            ReportID = reportId;
            ReportedUserID = reportedUserId;
            SubmittedUserID = submittedUserId;
            ReportText = reportText;
            ReportReason = reportReason;
        }
        
    }

}
