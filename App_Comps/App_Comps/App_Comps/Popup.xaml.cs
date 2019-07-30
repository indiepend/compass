using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//it's based on rotorgames plugin available at https://github.com/rotorgames/Rg.Plugins.Popup
namespace App_Comps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Popup : Rg.Plugins.Popup.Pages.PopupPage
    {
		public Popup ()
		{
			InitializeComponent ();
		}

        void OnClick(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "noteSend", newNote.Text);
            MessagingCenter.Send(this, "closeSend");
        }
	}
}