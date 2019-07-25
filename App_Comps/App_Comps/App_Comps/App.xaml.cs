using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using SQLite;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace App_Comps
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            Position3D position3D = new Position3D();

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
        public static Location phoneloc { get; set; } = new Location();
        public static Location location { get; set; }
        public static double alpha { get; set; } = 0;
        public static SQLiteConnection db;
        public static System.Collections.ObjectModel.ObservableCollection<string> coordinatesList = new System.Collections.ObjectModel.ObservableCollection<string>();
        public static List<Coords> coordsList = new List<Coords>();
    }

    class Coords
    {
        public double longitude;
        public double latitude;
        public string address;
        public int visits { get; set; }
        public bool isFav { get; set; }

        public override string ToString()
        {
            return "latitude: " + latitude.ToString() + "longitude: " + longitude.ToString();
        }
    }

    public class Position3D //reference to: https://github.com/indiepend/compass/issues/23
    {
        // At basic level it restarts compass when phone changes position for about 70 degrees in x or y orientation
        // I'm not really sure that it's working correctly but i'm gonna check it
        bool _flag=false;//flag for first orientation check
        float _x, _y;//registered orientation
        public Position3D()
        {
            // Register for reading changes
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
        }

        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)//on orientation change
        {
            var data = e.Reading;
            Console.WriteLine($"Reading: X: {data.Orientation.X}, Y: {data.Orientation.Y}, Z: {data.Orientation.Z}, W: {data.Orientation.W}");
            if (!_flag) {//if it's first orientation check
                _flag = true;//then flag gotta go
                _x = data.Orientation.X;//first orientation
                _y = data.Orientation.Y;//=||=
            }
            else {//if it isn't first check
                if (Math.Abs(_x-data.Orientation.X) > 0.6 || Math.Abs(_y - data.Orientation.Y) > 0.6)//if orientation changed for about 70 degrees
                {
                    CompassInterface.ToggleCompass();//switches off
                    CompassInterface.ToggleCompass();//switches on
                    _x = data.Orientation.X;//new orientation
                    _y = data.Orientation.Y;//=||=
                }
            }
        }

        public static void ToggleOrientationSensor()//as name says it toggles on and off sensor, used once in MainActivity.cs
        {
            try
            {
                if (OrientationSensor.IsMonitoring)
                    OrientationSensor.Stop();
                else
                    OrientationSensor.Start(SensorSpeed.UI);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature not supported on device
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }
    }
}
