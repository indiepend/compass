using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using SQLite;

[assembly: XamlCompilation(XamlCompilationOptions.Skip)]
namespace App_Comps
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }

    class AppVariables//stores variables used throughout application
    {
        public static Location PhoneLoc { get; set; } = new Location();
        public static Location location { get; set; }
        public static double Alpha { get; set; } = 0;
        public static SQLiteConnection db;
        public static System.Collections.ObjectModel.ObservableCollection<Coords> favouritesList = new System.Collections.ObjectModel.ObservableCollection<Coords>();
        public static System.Collections.ObjectModel.ObservableCollection<Coords> coordsList = new System.Collections.ObjectModel.ObservableCollection<Coords>();
    }

    class Coords
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string record { get { if (note != null) return note; else return "Long.: " + longitude + " Lat.: " + latitude; } }
        public int Id { get; set; }
        public string note { get; set; }
        public bool IsFav { get; set; }
    }
}
