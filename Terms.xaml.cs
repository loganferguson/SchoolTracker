using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using SQLitePCL;
using System.IO;
using TrackerApp.Models;

namespace TrackerApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Terms : ContentPage
    {
        readonly ObservableCollection<Term> terms = new ObservableCollection<Term>();

        static SQLiteConnection db;

        static Term _selectedTerm;

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

        public Terms()
        {
            InitializeComponent();
            Init();
            var termList = db.Table<Term>().ToList();

            
            foreach (Term t in termList)
            {          
                terms.Add(t);
            }
            TermListView.ItemsSource = terms; 

            TermMenu.IsVisible = false;
        }

        private void AddTerm_Clicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new AddTerm());
        }

        private void EditTerm_Clicked(object sender, EventArgs args)
        {
            var id = _selectedTerm.TermId;
            var title = _selectedTerm.TermTitle;
            var start = _selectedTerm.TermStart;
            var end = _selectedTerm.TermEnd;
            Navigation.PushAsync(new EditTerm(db, id, title, start, end));
        }

        //private void DeleteTerms()
        //{

        //    string query = "DELETE FROM Term";
        //    SQLiteCommand command = new SQLiteCommand(db);

        //    command.CommandText = query;
        //    command.ExecuteNonQuery();
        //}

        private async void DeleteTerm_Clicked(object sender, EventArgs args)
        {

            var id = _selectedTerm.TermId;

            string query = string.Format("DELETE FROM Term WHERE TermId = {0}", id);
            SQLiteCommand command = new SQLiteCommand(db);
            
            command.CommandText = query;

            bool answer = await DisplayAlert("Delete Term", "Are you sure you want to delete this Term?", "Yes", "No");

            if (answer)
            {
                command.ExecuteNonQuery();
            }

            

            await Navigation.PushAsync(new Terms());
            
            
        }

        private void TermListView_ItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            TermMenu.IsVisible = true;
            _selectedTerm = ((ListView)sender).SelectedItem as Term;

        }

        
    }
}