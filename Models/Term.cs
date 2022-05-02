using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using TrackerApp.Models;
namespace TrackerApp
{
    class Term
    {

        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }
        public string TermTitle { get; set; }
        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }


    }
}
