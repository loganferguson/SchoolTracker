﻿using System;
using System.Collections.Generic;
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
    public partial class EditCourse : ContentPage
    {

        static int term_id;
        static string term_title;
        static DateTime term_start;
        static DateTime term_end;

        static int course_id;
        static string course_name;
        static DateTime course_start;
        static DateTime course_end;
        static bool course_start_notify;
        static bool course_end_notify;
        public SQLiteConnection _db;
        static Assessment _selectedAssessment;

        public EditCourse(SQLiteConnection db, int termId, string termTitle, DateTime termStart, DateTime termEnd, int courseId, string courseName, DateTime courseStart, DateTime courseEnd, bool courseStartNotify, bool courseEndNotify)
        {
            InitializeComponent();
            AssessmentMenu.IsVisible = false;

            _db = db;

            term_id = termId;
            term_title = termTitle;
            term_start = termStart;
            term_end = termEnd;

            course_id = courseId;
            course_name = courseName;
            course_start = courseStart;
            course_end = courseEnd;
            course_start_notify = courseStartNotify;
            course_end_notify = courseEndNotify;

            CourseName.Text = course_name;
            StartCourse_DatePicker.Date = course_start.Date;
            EndCourse_DatePicker.Date = course_end.Date;

            if (courseStartNotify)
            {
                StartCourse_Switch.IsToggled = true;
            }
            if (courseEndNotify)
            {
                EndCourse_Switch.IsToggled = true;
            }
            var assessmentList = db.Table<Assessment>().Where(x => x.CourseId == course_id).ToList();

            EditCourseAssessmentList.ItemsSource = assessmentList;
        }


        public void EditCourseCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new EditTerm(_db, term_id, term_title, term_start, term_end));
        }

        public void EditCourseSubmit_Clicked(object sender, EventArgs args)
        {
            SQLiteCommand command = new SQLiteCommand(_db);
            var name = CourseName.Text;
            var start = StartCourse_DatePicker.Date;
            var end = EndCourse_DatePicker.Date;
            var notifyStart = NotificationIsOn(StartCourse_Switch);
            var notifyEnd = NotificationIsOn(EndCourse_Switch);
            command.CommandText = "UPDATE Course SET CourseName = @name, CourseStart = @start, CourseEnd = @end, StartNotificationsOn = @notifyStart, EndNotificationsOn = @notifyEnd WHERE CourseId = @id";
            command.Bind("@id", course_id);
            command.Bind("@name", name);
            command.Bind("@start", start);
            command.Bind("@end", end);
            command.Bind("@notifyStart", notifyStart);
            command.Bind("@notifyEnd", notifyEnd);
            
            command.ExecuteNonQuery();
            Navigation.PushAsync(new EditTerm(_db, term_id, term_title, term_start, term_end));
        }

        public bool NotificationIsOn(Switch notificationSwitch)
        {
            if (notificationSwitch.IsToggled)
            {
                return true;
            }

            return false;
        }

        /*---------ASSESSMENT MENU BUTTON CLICKS-------------*/

        public void AddAssessment_Clicked(object sender, EventArgs args)
        {
            int assessmentCount = _db.Table<Assessment>().Where(x => x.CourseId == course_id).Count();
            if(assessmentCount > 1)
            {
                DisplayAlert("Sorry", "This course already has two assessments.", "Ok");
                return;
            }
            Navigation.PushAsync(new AddAssessment(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify));
        }

        public void EditAssessment_Clicked(object sender, EventArgs args)
        {
            var assessmentId = _selectedAssessment.AssessmentId;
            var assessmentName = _selectedAssessment.AssessmentName;
            var assessmentStart = _selectedAssessment.AssessmentStart;
            var assessmentEnd = _selectedAssessment.AssessmentEnd;
            var assessmentType = _selectedAssessment.AssessmentType;
            var assessmentStartNotify = _selectedAssessment.StartNotificationsOn;
            var assessmentEndNotify = _selectedAssessment.EndNotificationsOn;
            Navigation.PushAsync(new EditAssessment(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify, assessmentId, assessmentType, assessmentName, assessmentStart, assessmentEnd, assessmentStartNotify, assessmentEndNotify));
        }

        public async void DeleteAssessment_Clicked(object sender, EventArgs args)
        {
            var id = _selectedAssessment.CourseId;

            string query = string.Format("DELETE FROM Assessment WHERE AssessmentId = {0}", id);
            SQLiteCommand command = new SQLiteCommand(_db);

            command.CommandText = query;

            bool answer = await DisplayAlert("Delete Assessment", "Are you sure you want to delete this assessment?", "Yes", "No");

            if (answer)
            {
                command.ExecuteNonQuery();
            }

            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify));
        }

        public void CourseNotes_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new CourseNotes(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify));
        }
        public void AssessmentListView_ItemSelected(object sender, EventArgs args)
        {
            AssessmentMenu.IsVisible = true;
            _selectedAssessment = ((ListView)sender).SelectedItem as Assessment;
        }
    }
}