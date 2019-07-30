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
	public partial class Favourite : ContentPage
	{
		public Favourite ()
		{
			InitializeComponent ();
            archiveList.ItemSelected += OnSelect;//when location is selected from list it triggers this method
            archiveList.ItemsSource = AppVariables.favouritesList;//takes coordinates from database as source
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
            AppVariables.favouritesList.Remove(coord);//removes record from favourite list
            AppVariables.db.Query<ArchiveDatabase>("DELETE FROM Registry WHERE _id = ?", coord.Id);//deletes location in database by it's Id
        }

        void OnUnfavourite(object sender, EventArgs e)//when unfavourited(?) 
        {
            var item = ((MenuItem)sender);
            var coord = (Coords)item.CommandParameter;
            AppVariables.favouritesList.Remove(coord);//removes record from favourite list
            AppVariables.db.Query<ArchiveDatabase>("UPDATE Registry SET IsFavourite = 0 WHERE _id = ?", coord.Id);//sets isfavourite as false
        }

        async void OnChange(object sender, EventArgs e)//when note change
        {
            Popup popup = new Popup();
            var item = ((MenuItem)sender);
            var coord = (Coords)item.CommandParameter;
            int _index = AppVariables.favouritesList.IndexOf(coord);
            //first we need to describe what data do we need to receive from this popup
            MessagingCenter.Subscribe<Popup, string>(this, "noteSend", (sender2, f) => {
                AppVariables.db.Query<ArchiveDatabase>("UPDATE Registry SET Note = ? WHERE _id = ?", f, coord.Id);//will set new note in database
                //if(f=="")
                    //AppVariables.favouritesList[_index].note = null;//if there's no note or it will be deleted
                //else
                    AppVariables.favouritesList[_index].note = f;//changes note in list
            });
            MessagingCenter.Subscribe<Popup>(this, "closeSend", (sender2) => { PopupNavigation.Instance.PopAsync(false); });//now we subscribe for exit
            await PopupNavigation.Instance.PushAsync(popup);//finally we create our popup
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