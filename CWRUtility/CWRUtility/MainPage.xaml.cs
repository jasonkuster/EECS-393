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
            if (!(settings.Contains("esDefault")))
            {
                settings["esDefault"] = "";
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
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            CheckNBDef();
            CheckESDef();

            base.OnNavigatedTo(e);
        }

        private void CheckESDef()
        {
            if (!String.IsNullOrEmpty((string)settings["esDefault"]))
            {
                string[] esDefault = ((string)settings["esDefault"]).Split('!');
                esLoc.Text = esDefault[0];
                esPanel.Visibility = System.Windows.Visibility.Visible;
                ScrapeHTML(new Uri(esDefault[1]));
            }
            /*else
            {
               esPanel.Visibility = System.Windows.Visibility.Collapsed;
            } */
        }

        private void CheckNBDef()
        {
            if (!String.IsNullOrEmpty((string)settings["nbDefault"]))
            {
                string[] nbDefault = ((string)settings["nbDefault"]).Split('!');
                nbDef.Text = nbDefault[2];
                nbPanel.Visibility = System.Windows.Visibility.Visible;
                GetHtml(new Uri(nbDefault[3]));
            }
            /*else
            {
                nbPanel.Visibility = System.Windows.Visibility.Collapsed;
            }*/
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

        private void eSuds_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/eSuds.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Menus_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Menus.xaml", UriKind.RelativeOrAbsolute));
        }

        private void CaseNews_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CaseNews.xaml", UriKind.RelativeOrAbsolute));
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
            if (e.Error != null)
            {
            }
            else
            {
                Stream data = e.Result as Stream;
                StreamReader reader = new StreamReader(data);
                HtmlDocument busPredictions = new HtmlDocument();
                busPredictions.Load(reader);
                data.Close();
                reader.Close();
                ParseHtml(busPredictions);
            }
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
                nbPred2.Text = "No Predictions";
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

        #region eSuds Scraper

        private void ScrapeHTML(Uri locUri)
        {
            WebClient esClient = new WebClient();
            esClient.OpenReadCompleted += new OpenReadCompletedEventHandler(esClient_OpenReadCompleted);
            esClient.OpenReadAsync(locUri);
        }

        void esClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
            }
            else
            {
                Stream data = e.Result as Stream;
                StreamReader reader = new StreamReader(data);
                HtmlDocument sudsTimes = new HtmlDocument();
                sudsTimes.Load(reader);
                data.Close();
                reader.Close();
                DisplayStates(sudsTimes);
            }
        }

        private void DisplayStates(HtmlDocument sudsTimes)
        {
            List<LaundryMachine> machines = ExtractStates(sudsTimes);
            if (machines != null)
            {
                int freeWash = 0;
                int freeDry = 0;
                foreach (LaundryMachine m in machines)
                {
                    if (m.availability == "Available" && m.type == "Washer")
                    {
                        freeWash++;
                    }
                    else if (m.availability == "Available" && m.type == "Dryer")
                    {
                        freeDry++;
                    }
                }
                esFree.Text = "Free: W: " + freeWash + " D: " + freeDry;
            }
            else
            {
            }
        }

        private List<LaundryMachine> ExtractStates(HtmlDocument sudsTimes)
        {
            if (sudsTimes != null)
            {
                List<LaundryMachine> machines = new List<LaundryMachine>();
                int dryer = -1;

                foreach (HtmlNode row in sudsTimes.DocumentNode.SelectNodes("//tr"))
                {
                    IEnumerable<HtmlNode> thNodes = row.Elements("th");
                    if (thNodes.ToList().Count != 0)
                    {
                        dryer++;
                    }
                    if (row.HasAttributes && (row.Attributes[0].Value == "even" || row.Attributes[0].Value == "odd"))
                    {
                        IEnumerable<HtmlNode> nodes = row.Elements("td");
                        LaundryMachine newWD = new LaundryMachine(
                            nodes.ElementAt(1).InnerText,
                            dryer <= 0 ? "Washer" : "Dryer", //nodes.ElementAt(2).InnerText
                            nodes.ElementAt(3).InnerText.Replace("\n", ""),
                            nodes.ElementAt(4).InnerText != "&nbsp;" ? nodes.ElementAt(4).InnerText : "",
                            "");
                        machines.Add(newWD);
                    }
                }
                return machines;
            }
            return null;
        }

        #endregion
    }
}