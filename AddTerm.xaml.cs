using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using TrackerApp.Models;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTerm : ContentPage
    {
        static SQLiteConnection db;
        static void Init()
        {
            if(db != null)
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
        public AddTerm()
        {
            InitializeComponent();
        }

        public void AddTermCancel_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new Terms());
        }

        public void AddTermSubmit_Clicked(object sender, EventArgs args)
        {
            Init();
            Term term = new Term
            {
                TermTitle = TermTitleEntry.Text,
                TermStart = StartTerm_DatePicker.Date,
                TermEnd = EndTerm_DatePicker.Date
            };

            db.Insert(term);
            
            Navigation.PushAsync(new Terms());

        }
    }
}