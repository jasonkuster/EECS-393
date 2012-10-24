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
using System.IO.IsolatedStorage;
using System.IO;
using HtmlAgilityPack;

namespace CWRUtility
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        //This is the file in which we'll put all the main page logic.
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!(settings.Contains("nbDefault")))
            {
                settings["nbDefault"] = "";
            }
            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            if (!String.IsNullOrEmpty((string)settings["nbDefault"]))
            {
                string[] nbDefault = ((string)settings["nbDefault"]).Split('!');
                nbDef.Text = nbDefault[2];
                GetHtml(new Uri(nbDefault[3]));
            }
        }

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Map.xaml", UriKind.RelativeOrAbsolute));
        }

        private void NextBus_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NextBus.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Directory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Directory.xaml", UriKind.RelativeOrAbsolute));
        }

        #region Nextbus Scraper

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
            ParseHtml(busPredictions);
        }

        private void ParseHtml(HtmlDocument busPredictions)
        {
            List<string> predictions = new List<string>();
            predictions = extractPredictions(busPredictions);
            if (predictions != null)
            {
                nbPred1.Visibility = System.Windows.Visibility.Visible;
                nbPred2.Width = 80;
                nbPred2.FontSize = 37.333;
                nbPred3.Visibility = System.Windows.Visibility.Visible;
                if (predictions.Count == 3)
                {
                    nbPred1.Text = predictions[0];
                    nbPred2.Text = predictions[1];
                    nbPred3.Text = predictions[2];
                }
                else if (predictions.Count == 2)
                {
                    nbPred1.Text = "Arr.";
                    nbPred2.Text = predictions[0];
                    nbPred3.Text = predictions[1];
                }
            }
            else
            {
                nbPred1.Visibility = System.Windows.Visibility.Collapsed;
                nbPred3.Visibility = System.Windows.Visibility.Collapsed;
                nbPred2.Width = 240;
                nbPred2.FontSize = 24;
                nbPred2.Text = "No Prediction Available";
            }
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
                    bpTags.Add(link.InnerText);
                }

                List<string> parsedStrings = new List<string>();

                foreach (string s in bpTags)
                {
                    parsedStrings.Add(":" + s.Substring(6).Replace(" ",""));
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
    }
}