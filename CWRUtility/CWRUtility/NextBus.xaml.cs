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
using HtmlAgilityPack;
using System.IO.IsolatedStorage;
using System.Windows.Threading;

namespace CWRUtility
{
    public partial class NextBus : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        Dictionary<string, Dictionary<string, Dictionary<string, Uri>>> buses;
        DispatcherTimer timer;
        Uri currentUri;

        public NextBus()
        {
            createBusLists();
            InitializeComponent();
            InitializeTimer();

            if (!(settings.Contains("nbFavorites")))
            {
                settings["nbFavorites"] = new List<string>();
            }
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(30);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (!(currentUri == null))
            {
                GetHtml(currentUri);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New || e.NavigationMode == System.Windows.Navigation.NavigationMode.Refresh)
            {
                routePicker.ItemsSource = buses.Keys;
                SetDefault();
            }
            base.OnNavigatedTo(e);
        }

        private void SetDefault()
        {
            if (!String.IsNullOrEmpty((string)settings["nbDefault"]))
            {
                string[] nbDef = ((string)settings["nbDefault"]).Split('!');
                routePicker.SelectedItem = nbDef[0];
                dirPicker.ItemsSource = buses[nbDef[0]].Keys;
                dirPicker.SelectedItem = nbDef[1];
                stopPicker.ItemsSource = buses[nbDef[0]][nbDef[1]].Keys;
                stopPicker.SelectedItem = nbDef[2];
                getBusPrediction();
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
            {
                timer.Stop();
            }
        }

        #region bus dictionary creation

        private void createBusLists()
        {
            buses = new Dictionary<string, Dictionary<string, Dictionary<string, Uri>>>();
            List<string> routes = new List<string>() { "Circle Link", "Commuter Shuttle", "Evening Shuttle North", "Evening Shuttle South" };
            foreach (string route in routes)
            {
                buses.Add(route, new Dictionary<string, Dictionary<string, Uri>>());
                addDirections(route);
            }
        }

        private void addDirections(string route)
        {
            List<string> directions = new List<string>();
            switch (route)
            {
                case "Circle Link":
                    directions = new List<string>() { "To Circle Station", "To Cleveland Museum of Art" };
                    break;
                case "Commuter Shuttle":
                    directions = new List<string>() { "Continuous Loop" };
                    break;
                case "Evening Shuttle North":
                    directions = new List<string>() { "To Circle Station", "To Lot 46" };
                    break;
                case "Evening Shuttle South":
                    directions = new List<string>() { "Continuous Loop" };
                    break;
            }
            foreach (string direction in directions)
            {
                buses[route].Add(direction, getStopDict(route, direction));
            }
        }

        private Dictionary<string, Uri> getStopDict(string route, string dir)
        {
            string nbRoute = "";
            string nbDirection = "";
            List<string> stops = new List<string>();
            List<string> uris = new List<string>();
            switch (route)
            {
                case "Circle Link":
                    nbRoute = "circlelink";
                    switch (dir)
                    {
                        case "To Circle Station":
                            nbDirection = "2circle";
                            stops = new List<string>() { "Cleveland Museum of Art - Departure", "East Blvd (Law School)", "Juniper Central South", "Juniper East South",
                                "E115th Euclid West", "Lot 46 South", "Euclid Ave - Up Town", "Ford Lot 419", "Euclid Ave (Church of the Covenant)", "Adelbert Lot 13A West",
                                "Adelbert Lot 13", "Adelbert Kent Hale Smith", "Adelbert 1-2-1 West", "Adelbert Lot 47", "Murray Hill & Fairchild", "Circle Station - Arrival" };
                            uris = new List<string>() { "artmus_d", "lawsch", "junipcs", "junipes", "115ew", "lot46_c", "euclup", "flot419", "euclchur", "adellot13aw",
                                "adellot13", "adelsmith", "adel121w", "adellot47", "murrayhf", "circlestat_a", "circlestat_a" };
                            break;
                        case "To Cleveland Museum of Art":
                            nbDirection = "2artmus";
                            stops = new List<string>() { "Circle Station - Departure", "Murray Hill & Glenwood", "Murray Hill & Adelbert", "Adelbert Lot 13A East",
                                "East Blvd Lot 29 East", "East Blvd Central East", "Bellflower CIA South", "Bellflower PBL South", "Ford & Juniper", "CIM", "Hazel North",
                                "Magnolia North", "VA Hospital", "Natural History Museum", "Cleveland Museum of Art - Arrival" };
                            uris = new List<string>(){ "circlestat_d", "murrayhg", "murrayadel", "adel121e", "adellot13ae", "eastlot29e", "eastce", "belcias", "belpbls", "junipw",
                                "cim", "hazeln", "magnolian", "vahosp", "nhm", "artmus_a", "artmus_a" };
                            break;
                    }
                    break;
                case "Commuter Shuttle":
                    nbRoute = "commuter";
                    switch (dir)
                    {
                        case "Continuous Loop":
                            nbDirection = "loop";
                            stops = new List<string>() { "Lerner Towers", "Thwing Lot", "VIC Lot", "DeGrace", "Adelbert 1-2-1 West", "Wolstein Research Building", "Foley Building",
                                "Lot 46 South", "Bellflower at E115th Street", "Wolstein Hall", "WSOM" };
                            uris = new List<string>(){ "lernertow", "thwing", "viclot", "dgra", "adel121w", "wrb", "folebuil", "lot46_c", "belle115", "wolshall", "wsom", "wsom" };
                            break;
                    }
                    break;
                case "Evening Shuttle North":
                    nbRoute = "eveningnorth";
                    switch (dir)
                    {
                        case "To Circle Station":
                            nbDirection = "2circle";
                            stops = new List<string>() { "Lot 46 South", "East 117th & Euclid Ave", "119th - Little Italy", "Thwing Center", "Adelbert Lot 13A West",
                                "Adelbert Lot 13", "Adelbert Kent Hale Smith", "Adelbert 1-2-1 West", "Adelbert Lot 47", "Murray Hill & Fairchild", "Lot 44", "Circle Station - Arrival" };
                            uris = new List<string>(){ "lot46_c", "117euclid", "119italy", "thwing", "adellot13aw", "adellot13", "adelsmith", "adel121w", "adellot47", "murrayhf", "lot44",
                                "circlestat_a", "circlestat_a" };
                            break;
                        case "To Lot 46":
                            nbDirection = "2lot46s";
                            stops = new List<string>() { "Circle Station - Departure", "Murray Hill & Glenwood", "Fribley Commons", "Murray Hill & Adelbert", "Adelbert 1-2-1 East",
                                "Adelbert Pathology", "Adelbert Lot 13A East", "East Blvd Lot 29 East", "Ford & Bellflower North", "Ford & Juniper", "Juniper Central South", "East 115th & Bellflower",
                                "Village Stop A", "Village Stop B" };
                            uris = new List<string>(){ "circlestat_d", "murrayhg", "fribleyc", "murrayadel", "adel121e", "adelpath", "adellot13ae", "eastlot29e", "fordbell", "junipw", "junipcs",
                                "115bell", "villa", "villb", "villb" };
                            break;
                    }
                    break;
                case "Evening Shuttle South":
                    nbRoute = "eveningsouth";
                    switch (dir)
                    {
                        case "Continuous Loop":
                            nbDirection = "loop";
                            stops = new List<string>() { "Fribley Commons", "Murray Hill & Cornell", "Overlook Rd & Edgehill", "Coventry & Overlook", "Euclid Hts & Coventry",
                                "Euclid Hts & Edgehill", "Euclid Hts & Lennox", "Lennox & Cedar", "Carlton Road", "Euclid Hts & Cedar", "Murray Hill & Glenwood", "Circle Station - Departure" };
                            uris = new List<string>(){ "fribleyc", "murrayhc", "overedge", "coveover", "euccoven", "eucedge", "eucderby", "lennceda", "carlton", "euclidhts", "murrayhg",
                                "circlestat_d", "circlestat_d" };
                            break;
                    }
                    break;
            }
            return generateStopDict(nbRoute, nbDirection, stops, uris);
        }

        private Dictionary<string, Uri> generateStopDict(string nbRoute, string nbDirection, List<string> stops, List<string> uri_parts)
        {
            Dictionary<string, Uri> stopDict = new Dictionary<string, Uri>();
            for (int i = 0; i < stops.Count; i++)
            {
                stopDict.Add(stops[i], new Uri("http://www.nextbus.com/predictor/fancyNewPredictionLayer.jsp?a=case-western&r="+nbRoute+"&d="+nbDirection+"&s="
                    + uri_parts[i] + "&ts=" + uri_parts[i + 1]));
            }
            return stopDict;
        }

        #endregion

        #region selection changed events

        private void routePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(routePicker.SelectedIndex == -1))
            {
                dirPicker.IsEnabled = true;
                string route = (string)routePicker.SelectedItem;
                if (dirPicker.ItemsSource != buses[route].Keys)
                {
                    dirPicker.ItemsSource = null;
                    dirPicker.ItemsSource = buses[route].Keys;
                }
            }
        }

        private void dirPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(dirPicker.SelectedIndex == -1))
            {
                stopPicker.IsEnabled = true;
                string route = (string)routePicker.SelectedItem;
                string direction = (string)dirPicker.SelectedItem;
                if (stopPicker.ItemsSource != buses[route][direction].Keys)
                {
                    stopPicker.ItemsSource = null;
                    stopPicker.ItemsSource = buses[route][direction].Keys;
                }
            }
        }

        #endregion

        #region button click and prediction scraping

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getBusPrediction();
        }
        
        private void getBusPrediction()
        {
            string route = (string)routePicker.SelectedItem;
            string direction = (string)dirPicker.SelectedItem;
            string stop = (string)stopPicker.SelectedItem;
            predTextBlock.Text = stop;
            currentUri = buses[route][direction][stop];
            LockUI();
            GetHtml(currentUri);
        }

        private void GetHtml(Uri stopUri)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(stopUri);
            ProgressBar.IsVisible = true;
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Stream data = e.Result as Stream;
            StreamReader reader = new StreamReader(data);
            HtmlDocument busPredictions = new HtmlDocument();
            busPredictions.Load(reader);
            data.Close();
            reader.Close();
            DisplayPredictions(busPredictions);
        }

        private void DisplayPredictions(HtmlDocument busPredictions)
        {
            List<string> predictions = extractPredictions(busPredictions);
            if (predictions != null)
            {
                pred1.Visibility = System.Windows.Visibility.Visible;
                pred2.Width = 144;
                pred2.FontSize = 48;
                pred3.Visibility = System.Windows.Visibility.Visible;
                if (predictions.Count == 3)
                {
                    pred1.Text = predictions[0];
                    pred2.Text = predictions[1];
                    pred3.Text = predictions[2];
                }
                else if (predictions.Count == 2)
                {
                    pred1.Text = "Arr.";
                    pred2.Text = predictions[0];
                    pred3.Text = predictions[1];
                }
                else
                {
                    MessageBox.Show("Error getting predictions, please try again.", "Error", MessageBoxButton.OK);
                }
            }
            else
            {
                pred1.Visibility = System.Windows.Visibility.Collapsed;
                pred3.Visibility = System.Windows.Visibility.Collapsed;
                pred2.Width = 432;
                pred2.FontSize = 36;
                pred2.Text = "No Prediction Available";
            }
            UnlockUI();
            ProgressBar.IsVisible = false;
        }

        private List<string> extractPredictions(HtmlDocument busPredictions)
        {
            if (busPredictions != null)
            {
                List<string> bpTags = new List<string>();

                if (busPredictions.DocumentNode.SelectNodes("//p").Count == 2)
                {
                    return null;
                }

                foreach (HtmlNode link in busPredictions.DocumentNode.SelectNodes("//div"))
                {
                    //HtmlAttribute att = link.Attributes["div"];
                    if (link.InnerText == null)
                    {
                        System.Diagnostics.Debug.WriteLine(busPredictions);
                    }
                    bpTags.Add(link.InnerText);
                }

                List<string> parsedStrings = new List<string>();

                foreach (string s in bpTags)
                {
                    parsedStrings.Add(":" + s.Substring(6));
                }
                parsedStrings.Remove(parsedStrings.Last());

                if (parsedStrings.Count == 0)
                {
                    return null;
                }
                else
                {
                    return parsedStrings;
                }
            }
            return null;
        }

        #endregion

        private void favButton_Click(object sender, EventArgs e)
        {
            string route = (string)routePicker.SelectedItem;
            string direction = (string)dirPicker.SelectedItem;
            string stop = (string)stopPicker.SelectedItem;
            string uri = buses[route][direction][stop].ToString();
            string fav = route + '!' + direction + '!' + stop + '!' + uri;
            settings["nbDefault"] = fav;
            MessageBox.Show(stop + " set as default.", "Default Set", MessageBoxButton.OK);
            /*
            if (((List<string>)settings["nbFavorites"]).Contains(fav))
            {
                MessageBox.Show("Favorite already exists", "Error", MessageBoxButton.OK);
            }
            else
            {
                ((List<string>)settings["nbFavorites"]).Add(fav);
            }
            */
        }

        private void LockUI()
        {
            goButton.IsEnabled = false;
            routePicker.IsEnabled = false;
            dirPicker.IsEnabled = false;
            stopPicker.IsEnabled = false;
        }

        private void UnlockUI()
        {
            goButton.IsEnabled = true;
            routePicker.IsEnabled = true;
            dirPicker.IsEnabled = true;
            stopPicker.IsEnabled = true;
        }
    }
}