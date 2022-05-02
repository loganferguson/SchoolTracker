using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TrackerApp.Models
{
    class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public int CourseId { get; set; }
        public string AssessmentName { get; set; }
        public DateTime AssessmentStart { get; set; }
        public DateTime AssessmentEnd { get; set; }

        public string AssessmentType { get; set; }
        public bool StartNotificationsOn { get; set; }
        public bool EndNotificationsOn { get; set; }
    }
}
