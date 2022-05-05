using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerApp.Models;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessment : ContentPage
    {
        static SQLiteConnection _db;
        int term_id;
        string term_title;
        DateTime term_start;
        DateTime term_end;

        int course_id;
        string course_name;
        DateTime course_start;
        DateTime course_end;
        bool start_notify;
        bool end_notify;
        string course_status;

        static void Init()
        {
            if (_db != null)
            {
                return;
            }
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

            _db = new SQLiteConnection(databasePath);
            _db.CreateTable<Instructor>();
            _db.CreateTable<Assessment>();
            _db.CreateTable<Course>();
            _db.CreateTable<Term>();
        }

        public AddAssessment(SQLiteConnection db, int termId, string termTitle, DateTime termStart, DateTime termEnd, int courseId, string courseName, DateTime courseStart, DateTime courseEnd, bool startNotify, bool endNotify, string courseStatus)
        {
            InitializeComponent();

            _db = db;
            term_id = termId;
            term_title = termTitle;
            term_start = termStart;
            term_end = termEnd;

            course_id = courseId;
            course_name = courseName;
            course_start = courseStart;
            course_end = courseEnd;
            start_notify = startNotify;
            end_notify = endNotify;
            course_status = courseStatus;

            string[] assessmentTypes = { "Objective", "Performance" };
            TypePicker.ItemsSource = assessmentTypes;
        }


        public void AddAssessmentCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, start_notify, end_notify, course_status));
        }

        public void AddAssessmentSubmit_Clicked(object sender, EventArgs args)
        {
            Init();

            Assessment assessment = new Assessment
            {
                CourseId = course_id,
                AssessmentName = AssessmentNameEntry.Text,
                AssessmentStart = StartAssessment_DatePicker.Date,
                AssessmentEnd = EndAssessment_DatePicker.Date,
                AssessmentType = (string)TypePicker.ItemsSource[TypePicker.SelectedIndex],
                StartNotificationsOn = NotificationIsOn(StartAssessment_Switch),
                EndNotificationsOn = NotificationIsOn(EndAssessment_Switch)
            };

            _db.Insert(assessment);

            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, start_notify, end_notify, course_status));

        }

        public bool NotificationIsOn(Switch notificationSwitch)
        {
            if (notificationSwitch.IsToggled)
            {
                return true;
            }

            return false;
        }
    }
}