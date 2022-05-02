using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLitePCL;
using TrackerApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseNotes : ContentPage
    {

        static int term_id;
        static string term_title;
        static DateTime term_start;
        static DateTime term_end;

        static int course_id;
        static string course_name;
        static DateTime course_start;
        static DateTime course_end;
        static bool start_notify;
        static bool end_notify; 

        public SQLiteConnection _db;
        public CourseNotes(SQLiteConnection db, int termId, string termTitle, DateTime termStart, DateTime termEnd, int courseId, string courseName, DateTime courseStart, DateTime courseEnd, bool startNotify, bool endNotify)
        {
            InitializeComponent();

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

            _db = db;
            SQLiteCommand command = new SQLiteCommand(db);
            
       
            var notes = db.Table<Course>().Where(x => x.CourseId == course_id).FirstOrDefault() as Course;

            NotesArea.Text = notes.Notes;
        }

        public void SubmitNote_Clicked(object sender, EventArgs args)
        {
            var updatedNotes = GetCurrentNotes() + Environment.NewLine + "\t\u2022 " + NotesEntry.Text;
            _db.Execute("UPDATE Course SET Notes = ? WHERE CourseId = ?", updatedNotes, course_id);

            Navigation.PushAsync(new CourseNotes(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, start_notify, end_notify));
        }

        public void CancelNote_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, start_notify, end_notify));
        }

        public string GetCurrentNotes()
        {
            var course = _db.Table<Course>().Where(x => x.CourseId == course_id).FirstOrDefault() as Course;

            return course.Notes;
        }

        public async void ShareNotes_Clicked(object sender, EventArgs args)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = NotesArea.Text

            });
        }

    }
}