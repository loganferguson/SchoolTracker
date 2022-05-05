using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using TrackerApp.Models;

namespace TrackerApp.Models
{
    class Course
    {

        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }

        public int TermId { get; set; }
        public string CourseName { get; set; }
        public DateTime CourseStart { get; set; }
        public DateTime CourseEnd { get; set; }

        public string Status { get; set; }
        public string Notes { get; set; }
        public bool StartNotificationsOn { get; set; }

        public bool EndNotificationsOn { get; set; }

    }
}
