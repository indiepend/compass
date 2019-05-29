﻿using System;
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
	public partial class HistoryPage : ContentPage
    {
		public HistoryPage()
		{
			InitializeComponent();
            Database database = new Database();
            historia.ItemSelected += database.HistoriaSelected;//when location is selected from list it triggers this method
            historia.ItemsSource = AppVariables.coordinatesList;//takes coordinates from database as source
        }

        void onDelete(object sender, EventArgs e)//it going around method to delete location from list
        {
            var item = ((MenuItem)sender);
            var cell = item.CommandParameter;
            AppVariables.coordinatesList.Remove(cell.ToString());//first it's removed from listview so can't be seen anymore
            string fast = (cell as string);

            string str = "";
            bool flag = true;
            int index = 0;
            while (flag)//takes index of deleted location
            {
                if (fast[index] != '.')
                    str += fast[index];

                else
                    flag = false;

                index++;
            }
            AppVariables.db.Delete<HistoryDatabase>(int.Parse(str) - 1);//deletes location in database by it's index
        }
    }

    [Table("Items")]
    public class HistoryDatabase
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string address { get; set; }
    }

    class Database
    {
        public Database()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "history.db3");
            AppVariables.db = new SQLiteConnection(dbPath);//connecting to local database or creating new one
            AppVariables.db.CreateTable<HistoryDatabase>();

            var _table = AppVariables.db.Table<HistoryDatabase>();

            var _s = _table.GetEnumerator();
            _s.Reset();
            _s.MoveNext();

            int i = 0;
            foreach (var k in _table)//populates observable collection with data
            {

                if (_s.Current.address != null)
                    AppVariables.coordinatesList.Add($"{i + 1}. \n longitude: {_s.Current.longitude.ToString()} \n latitude: {_s.Current.latitude.ToString()} \n address: {_s.Current.address}");

                else
                    AppVariables.coordinatesList.Add($"{i + 1}. \n longitude: {_s.Current.longitude.ToString()} \n latitude: {_s.Current.latitude.ToString()}");

                AppVariables.coordsList.Add(new Coords());
                AppVariables.coordsList[i].latitude = _s.Current.latitude;
                AppVariables.coordsList[i].longitude = _s.Current.longitude;
                _s.MoveNext();
                i++;
            }

        }

        public void HistoriaSelected(object sender, EventArgs e)//triggered when selected
        {
            AppVariables.location = new Location();
            string _str = "", _temp = "";

            _str = (sender as ListView).SelectedItem.ToString();

            bool flag = true;
            int index = 0;
            while (flag)//gets index of cell and item in database
            {
                if (_str[index] != '.')
                    _temp += _str[index];
                else
                    flag = false;
                index++;
            }
            AppVariables.location.Longitude = AppVariables.coordsList[int.Parse(_temp) - 1].longitude;//passes new coordinates
            AppVariables.location.Latitude = AppVariables.coordsList[int.Parse(_temp) - 1].latitude;
        }
    }
}