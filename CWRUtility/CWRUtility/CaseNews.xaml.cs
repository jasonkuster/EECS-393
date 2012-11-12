using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Phone.Tasks;

namespace CWRUtility
{
    public partial class CaseNews : PhoneApplicationPage
    {
        ListBox feedListBox;
        string feedUri;
        // Constructor
        public CaseNews()
        {
            InitializeComponent();            
        }
        // Event handler which runs after the feed is fully downloaded.
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // Showing the exact error message is useful for debugging. In a finalized application, 
                    // output a friendly and applicable string to the user instead. 
                    MessageBox.Show(e.Error.Message);
                });
            }
            else
            {
                // Save the feed into the State property in case the application is tombstoned. 
                this.State["feed"] = e.Result;

                UpdateFeedList(e.Result);
            }
        }

        // This method determines whether the user has navigated to the application after the application was tombstoned.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.
            if (this.State.ContainsKey("feed"))
            {
                // Get the feed again only if the application was tombstoned, which means the ListBox will be empty.
                // This is because the OnNavigatedTo method is also called when navigating between pages in your application.
                // You would want to rebind only if your application was tombstoned and page state has been lost. 
                if (feedListBox.Items.Count == 0)
                {
                    refreshFeed_Click(null, null);
                }
            }
        }

        // This method sets up the feed and binds it to our ListBox. 
        private void UpdateFeedList(string feedXML)
        {
            // Load the feed into a SyndicationFeed instance.
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
           
            // In Windows Phone OS 7.1, WebClient events are raised on the same type of thread they were called upon. 
            // For example, if WebClient was run on a background thread, the event would be raised on the background thread. 
            // While WebClient can raise an event on the UI thread if called from the UI thread, a best practice is to always 
            // use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Bind the list of SyndicationItems to our ListBox.
                feedListBox.ItemsSource = feed.Items;
            });

            ProgressBar.IsVisible = false;
        }

        // The SelectionChanged handler for the feed items 
        private void feedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                // Get the SyndicationItem that was tapped.
                SyndicationItem sItem = (SyndicationItem)listBox.SelectedItem;

                // Set up the page navigation only if a link actually exists in the feed item.
                if (sItem.Links.Count > 0)
                {
                    // Get the associated URI of the feed item.
                    Uri uri = sItem.Links.FirstOrDefault().Uri;
                    NavigationService.Navigate(new Uri(string.Format("/NewsStoryPage.xaml?storyUri={0}", uri), UriKind.RelativeOrAbsolute));
                }
            }
        }

        private void refreshFeed_Click(object sender, EventArgs e)
        {
            ProgressBar.IsVisible = true;
            // WebClient is used instead of HttpWebRequest in this code sample because 
            // the implementation is simpler and easier to use, and we do not need to use 
            // advanced functionality that HttpWebRequest provides, such as the ability to send headers.
            WebClient webClient = new WebClient();
            // Subscribe to the DownloadStringCompleted event prior to downloading the RSS feed.
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

            // Download the RSS feed. DownloadStringAsync was used instead of OpenStreamAsync because we do not need 
            // to leave a stream open, and we will not need to worry about closing the channel. 

            webClient.DownloadStringAsync(new System.Uri(feedUri));
            
        }

        private void Pivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            feedListBox = e.Item == Daily ? feedListBox2 : feedListBox1;
            feedUri = e.Item == Daily ? "http://cwru-daily.com/news/?feed=rss2" : "http://www.thecwruobserver.com/feed/";
            refreshFeed_Click(null, null);
        }
    }
}