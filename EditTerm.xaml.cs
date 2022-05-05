using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SQLite;
using SQLitePCL;
using TrackerApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTerm : ContentPage
    {

        static int term_id;
        static string term_title;
        static DateTime term_start;
        static DateTime term_end;
        public SQLiteConnection _db;
        static Course _selectedCourse;

        public EditTerm(SQLiteConnection db, int id, string title, DateTime start, DateTime end)
        {

            InitializeComponent();

                     
            CourseMenu.IsVisible = false;

            _db = db;
            term_id = id;
            term_title = title;
            term_start = start;
            term_end = end;
            TermTitleEntry.Text = title;
            StartTerm_DatePicker.Date = start.Date;
            EndTerm_DatePicker.Date = end.Date;

            var courseList = db.Table<Course>().Where(x => x.TermId == id).ToList();

            EditTermCourseList.ItemsSource = courseList;

        }

        public void NavigatetoTerms()
        {
            Navigation.PushAsync(new Terms());
        }

        public void AddCourse_Clicked(object sender, EventArgs args)
        {
            int courseCount = _db.Table<Course>().Where(x => x.TermId == term_id).Count();
            if (courseCount > 5)
            {
                DisplayAlert("Sorry", "You already have six courses added for this term.", "Ok");
                return;
            }
            Navigation.PushAsync(new AddCourse(_db, term_id, term_title, term_start, term_end));
        }

        public void EditTermCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new Terms());
        }

        public void EditTermSubmit_Clicked(object sender, EventArgs args)
        {        
            SQLiteCommand command = new SQLiteCommand(_db);
            var title = TermTitleEntry.Text;
            var start = StartTerm_DatePicker.Date;
            var end = EndTerm_DatePicker.Date;
            command.CommandText = "UPDATE Term SET TermTitle = @title, TermStart = @start, TermEnd = @end WHERE TermId = @id";
            command.Bind("@id", term_id);
            command.Bind("@title", title);
            command.Bind("@start", start);
            command.Bind("@end", end);

            command.ExecuteNonQuery();
            Navigation.PushAsync(new Terms());
        }



        private void EditCourse_Clicked(object sender, EventArgs args)
        {



            var courseId = _selectedCourse.CourseId;
            var courseName = _selectedCourse.CourseName;
            var courseStart = _selectedCourse.CourseStart;
            var courseEnd = _selectedCourse.CourseEnd;
            var startNotify = _selectedCourse.StartNotificationsOn;
            var endNotify = _selectedCourse.EndNotificationsOn;
            var courseStatus = _selectedCourse.Status;
            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, courseId, courseName, courseStart, courseEnd, startNotify, endNotify, courseStatus));
        }


        private async void DeleteCourse_Clicked(object sender, EventArgs args)
        {

            var id = _selectedCourse.CourseId;

            string query = string.Format("DELETE FROM Course WHERE CourseId = {0}", id);
            SQLiteCommand command = new SQLiteCommand(_db);
            command.CommandText = query;
            bool answer = await DisplayAlert("Delete Course", "Are you sure you want to delete this course?", "Yes", "No");

            if (answer)
            {
                command.ExecuteNonQuery();
            }
            Navigation.PushAsync(new EditTerm(_db, term_id, term_title, term_start, term_end));

        }

        private void CourseListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            CourseMenu.IsVisible = true;
            _selectedCourse = ((ListView)sender).SelectedItem as Course;

        }
    }
}