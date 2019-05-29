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
	public partial class EntryPage : ContentPage
    {
		public EntryPage ()
		{
			InitializeComponent ();

            SubscribeForObjects();//listener for changes in UI

            CompassInterface cmp = new CompassInterface();//sends data about arrow and compass rotation

            CoordinatesFinder finder = new CoordinatesFinder();//finds and sends coordinates of user
            finder.Run();
		}

        public void SubscribeForObjects()//listens for changes
        {
            MessagingCenter.Subscribe<CompassInterface,double>(this, "imageSend", (sender,e) => { image22.Rotation = e; });
            MessagingCenter.Subscribe<CompassInterface, double>(this, "arrowSend", (sender, e) => { arrow.Rotation = e; });

            MessagingCenter.Subscribe<CoordinatesFinder, bool>(this, "arrowVisSend", (sender, e) => { arrow.IsVisible = e; frame.IsVisible = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "latSend", (sender, e) => { firstloc.Text = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "longSend", (sender, e) => { secondloc.Text = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "distSend", (sender, e) => { distLbl.Text = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "aimlatSend", (sender, e) => { latcoordLbl.Text = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "aimlongSend", (sender, e) => { loncoordLbl.Text = e; });
        }
	}

    public class CompassInterface
    {
        public CompassInterface()//Subscribing to event
        {
            Compass.ReadingChanged += CompassReadingChanged;
            //if compass turns it triggers method below
        }

       public void CompassReadingChanged(object sender, CompassChangedEventArgs e)
       {
            var _data = e.Reading;
            MessagingCenter.Send(this, "imageSend", -_data.HeadingMagneticNorth);//sends new rotation of compass
            MessagingCenter.Send(this, "arrowSend", -_data.HeadingMagneticNorth - AppVariables.alpha);//sends new rotation of arrow
       }

        public static void ToggleCompass()//toggles compass on and off - used in MainActivity class
        {
            SensorSpeed speed = SensorSpeed.Game;
            if (Compass.IsMonitoring)
                Compass.Stop();
            else
                Compass.Start(speed, applyLowPassFilter: true);
        }
    }

    public class CoordinatesFinder
    {
        public async void Run()
        {
            Location _position = new Location();
            while (true)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var newposition = await Geolocation.GetLocationAsync(request);//waits for new location

                if (newposition != null)
                {
                    AppVariables.phoneloc = newposition;//sends position to variables manager it's needed later
                    _position = newposition;
                    MessagingCenter.Send(this, "latSend", "lat.: " + _position.Latitude.ToString());//sends new position to show
                    MessagingCenter.Send(this, "longSend", "long.: " + _position.Longitude.ToString());
                    if (AppVariables.location != null)//if there is destination choosen
                    {
                        MessagingCenter.Send(this, "aimlatSend", "lat.: " + AppVariables.location.Latitude.ToString());//sends new destination
                        MessagingCenter.Send(this, "aimlongSend", "long.: " + AppVariables.location.Longitude.ToString());

                        double dist = Location.CalculateDistance(AppVariables.location, AppVariables.phoneloc, DistanceUnits.Kilometers);//calculates distance between user and destination point
                        double y = Location.CalculateDistance(AppVariables.phoneloc, AppVariables.location.Latitude, AppVariables.phoneloc.Longitude, DistanceUnits.Kilometers);//that's additional distance needed to calculate radius
                        AppVariables.alpha = (180 * Math.Acos(y / dist)) / 3.1416;//that result is okay when aim is in second quadrant
                        if (AppVariables.phoneloc.Longitude < AppVariables.location.Longitude)//when result is in first or forth
                            AppVariables.alpha = -AppVariables.alpha;

                        if (AppVariables.phoneloc.Latitude > AppVariables.location.Latitude)//when result is in third or forth
                            AppVariables.alpha += 180;

                        
                        if (dist > 100)
                            MessagingCenter.Send(this, "distSend", Math.Round(dist).ToString()+"km");//sends distance 
                        else if(dist > 1)
                            MessagingCenter.Send(this, "distSend", (Math.Round(dist * 10) / 10).ToString() + "km");
                        else
                            MessagingCenter.Send(this, "distSend", (Math.Round(dist * 100) * 10).ToString()+"m");

                        MessagingCenter.Send(this, "arrowVisSend", true);//makes arrow on compass visible
                    }
                }
            }
        }
    }
}