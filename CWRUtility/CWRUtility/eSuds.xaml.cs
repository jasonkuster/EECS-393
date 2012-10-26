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

        public eSuds()
        {
            InitializeComponent();
            CreateBuildingsDict();
        }

        private void CreateBuildingsDict()
        {
            List<string> buildings = new List<string>() { "Alumni", "Clarke Tower", "Cutler", "Glaser", "Hitchcock", "Howe", "Kusch", "Michelson", "Norton", 
                "Pierce", "Raymond", "Sherman", "Smith", "Staley", "Storrs", "Taft", "Taplin", "Tippit", "Tyler", "Village House 1", "Village House 2", 
                "Village House 4", "Village House 5", "Village House 6", "Village House 7" };
            List<int> bIDs = new List<int>() { 1429, 1398, 1405, 1423, 4193, 1402, 1431, 4188, 4191, 1409, 1407, 1415, 1413, 1421, 1427, 1400, 1419, 1417, 1425, 1411,
                1443, 1445, 1447, 1448, 1449, 1450};

            for (int i = 0; i < buildings.Count; i++)
            {
                locUris.Add(buildings[i], new Uri("http://case-asi.esuds.net/RoomStatus/showRoomStatus.i?locationId=" + bIDs[i]));
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!String.IsNullOrEmpty((string)settings["esDefault"]))
            {
                buildingPicker.ItemsSource = locUris.Keys;
            }
            base.OnNavigatedTo(e);
        }

        private void buildingPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (buildingPicker.SelectedIndex != -1)
                GetTimes();
        }

        private void GetTimes()
        {
            string location = (string)buildingPicker.SelectedItem;
            LockUI();
            ScrapeHTML(locUris[location]);
        }

        private void ScrapeHTML(Uri locUri)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(locUri);
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
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
                WashersList.ItemsSource = machines;
            }
            else
            {
            }
            UnlockUI();
        }

        private List<WasherDryer> ExtractStates(HtmlDocument sudsTimes)
        {
            if (sudsTimes != null)
            {
                List<WasherDryer> machines = new List<WasherDryer>();

                foreach (HtmlNode row in sudsTimes.DocumentNode.SelectNodes("//tr"))
                {
                    if (row.Attributes[0].Value == "even" || row.Attributes[0].Value == "odd")
                    {
                        WasherDryer newWD = new WasherDryer(
                            row.ChildNodes.ElementAt(1).InnerText,
                            row.ChildNodes.ElementAt(2).InnerText,
                            row.ChildNodes.ElementAt(3).InnerText,
                            row.ChildNodes.ElementAt(4).InnerText);
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
    }
    class WasherDryer
    {
        public WasherDryer(string number, string type, string availability, string timeRemaining)
        {
            this.number = number;
            this.type = type;
            this.availability = availability;
            this.timeRemaining = timeRemaining;
        }

        public string number { get; set; }
        public string type { get; set; }
        public string availability { get; set; }
        public string timeRemaining { get; set; }
    }
}