using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Services;

namespace App_Comps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class All : ContentPage
	{
        public All()
        {
            InitializeComponent();
            archiveList.ItemSelected += OnSelect;//when location is selected from list it triggers this method
            archiveList.ItemsSource = AppVariables.coordsList;//takes coordinates from database as source
        }

        void OnClick(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "GoBackSend");
        }

        void OnDelete(object sender, EventArgs e)//deletes location from list and database
        {
            var item = ((MenuItem)sender);
            var coord = (Coords)item.CommandParameter;
            AppVariables.coordsList.Remove(coord);//first it's removed from listview so can't be seen anymore
            AppVariables.favouritesList.Remove(coord);
            AppVariables.db.Query<ArchiveDatabase>("DELETE FROM Registry WHERE _id = ?", coord.Id);//deletes location in database by it's Id
        }

        void OnFavourite(object sender, EventArgs e)//when setted to be favourite
        {
            var item = ((MenuItem)sender);
            var coord = (Coords)item.CommandParameter;
            if (!coord.IsFav)
            {
                coord.IsFav = true;
                AppVariables.favouritesList.Add(coord);
                AppVariables.db.Query<ArchiveDatabase>("UPDATE Registry SET IsFavourite = 1 WHERE _id = ?", coord.Id);
            }
        }

        async void OnChange(object sender, EventArgs e)//when note change
        {
            Popup popup = new Popup();
            var item = ((MenuItem)sender);
            var coord = (Coords)item.CommandParameter;
            int _index = AppVariables.coordsList.IndexOf(coord);

            MessagingCenter.Subscribe<Popup, string>(this, "noteSend", (sender2, f) => {
                AppVariables.db.Query<ArchiveDatabase>("UPDATE Registry SET Note = ? WHERE _id = ?", f, coord.Id);
                AppVariables.coordsList[_index].note = f;
            });
            MessagingCenter.Subscribe<Popup>(this, "closeSend", (sender2) => { PopupNavigation.Instance.PopAsync(false); });
            await PopupNavigation.Instance.PushAsync(popup);
        }

        void OnSelect(object sender, EventArgs e)//triggered when cell selected
        {
            AppVariables.location = new Location();

            var item = (sender as ListView).SelectedItem;
            AppVariables.location.Longitude = (item as Coords).longitude;
            AppVariables.location.Latitude = (item as Coords).latitude;
            MessagingCenter.Send(this, "gobackSend");//tell main page to swipe us back

            Trytofind trytofind = new Trytofind();
            trytofind.Check();//checks if there's already destination and calculates distance and new angle for arrow; find it in EntryPage.xaml.cs
        }
    }
}