using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerApp.Models
{
    class Instructor
    {

        [PrimaryKey, AutoIncrement]
        public int InstructorId { get; set; }

        public int CourseId { get; set; }
        public string InstructorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
