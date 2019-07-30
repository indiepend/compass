using Xamarin.Forms;


namespace App_Comps
{

    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            Children.Add(new EntryPage());//adds tabs to tabbed page
            Children.Add(new SearchPage());
            Children.Add(new HistoryPage());

            MessagingCenter.Subscribe<Searcher>(this, "gobackSend", (sender) => { GoToFirstPage(); });//goes to entry page when requested
            MessagingCenter.Subscribe<All>(this, "gobackSend", (sender) => { GoToFirstPage(); });
            MessagingCenter.Subscribe<Favourite>(this, "gobackSend", (sender) => { GoToFirstPage(); });
        }

        public void GoToFirstPage()//as name says
        {
            CurrentPage = Children[0];
        }
    }
}