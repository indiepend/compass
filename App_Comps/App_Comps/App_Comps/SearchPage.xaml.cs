using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App_Comps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        Searcher search;
        public SearchPage()
        {
            InitializeComponent();
            search = new Searcher();
            search.Receiver();
        }

        //sends input to method
        void Sender()
        {
            MessagingCenter.Send(this, "latSend", firstcoord.Text);
            MessagingCenter.Send(this, "lonSend", secondcoord.Text);

            MessagingCenter.Send(this, "addrSend", addrforsearch.Text);
        }

        void Trigger(object sender, System.EventArgs e)//when button clicked
        {
            Sender();
            search.Searching();
        }
    }

    public class Searcher
    {
        string _lon, _lat, _addr;

        public void Receiver()//listens for input
        {
            MessagingCenter.Subscribe<SearchPage, string>(this, "latSend", (sender, e) => { _lat = e; });
            MessagingCenter.Subscribe<SearchPage, string>(this, "lonSend", (sender, e) => { _lon = e; });
            MessagingCenter.Subscribe<SearchPage, string>(this, "addrSend", (sender, e) => { _addr = e; });
        }

        async public void Searching()//searches for such input
        {

            if (_lon != null && _lat != null)//there's longitude and latitude entered
            {
                AppVariables.location = new Location();
                AppVariables.location.Latitude = Double.Parse(_lat);
                AppVariables.location.Longitude = Double.Parse(_lon);
            }
            else if (_addr != null)//there's address entered
            {
                AppVariables.location = new Location();
                var locations = await Geocoding.GetLocationsAsync(_addr);//looks for location
                AppVariables.location = locations?.FirstOrDefault();//sets location found or sets null
            }
            else//nothing entered just button clicked
            {

            }

            if (AppVariables.location != null)//if there's location like that
            {
                var newCoord = new HistoryDatabase();
                newCoord.latitude = AppVariables.location.Latitude;//sets as new location to go
                newCoord.longitude = AppVariables.location.Longitude;
                newCoord.address = null;//not added yet
                AppVariables.db.Insert(newCoord);//insert location to database
            }
        }
    }
}