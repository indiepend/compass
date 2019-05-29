using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Essentials;
using SQLite;
namespace App_Comps
{

    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            Children.Add(new EntryPage());
            Children.Add(new SearchPage());
            Children.Add(new HistoryPage());
        }
    }
    class Coords
    {
        public double longitude;
        public double latitude;
        public string address;

        public override string ToString()
        {
            return "latitude: " + latitude.ToString() + "longitude: " + longitude.ToString();
        }
    }

    class AppVariables//stores variables used throughout application
    {
        public static Location phoneloc { get; set; } = new Location();
        public static Location location { get; set; }
        public static double alpha { get; set; } = 0;
        public static SQLiteConnection db;
        public static System.Collections.ObjectModel.ObservableCollection<string> coordinatesList = new System.Collections.ObjectModel.ObservableCollection<string>();
        public static List<Coords> coordsList = new List<Coords>();
    }
}