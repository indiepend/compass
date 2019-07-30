using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace App_Comps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : CarouselPage
    {
        public Database database = new Database();
        public HistoryPage()
        {
            InitializeComponent();
            Children.Add(new All());
            Children.Add(new Favourite());
            MessagingCenter.Subscribe<Favourite>(this, "GoBackSend", (sender) => { CurrentPage = Children[0]; });
            MessagingCenter.Subscribe<All>(this, "GoBackSend", (sender) => { CurrentPage = Children[1]; });
        }
    }

    [Table("Registry")]
    public class ArchiveDatabase
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int Time { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Note { get; set; }
        public double VisitsNumber { get; set; }
        public bool IsFavourite { get; set; }
    }

    public class Database
    {
        public Database()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "archive.db3");//path to database
            AppVariables.db = new SQLiteConnection(dbPath);//connecting to database or creating new one
            AppVariables.db.CreateTable<ArchiveDatabase>();//creating table if it doesn't exist

            var _main = AppVariables.db.Query < ArchiveDatabase > ("SELECT * FROM Registry ORDER BY Time DESC");//all records starting from most recent selected
            var _favourites = AppVariables.db.Query < ArchiveDatabase > ("SELECT * FROM Registry WHERE IsFavourite = 1");//favourites 
            var _oftenlyVisited = AppVariables.db.Query < ArchiveDatabase > ("SELECT * FROM Registry WHERE VisitsNumber > 5");//ofently visited
             
            var _enum = _main.GetEnumerator();
            _enum.MoveNext();

            int i = 0;
            foreach (var k in _main)//populates main observable collection with data
            {
                AppVariables.coordsList.Add(new Coords());
                AppVariables.coordsList[i].latitude = _enum.Current.Latitude;
                AppVariables.coordsList[i].longitude = _enum.Current.Longitude;
                AppVariables.coordsList[i].Id = _enum.Current.Id;
                AppVariables.coordsList[i].note = _enum.Current.Note;
                AppVariables.coordsList[i].IsFav = _enum.Current.IsFavourite;
                _enum.MoveNext();
                i++;
            }
            i = 0;
            _enum = _favourites.GetEnumerator();
            _enum.MoveNext();
            foreach (var f in _favourites)//populates favourites
            {
                AppVariables.favouritesList.Add(new Coords());
                AppVariables.favouritesList[i].latitude = _enum.Current.Latitude;
                AppVariables.favouritesList[i].longitude = _enum.Current.Longitude;
                AppVariables.favouritesList[i].Id = _enum.Current.Id;
                AppVariables.favouritesList[i].note = _enum.Current.Note;
                AppVariables.favouritesList[i].IsFav = _enum.Current.IsFavourite;
                _enum.MoveNext();
                i++;
            }
        }
    }
}