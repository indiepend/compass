using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using SQLite;
using System.IO;
namespace App_Comps
{
    public partial class MainPage : CarouselPage
    {
        Location phoneloc = new Location(), location;
        double alpha = 0;
        public static SQLiteConnection db;
        public System.Collections.ObjectModel.ObservableCollection<string> coordinates = new System.Collections.ObjectModel.ObservableCollection<string>();
        List<coordy> coord = new List<coordy>();
        [Table("Items")]
        public class Stock
        {
            [PrimaryKey, AutoIncrement, Column("_id")]
            public int Id { get; set; }
            public double longitude { get; set; }
            public double latitude { get; set; }
            public string address { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            //wtf.CreateContent();
            Init_database();
            Compas();
            coordsforlabel(); 
        }
        /// /////////////////////////////////////////////////////////////////
        public void Compas()
        {
            Compass.ReadingChanged += Compass_ReadingChanged;    
        }
        public void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            var data = e.Reading;
            image22.Rotation = -data.HeadingMagneticNorth;
            arrow.Rotation = -data.HeadingMagneticNorth - alpha;
        }
        /// /////////////////////////////////////////////////////////////////
        public async void coordsforlabel()
        {
            while (true)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var newposition = await Geolocation.GetLocationAsync(request);
                
                if (newposition != null)
                {
                    phoneloc = newposition;
                    firstloc.Text = $"{newposition.Latitude}";
                    secondloc.Text = $"{newposition.Longitude}";
                    Console.WriteLine($"Latitude: {newposition.Latitude}, Longitude: {newposition.Longitude}");

                    if (location != null)
                    {
                        double dist = Location.CalculateDistance(location, phoneloc, DistanceUnits.Kilometers);
                        double y = Location.CalculateDistance(phoneloc, location.Latitude, phoneloc.Longitude, DistanceUnits.Kilometers);
                        alpha = (180 * Math.Acos(y / dist)) / 3.1416;//okay jesli kat jest w drugiej cwiartce ukladu wspolrzednych
                        if (phoneloc.Longitude < location.Longitude)//jesli kat jest w pierwsze cwiartce lub czwartej
                        {
                            alpha = -alpha;
                        }
                        if (phoneloc.Latitude > location.Latitude)//jesli kat jest w trzeciej cwiartce lub czwartej
                        {
                            alpha += 180;
                        }
                        distLbl.Text = dist.ToString();
                    }
                }
            }
        }
        public void Historia_Selected(object sender, EventArgs e)
        {
            location = new Location();
            arrow.IsVisible = true;

            string str = "", temp = "";

            str = (sender as ListView).SelectedItem.ToString();

            bool flag = true;
            int index = 0;
            while (flag)
            {
                if (str[index] != '.')
                {
                    temp += str[index];
                }
                else
                {
                    flag = false;
                }
                index++;
            }
            location.Longitude = coord[int.Parse(temp) - 1].longitude;
            location.Latitude = coord[int.Parse(temp) - 1].latitude;
            loncoordLbl.Text = "longitude: " + location.Longitude.ToString();
            latcoordLbl.Text = "latitude: " + location.Latitude.ToString();
        }

        public void onDelete(object sender,EventArgs e)
        {
            var item = ((MenuItem)sender);
            var cell = item.CommandParameter;
            coordinates.Remove(cell.ToString());
            string fast = (cell as string);

            string str = "";
            bool flag = true;
            int index = 0;
            while (flag)
            {
                if (fast[index] != '.')
                {
                    str += fast[index];
                }
                else
                {
                    flag = false;
                }
                index++;
            }
            db.Delete<Stock>(int.Parse(str) - 1);
        }

        public void Init_database()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "history.db3");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<Stock>();

            var table = db.Table<Stock>();
            
            var s = table.GetEnumerator();
            s.Reset();
            s.MoveNext();

            int i = 0;
            foreach (var k in table)
            {
                historia.ItemSelected += Historia_Selected;
                if (s.Current.address != null)
                {
                    coordinates.Add($"{i+1}. \n longitude: {s.Current.longitude.ToString()} \n latitude: {s.Current.latitude.ToString()} \n address: {s.Current.address}");    
                }
                else{
                    coordinates.Add($"{i+1}. \n longitude: {s.Current.longitude.ToString()} \n latitude: {s.Current.latitude.ToString()}"); 
                }
                coord.Add(new coordy());
                coord[i].latitude = s.Current.latitude;
                coord[i].longitude = s.Current.longitude;
                Console.WriteLine($"at first you have {i} then {db.Table<Stock>().Count()}");
                Console.WriteLine($"First {s.Current.latitude.ToString()} second {s.Current.longitude.ToString()} ");//third {s.Current.address}");
                s.MoveNext();
                i++;
            }
            historia.ItemsSource = coordinates;
        }

        async void Searching(object sender, System.EventArgs e)
        {
        
            if (firstcoord.Text != null && secondcoord.Text != null)
            {
                location = new Location();
                location.Latitude = Double.Parse(firstcoord.Text);
                location.Longitude = Double.Parse(secondcoord.Text);
            }
            else if(addrforsearch.Text != null)
            {
                location = new Location();
                var locations = await Geocoding.GetLocationsAsync(addrforsearch.Text);
                location = locations?.FirstOrDefault();
            }
            else
            {
                Console.WriteLine("prooooooba 3");
                
            }

            if (location != null)
            {
                double dist = Location.CalculateDistance(location, phoneloc, DistanceUnits.Kilometers);
                double y = Location.CalculateDistance(phoneloc, location.Latitude, phoneloc.Longitude, DistanceUnits.Kilometers);
                Console.WriteLine($"dyst {dist} /n oraz dyst2 {y}");
                alpha = (180*Math.Acos(y / dist))/3.1416;//okay jesli kat jest w drugiej cwiartce ukladu wspolrzednych
                if (phoneloc.Longitude < location.Longitude)//jesli kat jest w pierwsze cwiartce lub czwartej
                {
                    alpha = -alpha;
                }
                if (phoneloc.Latitude > location.Latitude)//jesli kat jest w trzeciej cwiartce lub czwartej
                {
                    alpha += 180;
                }
                arrow.IsVisible = true;

                distLbl.Text = "distance: " + dist.ToString("F");
                loncoordLbl.Text = "longitude: " + location.Longitude.ToString();
                latcoordLbl.Text = "latitude: " + location.Latitude.ToString();

                var newCoord = new Stock();
                newCoord.latitude = location.Latitude;
                newCoord.longitude = location.Longitude;
                newCoord.address = null;
                db.Insert(newCoord);
            }
        }


    }
    public class Compass_class
    {
            public static void ToggleCompass()
            {
            
            SensorSpeed speed = SensorSpeed.Game;
            if (Compass.IsMonitoring)
                    Compass.Stop();
                else
                    Compass.Start(speed, applyLowPassFilter: true);
                //try
                //{
                //    if (Compass.IsMonitoring)
                //        Compass.Stop();
                //    else
                //        Compass.Start(speed);
                //}
                //catch (FeatureNotSupportedException fnsEx)
                //{

                //    // Feature not supported on device
                //}
                //catch (Exception ex)
                //{
                //    // Some other exception has occurred
                //}
                
            }
        }

    class coordy
    {
        public double longitude;
        public double latitude;
        public string address;

        public override string ToString()
        {
            return "latitude: " + latitude.ToString() + "longitude: " + longitude.ToString();
        }
    }
}