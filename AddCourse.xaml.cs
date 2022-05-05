using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Mail;
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

            string[] statuses = { "In-Progress", "Completed", "Dropped", "Plan to take" };
            StatusPicker.ItemsSource = statuses;
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
                Status = (string)StatusPicker.ItemsSource[StatusPicker.SelectedIndex],
                StartNotificationsOn = NotificationIsOn(StartCourse_Switch),
                EndNotificationsOn = NotificationIsOn(EndCourse_Switch)
            };

            _db.Insert(course);

            Instructor instructor = new Instructor
            {
                CourseId = GetCourseId(course.CourseName),
                InstructorName = InstructorNameEntry.Text,
                Phone = InstructorPhoneEntry.Text,
                Email = InstructorEmailEntry.Text
            };
            if(IsValidPhone(InstructorPhoneEntry.Text) == false)
            {
                DisplayAlert("Invalid Phone Number", "Please enter a valid phone number.", "Ok");
                return;
            }
            if(IsValidEmail(InstructorEmailEntry.Text) == false)
            {
                DisplayAlert("Invalid Email", "Please enter a valid email address.", "Ok");
                return;
            }
            
            _db.Insert(instructor);

            Navigation.PushAsync(new EditTerm(_db, _id, _title, _start, _end));

        }

        public bool IsValidPhone(string phone)
        {
            string regEx = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (phone != null)
            {
                return Regex.IsMatch(phone, regEx);
            }
            return false;

        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }


        public int GetCourseId(string courseName)
        {
            var id = _db.Table<Course>().Where(x => x.CourseName == courseName).Last();
            return id.CourseId;
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