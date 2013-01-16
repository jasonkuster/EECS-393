using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class Menus : PhoneApplicationPage
    {
        string[] Leut = new string[7];
       // string[] Frib = new string[7];
        string ToParse = "";
        ListBox feedListBox;
        string feedUri;
        DateTime date3 = DateTime.Today;
        DateTime date4 = DateTime.Today;
        int WhatToShow = 0;
        Boolean nono = true;

        public Menus()
        {
            
            InitializeComponent();

            DateBox.FontSize = 40; //the name should be noticiably larger than the other 3 data feilds
            //DateBox.Margin = new Thickness(12, 12, 12, 12);
            List<String> Days = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            DateBox.ItemsSource = Days;
            
            if (date3.DayOfWeek == DayOfWeek.Sunday)
                WhatToShow = 6;
            if (date3.DayOfWeek == DayOfWeek.Monday)
                WhatToShow = 0;
            if (date3.DayOfWeek == DayOfWeek.Tuesday)
                WhatToShow = 1;
            if (date3.DayOfWeek == DayOfWeek.Wednesday)
                WhatToShow = 2;
            if (date3.DayOfWeek == DayOfWeek.Thursday)
                WhatToShow = 3;
            if (date3.DayOfWeek == DayOfWeek.Friday)
                WhatToShow = 4;
            if (date3.DayOfWeek == DayOfWeek.Saturday)
                WhatToShow = 5;
            DateBox.SelectedItem = Days.ElementAt(WhatToShow);

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
                    DateBox.IsEnabled = true;
                    ProgressBar.IsVisible = false;
                
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
            ToParse = (string)stringReader.ReadToEnd();
            char[] CharArr = ToParse.ToCharArray();
            int date = -1;
            int comment = 0;
            for (int i = 0; i < CharArr.Length; i++)
            {
                char Un = CharArr[i]; //make a temporary char named 'Un' for 'Unhandled' at index i in the char array
                if (Un.Equals('<'))
                {
                    if (CharArr[i + 1] == 't' && CharArr[i + 2] == 'i' && CharArr[i + 3] == 't' && CharArr[i + 4] == 'l' && CharArr[i + 5] == 'e' && CharArr[i + 6] == '>')
                    {
                        int x = i + 7;
                        while (CharArr[x] != '<')
                        {
                            x++;
                        }
                        i = x + 7;
                        date = date + 1;
                    }
                    else if (CharArr[i + 1] == 'd' && CharArr[i + 2] == 'e' && CharArr[i + 3] == 's' && CharArr[i + 4] == 'c' && CharArr[i + 5] == 'r' && CharArr[i + 6] == 'i')
                    {
                        int g = i + 13;
                        char debugger = CharArr[i];
                        string realTemp = "";
                        while (!(CharArr[g + 1] == '/' && CharArr[i + 2] == 'd' && CharArr[i + 3] == 'e' && CharArr[i + 4] == 's' && CharArr[i + 5] == 'c' && CharArr[i + 6] == 'r'))
                        {
                            char debuggery = CharArr[g];
                            if (CharArr[g] == '&' && CharArr[g + 1] == 'l' && CharArr[g + 2] == 't' && CharArr[g + 3] == ';' && CharArr[g + 4] == 'h' && CharArr[g + 5] == '3')
                            {
                                realTemp = realTemp + "#";
                                g = g + 10;
                                while (!(CharArr[g] == '&' && CharArr[g + 1] == 'l' && CharArr[g + 2] == 't' && CharArr[g + 3] == ';' && CharArr[g + 4] == '/' && CharArr[g + 5] == 'h' && CharArr[g + 6] == '3'))
                                {
                                    realTemp = realTemp + CharArr[g];
                                    g++;
                                }
                                realTemp = realTemp + "#";
                            }

                            else if (CharArr[g] == '&' && CharArr[g + 1] == 'l' && CharArr[g + 2] == 't' && CharArr[g + 3] == ';' && CharArr[g + 4] == 'h' && CharArr[g + 5] == '4')
                            {
                                realTemp = realTemp + "=";
                                g = g + 10;
                                while (!(CharArr[g] == '&' && CharArr[g + 1] == 'a' && CharArr[g + 2] == 'm' && CharArr[g + 3] == 'p' && CharArr[g + 4] == ';' && CharArr[g + 5] == 'n' && CharArr[g + 6] == 'b'))
                                {
                                    if (CharArr[g] == '[')
                                    {
                                        while (CharArr[g] != ']')
                                        {
                                            g++;
                                        }
                                        g++;
                                    }
                                    char dumb = CharArr[g];
                                    char dumbb = CharArr[g + 1];
                                    char dumbbb = CharArr[g + 2];
                                    char dumbbbb = CharArr[g + 3];
                                    realTemp = realTemp + CharArr[g];
                                    g = g + 1;
                                }
                                realTemp = realTemp + "=";
                            }

                            else if (CharArr[g] == '&' && CharArr[g + 1] == 'l' && CharArr[g + 2] == 't' && CharArr[g + 3] == ';' && CharArr[g + 4] == 'p')
                            {
                                realTemp = realTemp + "}";
                                g = g + 9;
                                while (!(CharArr[g] == '&' && CharArr[g + 1] == 'l' && CharArr[g + 2] == 't' && CharArr[g + 3] == ';' && CharArr[g + 4] == '/' && CharArr[g + 5] == 'p'))
                                {
                                    realTemp = realTemp + CharArr[g];
                                    g++;
                                }
                                realTemp = realTemp + "}";
                            }
                            else
                                g++;
                            i = g;

                        }
                        if (date > 0 && date <= 7)
                            Leut[date - 1] = realTemp;
                    }
                }


            }
            // In Windows Phone OS 7.1 and higher, WebClient events are raised on the same type of thread they were called upon. 
            // For example, if WebClient was run on a background thread, the event would be raised on the background thread. 
            // While WebClient can raise an event on the UI thread if called from the UI thread, a best practice is to always 
            // use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
            //Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            // Bind the list of SyndicationItems to our ListBox.
            //feedListBox.ItemsSource = feed.Items;
            //loadFeedButton.Content = "Refresh Feed";
            //});
            UpdateLunchables();
        }

        private void UpdateLunchables()
        {
            //for (int y = 0; y < Leut.Length; y++)
            //{
              //  TextBlock NM = new TextBlock();
           /* int y = 0;
            if (date3.DayOfWeek == DayOfWeek.Sunday)
                y = 6;
            if (date3.DayOfWeek == DayOfWeek.Monday)
                y = 0;
            if (date3.DayOfWeek == DayOfWeek.Tuesday)
                y = 1;
            if (date3.DayOfWeek == DayOfWeek.Wednesday)
                y = 2;
            if (date3.DayOfWeek == DayOfWeek.Thursday)
                y = 3;
            if (date3.DayOfWeek == DayOfWeek.Friday)
                y = 4;
            if (date3.DayOfWeek == DayOfWeek.Saturday)
                y = 5;*/
            for (int k = 0; k < Leut[WhatToShow].Length; k++)
                {
                    TextBlock NM = new TextBlock();
                    if (Leut[WhatToShow].ElementAt(k) == '#')
                    {
                       
                        k = k + 1;
                        string toBlock = "";
                        while (!(Leut[WhatToShow].ElementAt(k) == '#') && (k != Leut[WhatToShow].Length))
                        {
                            toBlock = toBlock + Leut[WhatToShow].ElementAt(k);
                            k = k + 1;
                        }
                        if (toBlock != "")
                        {
                            NM.Text = toBlock;
                            NM.FontSize = 45;
                            NM.Foreground = Application.Current.Resources["PhoneForegroundBrush"] as Brush;
                            NM.TextAlignment = TextAlignment.Center;
                            NM.TextWrapping = TextWrapping.Wrap;
                            NM.Margin = new Thickness(128, 12, 12, 12);
                            this.feedListBox.Items.Add(NM);
                        }
                    }
                    else if (Leut[WhatToShow].ElementAt(k) == '=')
                    {
                        k = k + 2;
                        string toBlock = "";
                        while (!(Leut[WhatToShow].ElementAt(k) == '=') && (k != Leut[WhatToShow].Length))
                        {
                            toBlock = toBlock + Leut[WhatToShow].ElementAt(k);
                            k = k + 1;
                        }
                        if (toBlock != "")
                        {
                        NM.Text = toBlock;
                        NM.TextAlignment = TextAlignment.Left;
                        NM.FontSize = 35;
                        NM.Foreground =  new SolidColorBrush((Color) Application.Current.Resources["PhoneAccentColor"]);
                        NM.TextWrapping = TextWrapping.Wrap;
                        //NM.Width = 465;
                        NM.Margin = new Thickness(12, 12, 12, 12);
                        this.feedListBox.Items.Add(NM);
                        }
                    }
                    else if (Leut[WhatToShow].ElementAt(k) == '}')
                    {
                        k = k + 1;
                        string toBlock = "";
                        while (!(Leut[WhatToShow].ElementAt(k) == '}') && (k != Leut[WhatToShow].Length))
                        {
                            toBlock = toBlock + Leut[WhatToShow].ElementAt(k);
                            k = k + 1;
                        }
                        if (toBlock != "")
                        {
                            NM.Text = toBlock;
                            NM.FontSize = 25;
                            NM.TextAlignment = TextAlignment.Left;
                            NM.Foreground = new SolidColorBrush(Colors.LightGray);
                            NM.TextWrapping = TextWrapping.Wrap;
                            NM.Margin = new Thickness(12, 12, 12, 12);
                            //NM.Width = 465;
                            this.feedListBox.Items.Add(NM);
                        }
                    }


                }

           // }
        }



        // The SelectionChanged handler for the feed items 
        /*
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
        }*/

        private void refreshFeed_Click(object sender, EventArgs e)
        {
            // WebClient is used instead of HttpWebRequest in this code sample because 
            // the implementation is simpler and easier to use, and we do not need to use 
            // advanced functionality that HttpWebRequest provides, such as the ability to send headers.
            WebClient webClient = new WebClient();
            // Subscribe to the DownloadStringCompleted event prior to downloading the RSS feed.
            DateBox.IsEnabled = false;
            ProgressBar.IsVisible = true;
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

            // Download the RSS feed. DownloadStringAsync was used instead of OpenStreamAsync because we do not need 
            // to leave a stream open, and we will not need to worry about closing the channel. 

            webClient.DownloadStringAsync(new System.Uri(feedUri));
        }

        public void Blank_Tap(object sender, System.Windows.Input.GestureEventArgs e)//method called in the event that the resources name is tapped and therefor the field underneath need expanding
        {
            this.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void Pivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            feedListBox = e.Item == Leutner ? feedListBox2 : feedListBox1;
            feedUri = e.Item == Leutner ? "http://www.cafebonappetit.com/rss/menu/45" : "http://www.cafebonappetit.com/rss/menu/43";
            refreshFeed_Click(null, null);
        }

        private void DateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            nono = false;
            if (Leut[WhatToShow] != null)
            {
                if (DateBox.SelectedItem == "Monday")
                    WhatToShow = 0;
                if (DateBox.SelectedItem == "Tuesday")
                    WhatToShow = 1;
                if (DateBox.SelectedItem == "Wednesday")
                    WhatToShow = 2;
                if (DateBox.SelectedItem == "Thursday")
                    WhatToShow = 3;
                if (DateBox.SelectedItem == "Friday")
                    WhatToShow = 4;
                if (DateBox.SelectedItem == "Saturday")
                    WhatToShow = 5;
                if (DateBox.SelectedItem == "Sunday")
                    WhatToShow = 6;
               // feedListBox.SelectionMode = SelectionMode.Extended;
                //feedListBox1.SelectionMode = SelectionMode.Extended;
                //feedListBox2.SelectionMode = SelectionMode.Extended;
                //feedListBox.SelectAll();
                //feedListBox.SelectedItems.Clear();
                 //   feedListBox1.SelectAll();
                    feedListBox1.Items.Clear();
                 //   feedListBox2.SelectAll();
                    feedListBox2.Items.Clear();
                //    feedListBox1.SelectionMode = SelectionMode.Single;
                 //   feedListBox2.SelectionMode = SelectionMode.Single;
                 //   feedListBox.SelectionMode = SelectionMode.Single;
                UpdateLunchables();
                
            }
            nono = true;
        }
    }
}