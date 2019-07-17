using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
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
            //CompassInterface.ToggleCompass();//turns off compass because when resumes it stops working
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            //CompassInterface.ToggleCompass();//turns it back on
            // Handle when your app resumes
        }
    }

    public class Position3D
    {
        // Set speed delay for monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;

        public Position3D()
        {
            // Register for reading changes, be sure to unsubscribe when finished
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
        }

        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            Console.WriteLine($"Reading: X: {data.Orientation.X}, Y: {data.Orientation.Y}, Z: {data.Orientation.Z}, W: {data.Orientation.W}");
            // Process Orientation quaternion (X, Y, Z, and W)
        }

        public void ToggleOrientationSensor()
        {
            try
            {
                if (OrientationSensor.IsMonitoring)
                    OrientationSensor.Stop();
                else
                    OrientationSensor.Start(speed);
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
