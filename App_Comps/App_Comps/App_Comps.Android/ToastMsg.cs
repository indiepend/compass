using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

///I ain't gonna lie it's copied from:
///https://stackoverflow.com/questions/35279403/toast-equivalent-for-xamarin-forms
///praise Alex Chengalan
///he's awesome


[assembly: Xamarin.Forms.Dependency(typeof(App_Comps.Droid.MessageAndroid))]
namespace App_Comps.Droid
{
    
    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}