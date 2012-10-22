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

namespace CWRUtility
{
    public partial class NextBus : PhoneApplicationPage
    {
        List<string> routes;
        List<string> clDirections;
        List<string> clStops_cs;
        List<string> clStops_cma;
        List<string> csDirections;
        List<string> csStops;
        List<string> esnDirections;
        List<string> esnStops_cs;
        List<string> esnStops_l46;
        List<string> essDirections;
        List<string> essStops;
        public NextBus()
        {
            routes = new List<string>(){"Circle Link", "Commuter Shuttle", "Evening Shuttle North", "Evening Shuttle South"};
            clDirections = new List<string>() { "To Circle Station", "To Cleveland Museum of Art" };
            clStops_cs = new List<string>() {"Cleveland Museum of Art - Departure", "East Blvd (Law School)", "Juniper Central South", "Juniper East South",
                "E115th Euclid West", "Lot 46 South", "Euclid Ave - Up Town", "Ford Lot 419", "Euclid Ave (Church of the Covenant)", "Adelbert Lot 13A West",
                "Adelbert Lot 13", "Adelbert Kent Hale Smith", "Adelbert 1-2-1 West", "Adelbert Lot 47", "Murray Hill & Fairchild", "Circle Station - Arrival"};
            csDirections = new List<string>() { "Continuous Loop" };
            csStops = new List<string>() { "Lerner Towers", "Thwing Lot", "VIC Lot", "DeGrace", "Adelbert 1-2-1 West", "Wolstein Research Building", "Foley Building",
                "Lot 46 South", "Bellflower at E115th Street", "Wolstein Hall", "WSOM" };
            esnDirections = new List<string>() { "To Circle Station", "To Lot 46" };
            essDirections = new List<string>() { "Continuous Loop" };
            essStops = new List<string>() { "Fribley Commons", "Murray Hill & Cornell", "Overlook Rd & Edgehill", "Coventry & Overlook", "Euclid Hts & Coventry",
                "Euclid Hts & Edgehill", "Euclid Hts & Lennox", "Lennox & Cedar", "Carlton Road", "Euclid Hts & Cedar", "Murray Hill & Glenwood", "Circle Station - Departure" };
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(NextBus_Loaded);
        }

        private void NextBus_Loaded(object sender, RoutedEventArgs e)
        {
            routePicker.ItemsSource = routes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetHtml("http://www.nextbus.com/predictor/fancyNewPredictionLayer.jsp?a=case-western&r=eveningnorth&d=2circle&s=adellot13aw&ts=adellot13");

            // extract hrefs

            
        }
        private void GetHtml(string uri)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(new Uri(uri));
            goButton.IsEnabled = false;
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Stream data = e.Result as Stream;
            StreamReader reader = new StreamReader(data);
            HtmlDocument busPredictions = new HtmlDocument();
            busPredictions.Load(reader);
            data.Close();
            reader.Close();
            PHtml(busPredictions);
        }

        private void PHtml(HtmlDocument busPredictions)
        {
            List<string> predictions = new List<string>();
            predictions = extractPredictions(busPredictions);
            pred1.Text = predictions[0];
            pred2.Text = predictions[1];
            pred3.Text = predictions[2];
            //favBox.ItemsSource = predictions;
            goButton.IsEnabled = true;
        }

        private List<string> extractPredictions(HtmlDocument busPredictions)
        {
            if (busPredictions != null)
            {
                List<string> bpTags = new List<string>();

                foreach (HtmlNode link in busPredictions.DocumentNode.SelectNodes("//div"))
                {
                    //HtmlAttribute att = link.Attributes["div"];
                    bpTags.Add(link.InnerText);
                }

                List<string> parsedStrings = new List<string>();

                foreach (string s in bpTags)
                {
                    parsedStrings.Add(":"+s.Substring(6));
                }
                parsedStrings.Remove(parsedStrings.Last());

                return parsedStrings;
            }
            else
                throw new ArgumentNullException();
        }

        private void routePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dirPicker.IsEnabled = true;
            switch ((string)routePicker.SelectedItem)
            {
                case "Circle Link":
                    dirPicker.ItemsSource = clDirections;
                    break;
                case "Commuter Shuttle":
                    dirPicker.ItemsSource = csDirections;
                    break;
                case "Evening Shuttle North":
                    dirPicker.ItemsSource = esnDirections;
                    break;
                case "Evening Shuttle South":
                    dirPicker.ItemsSource = essDirections;
                    break;
            }
                
        }

        private void dirPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stopPicker.IsEnabled = true;
            switch ((string)routePicker.SelectedItem)
            {
                case "Circle Link":
                    switch ((string)dirPicker.SelectedItem)
                    {
                        case "To Circle Station":
                            stopPicker.ItemsSource = clStops_cs;
                            break;
                        case "To Cleveland Museum of Art":
                            stopPicker.ItemsSource = clStops_cma;
                            break;
                    }
                    break;
                case "Commuter Shuttle":
                    stopPicker.ItemsSource = csStops;
                    break;
                case "Evening Shuttle North":
                    switch ((string)dirPicker.SelectedItem)
                    {
                        case "To Circle Station":
                            stopPicker.ItemsSource = esnStops_cs;
                            break;
                        case "To Lot 46":
                            stopPicker.ItemsSource = esnStops_l46;
                            break;
                    }
                    break;
                case "Evening Shuttle South":
                    stopPicker.ItemsSource = essStops;
                    break;
            }
        }
    }
}