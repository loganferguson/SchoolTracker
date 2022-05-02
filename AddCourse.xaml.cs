using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using TrackerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCourse : ContentPage
    {

        static int _id;
        static string _title;
        static DateTime _start;
        static DateTime _end;
        static SQLiteConnection _db;
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


        public AddCourse(SQLiteConnection db, int id, string title, DateTime start, DateTime end)
        {
            InitializeComponent();

            _id = id;
            _db = db;
            _title = title;
            _start = start;
            _end = end;
        }


        public void AddCourseCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new EditTerm(_db, _id, _title, _start, _end));
        }

        public void AddCourseSubmit_Clicked(object sender, EventArgs args)
        {
            Init();
            Course course = new Course
            {
                TermId = _id,
                CourseName = CourseNameEntry.Text,
                CourseStart = StartCourse_DatePicker.Date,
                CourseEnd = EndCourse_DatePicker.Date,
                IsComplete = false,
                StartNotificationsOn = NotificationIsOn(StartCourse_Switch),
                EndNotificationsOn = NotificationIsOn(EndCourse_Switch)
            };

            Instructor instructor = new Instructor
            {
                CourseId = course.CourseId,
                InstructorName = InstructorNameEntry.Text,
                Phone = InstructorPhoneEntry.Text,
                Email = InstructorEmailEntry.Text
            };

            _db.Insert(course);
            _db.Insert(instructor);

            Navigation.PushAsync(new EditTerm(_db, _id, _title, _start, _end));

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