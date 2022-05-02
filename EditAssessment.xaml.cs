using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAssessment : ContentPage
    {

        //_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, assessmentId, assessmentType, assessmentName, assessmentStart, assessmentEnd
        static SQLiteConnection _db;
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

        static int assessment_id;
        static string assessment_type;
        static string assessment_name;
        static DateTime assessment_start;
        static DateTime assessment_end;
        static bool assessment_start_notify;
        static bool assessment_end_notify;


        public EditAssessment(SQLiteConnection db, int termId, string termTitle, DateTime termStart, DateTime termEnd, int courseId, string courseName, DateTime courseStart, DateTime courseEnd, bool courseStartNotify, bool courseEndNotify, int assessmentId, string assessmentType, string assessmentName, DateTime assessmentStart, DateTime assessmentEnd, bool assessmentStartNotify, bool assessmentEndNotify)
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
            course_start_notify = courseStartNotify;
            course_end_notify = courseEndNotify;

            assessment_id = assessmentId;
            assessment_type = assessmentType;
            assessment_name = assessmentName;
            assessment_start = assessmentStart;
            assessment_end = assessmentEnd;
            assessment_start_notify = assessmentStartNotify;
            assessment_end_notify = assessmentEndNotify;

            if (assessmentStartNotify)
            {
                StartAssessment_Switch.IsToggled = true;
            }
            if (assessmentEndNotify)
            {
                EndAssessment_Switch.IsToggled = true;
            }

            string[] assessmentTypes = { "Objective", "Performance" };
            TypePicker.ItemsSource = assessmentTypes;
            TypePicker.SelectedIndex = GetTypeIndex(assessment_type);
            AssessmentName.Text = assessment_name;
            StartAssessment_DatePicker.Date = assessment_start.Date;
            EndAssessment_DatePicker.Date = assessment_end.Date;
        }


        public int GetTypeIndex(string typeString)
        {
            if(typeString == "Objective")
            {
                return 0;
            }

            return 1;
        }


        public void EditAssessmentSubmit_Clicked(object sender, EventArgs args)
        {
            SQLiteCommand command = new SQLiteCommand(_db);
            var name = AssessmentName.Text;
            var start = StartAssessment_DatePicker.Date;
            var end = EndAssessment_DatePicker.Date;
            var type = (string)TypePicker.ItemsSource[TypePicker.SelectedIndex];
            var notifyStart = NotificationIsOn(StartAssessment_Switch);
            var notifyEnd = NotificationIsOn(EndAssessment_Switch);
            command.CommandText = "UPDATE Assessment SET AssessmentName = @name, AssessmentStart = @start, AssessmentEnd = @end,  AssessmentType = @type, StartNotificationsOn = @startNotify, EndNotificationsOn = @endNotify WHERE AssessmentId = @id";
            command.Bind("@id", assessment_id);
            command.Bind("@name", name);
            command.Bind("@start", start);
            command.Bind("@end", end);
            command.Bind("@type", type);
            command.Bind("@startNotify", notifyStart);
            command.Bind("@endNotify", notifyEnd);

            command.ExecuteNonQuery();
            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify));
        }

        public bool NotificationIsOn(Switch notificationSwitch)
        {
            if (notificationSwitch.IsToggled)
            {
                return true;
            }

            return false;
        }

        public void EditAssessmentCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new EditCourse(_db, term_id, term_title, term_start, term_end, course_id, course_name, course_start, course_end, course_start_notify, course_end_notify));
        }
        
    }
}