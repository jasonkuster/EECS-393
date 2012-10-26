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
    public partial class eSuds : PhoneApplicationPage
    {
        Dictionary<string, Uri> locUris = new Dictionary<string, Uri>();
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        bool gettingTimes = false;
        bool loading = false;

        public eSuds()
        {
            CreateBuildingsDict();
            InitializeComponent();
        }

        private void CreateBuildingsDict()
        {
            List<string> buildings = new List<string>() { "Alumni", "Clarke Tower", "Cutler", "Cutter", "Glaser", "Hitchcock", "Howe", "Kusch", "Michelson", "Norton", 
                "Pierce", "Raymond", "Sherman", "Smith", "Staley", "Storrs", "Taft", "Taplin", "Tippit", "Tyler", "Village House 1", "Village House 2", 
                "Village House 4", "Village House 5", "Village House 6", "Village House 7" };
            List<int> bIDs = new List<int>() { 1429, 1398, 1405, 1423, 4193, 1403, 1431, 4188, 4191, 1409, 1407, 1415, 1413, 1421, 1427, 1400, 1419, 1417, 1425, 1411,
                1443, 1444, 1446, 1447, 1448, 1449};

            for (int i = 0; i < buildings.Count; i++)
            {
                locUris.Add(buildings[i], new Uri("http://case-asi.esuds.net/RoomStatus/machineStatus.i?bottomLocationId=" + bIDs[i]));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            loading = true;
            buildingPicker.ItemsSource = locUris.Keys;
            loading = false;
            if ((e.NavigationMode == System.Windows.Navigation.NavigationMode.New || e.NavigationMode == System.Windows.Navigation.NavigationMode.Refresh)
                && !String.IsNullOrEmpty((string)settings["esDefault"]))
            {
                buildingPicker.SelectedItem = ((string)settings["esDefault"]).Split('!')[0];
            }
            base.OnNavigatedTo(e);
        }

        private void buildingPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buildingPicker.SelectedIndex != -1 && !loading && !gettingTimes)
                GetTimes();
        }

        private void GetTimes()
        {
            string location = (string)buildingPicker.SelectedItem;
            gettingTimes = true;
            LockUI();
            ScrapeHTML(locUris[location]);
        }

        private void ScrapeHTML(Uri locUri)
        {
            WebClient esClient = new WebClient();
            esClient.OpenReadCompleted += new OpenReadCompletedEventHandler(esClient_OpenReadCompleted);
            esClient.OpenReadAsync(locUri);
        }

        void esClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            Stream data = e.Result as Stream;
            StreamReader reader = new StreamReader(data);
            HtmlDocument sudsTimes = new HtmlDocument();
            sudsTimes.Load(reader);
            data.Close();
            reader.Close();
            DisplayStates(sudsTimes);
        }

        private void DisplayStates(HtmlDocument sudsTimes)
        {
            List<WasherDryer> machines = ExtractStates(sudsTimes);
            if (machines != null)
            {
                WashersList.ItemsSource = null;
                WashersList.ItemsSource = machines;
            }
            else
            {
            }
            UnlockUI();
            gettingTimes = false;
        }

        private List<WasherDryer> ExtractStates(HtmlDocument sudsTimes)
        {
            if (sudsTimes != null)
            {
                List<WasherDryer> machines = new List<WasherDryer>();
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
                        WasherDryer newWD = new WasherDryer(
                            nodes.ElementAt(1).InnerText,
                            dryer <= 0 ? "Washer": "Dryer", //nodes.ElementAt(2).InnerText
                            nodes.ElementAt(3).InnerText.Replace("\n", ""),
                            nodes.ElementAt(4).InnerText != "&nbsp;" ? nodes.ElementAt(4).InnerText : "",
                            nodes.ElementAt(3).InnerText.Replace("\n", "") == "Available" ? "green" : "red");
                        machines.Add(newWD);
                    }
                }
                return machines;
            }
            return null;
        }

        private void LockUI()
        {
            buildingPicker.IsEnabled = false;
        }

        private void UnlockUI()
        {
            buildingPicker.IsEnabled = true;
        }

        private void defaultButton_Click(object sender, EventArgs e)
        {
            string def = (string)buildingPicker.SelectedItem;
            settings["esDefault"] = def + "!" + locUris[def];
            MessageBox.Show(def + " set as default.", "Default Set", MessageBoxButton.OK);
        }
    }
    public class WasherDryer
    {
        public WasherDryer(string number, string type, string availability, string timeRemaining, string color)
        {
            this.number = number;
            this.type = type;
            this.availability = availability;
            this.timeRemaining = timeRemaining;
            this.color = color;
        }

        public string number { get; set; }
        public string type { get; set; }
        public string availability { get; set; }
        public string timeRemaining { get; set; }
        public string color { get; set; }
    }
}