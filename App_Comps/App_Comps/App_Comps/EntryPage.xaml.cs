﻿using System;
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

        public void SubscribeForObjects()//listens for changes; check code below for explanations
        {
            MessagingCenter.Subscribe<CompassInterface,double>(this, "imageSend", (sender,e) => { compassImg.Rotation = e; });
            MessagingCenter.Subscribe<CompassInterface, double>(this, "arrowSend", (sender, e) => { arrowImg.Rotation = e; });

            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "longSend", (sender, e) => { currentLon.Text = e; });
            MessagingCenter.Subscribe<CoordinatesFinder, string>(this, "latSend", (sender, e) => { currentLat.Text = e; });

            MessagingCenter.Subscribe<Trytofind, bool>(this, "arrowVisSend", (sender, e) => { arrowImg.IsVisible = e; frame.IsVisible = e; });
            MessagingCenter.Subscribe<Trytofind, string>(this, "distSend", (sender, e) => { distLbl.Text = e; });
            MessagingCenter.Subscribe<Trytofind, string>(this, "aimlatSend", (sender, e) => { latCoordLbl.Text = e; });
            MessagingCenter.Subscribe<Trytofind, string>(this, "aimlongSend", (sender, e) => { lonCoordLbl.Text = e; });
        }
	}

    public class CompassInterface
    {
        public CompassInterface()//Subscribing to event
        {
            Compass.ReadingChanged += CompassReadingChanged;
            //if phone turns it triggers method below
        }

       public void CompassReadingChanged(object sender, CompassChangedEventArgs e)
       {
            var _data = e.Reading;
            MessagingCenter.Send(this, "imageSend", -_data.HeadingMagneticNorth);//sends new rotation of compass to EntryPage class
            MessagingCenter.Send(this, "arrowSend", -_data.HeadingMagneticNorth - AppVariables.Alpha);//sends new rotation of arrow
       }

        public static void ToggleCompass()//toggles compass on and off - used in MainActivity class
        {
            SensorSpeed speed = SensorSpeed.Game;//sensor speed
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
                    AppVariables.PhoneLoc = newposition;//sends position to variables manager as it's needed later
                    _position = newposition;
                    MessagingCenter.Send(this, "latSend", "lat.: " + _position.Latitude.ToString());//sends new position to show
                    MessagingCenter.Send(this, "longSend", "long.: " + _position.Longitude.ToString());

                    Trytofind trytofind = new Trytofind();
                    trytofind.Check();//explanation below
                }
            }
        }
    }

    public class Trytofind {//checks if there's already destination and calculates distance and new angle for arrow
        public void Check()
        {
            if (AppVariables.location != null && AppVariables.PhoneLoc != null)//if there is destination chosen
            {
                MessagingCenter.Send(this, "aimlatSend", "lat.: " + AppVariables.location.Latitude.ToString());//sends new destination
                MessagingCenter.Send(this, "aimlongSend", "long.: " + AppVariables.location.Longitude.ToString());

                double dist = Location.CalculateDistance(AppVariables.location, AppVariables.PhoneLoc, DistanceUnits.Kilometers);//calculates distance between user and destination point
                double y = Location.CalculateDistance(AppVariables.PhoneLoc, AppVariables.location.Latitude, AppVariables.PhoneLoc.Longitude, DistanceUnits.Kilometers);//that's additional distance needed to calculate angle
                AppVariables.Alpha = (180 * Math.Acos(y / dist)) / 3.1416;//that result is okay when aim is in second quadrant
                if (AppVariables.PhoneLoc.Longitude < AppVariables.location.Longitude)//when result is in first or forth
                {
                    if (AppVariables.PhoneLoc.Latitude > AppVariables.location.Latitude)
                        AppVariables.Alpha += 180;//forth
                    else
                        AppVariables.Alpha = -AppVariables.Alpha;//first
                }
                else if (AppVariables.PhoneLoc.Latitude > AppVariables.location.Latitude)//when result is in third
                    AppVariables.Alpha = 180 - AppVariables.Alpha;


                if (dist > 100)
                    MessagingCenter.Send(this, "distSend", Math.Round(dist).ToString() + "km");//sends distance 
                else if (dist > 1)
                    MessagingCenter.Send(this, "distSend", (Math.Round(dist * 10) / 10).ToString() + "km");
                else
                    MessagingCenter.Send(this, "distSend", (Math.Round(dist * 100) * 10).ToString() + "m");

                MessagingCenter.Send(this, "arrowVisSend", true);//makes arrow on compass visible
            }
        }
    }

    public interface ISensorEventListener
    {
        void isCalibrated();
    }
}