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
            MessagingCenter.Subscribe<Searcher>(this, "resetSend", (sender) => { longitudeEntry.Text = ""; latitudeEntry.Text = ""; addressEntry.Text = ""; });

            search = new Searcher();
            search.Receiver();
        }

        //sends input to method
        void Sender()
        {
            MessagingCenter.Send(this, "lonSend", longitudeEntry.Text);
            MessagingCenter.Send(this, "latSend", latitudeEntry.Text);
            MessagingCenter.Send(this, "addrSend", addressEntry.Text);
        }

        void Trigger(object sender, System.EventArgs e)//when "Send" button clicked
        {
            Sender();//sends input to class below
            search.Searching();//checks if data is correct and search for given localization
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
            bool _flag = false;
            if (_lon != null && _lat != null)//there's longitude and latitude entered
            {
                double _x = Double.Parse(_lat), _y = Double.Parse(_lon);//localization parsed once to double
                if (_x < 90 && _x > -90 && _y < 180 && _y > -180)//checks if location is within earth
                {
                    AppVariables.location = new Location();//saving this localization as a current one
                    AppVariables.location.Latitude = _x;
                    AppVariables.location.Longitude = _y;
                    _flag = true;
                }
            }
            else if (_addr != null)//there's address entered
            {
                AppVariables.location = new Location();
                var locations = await Geocoding.GetLocationsAsync(_addr);//looks for location
                AppVariables.location = locations?.FirstOrDefault();//sets location found or sets null
                _flag = true;
            }
            else//nothing entered just button clicked
            {

            }

            if (AppVariables.location != null && _flag)//if there's location found
            {
                MessagingCenter.Send(this, "resetSend");//resets input bars
                var newCoord = new HistoryDatabase();
                newCoord.latitude = AppVariables.location.Latitude;//sets as new location to go
                newCoord.longitude = AppVariables.location.Longitude;
                newCoord.note = null;//not added yet
                AppVariables.db.Insert(newCoord);//insert location to database
                MessagingCenter.Send(this, "gobackSend");//tell main page to swipe us back

                Trytofind trytofind = new Trytofind();
                trytofind.Check();//checks if there's already destination and calculates distance and new angle for arrow; find it in EntryPage.xaml.cs
            }
            else
                DependencyService.Get<IMessage>().LongAlert("Wrong coordinates or address");
        }
}

    public interface IMessage //interface for toast messages, more in ToastMsg.cs
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}