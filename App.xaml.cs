using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;
using TrackerApp.Models;
using Plugin.LocalNotifications;
using System.Collections.Generic;

namespace TrackerApp
{
    public partial class App : Application
    {
        static SQLiteConnection db;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Terms());
        }

        protected override void OnStart()
        {
            Init();
            var courses = db.Table<Course>().ToList();
            var assessments = db.Table<Assessment>().ToList();
            CheckNotifications(courses, assessments);
        }

        static void CheckNotifications(List<Course> courses, List<Assessment> assessments)
        {
            foreach (Course c in courses)
            {
                if (c.CourseStart.DayOfYear == DateTime.Now.DayOfYear && c.StartNotificationsOn == true)
                {
                    string body = string.Format("{0} starts today!", c.CourseName);
                    CrossLocalNotifications.Current.Show("Course Start Notification", body);
                }

                if (c.CourseEnd.DayOfYear == DateTime.Now.DayOfYear && c.EndNotificationsOn == true)
                {
                    string body = string.Format("{0} ends today!", c.CourseName);
                    CrossLocalNotifications.Current.Show("Course End Notification", body);
                }
            }

            foreach (Assessment a in assessments)
            {
                if (a.AssessmentStart.DayOfYear == DateTime.Now.DayOfYear && a.StartNotificationsOn == true)
                {
                    string body = string.Format("{0} starts today!", a.AssessmentName);
                    CrossLocalNotifications.Current.Show("Assessment Start Notification", body);
                }

                if (a.AssessmentEnd.DayOfYear == DateTime.Now.DayOfYear && a.EndNotificationsOn == true)
                {
                    string body = string.Format("{0} ends today!", a.AssessmentName);
                    CrossLocalNotifications.Current.Show("Assessment End Notification", body);
                }
            }
        }
        static void Init()
        {

            if (db != null)
            {
                return;
            }
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyData.db");

            db = new SQLiteConnection(databasePath);
            db.CreateTable<Instructor>();
            db.CreateTable<Assessment>();
            db.CreateTable<Course>();
            db.CreateTable<Term>();

        }
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
