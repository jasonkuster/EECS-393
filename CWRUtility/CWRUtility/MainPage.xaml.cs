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
                //MessageBox.Show((string)settings["nbDefault"]);
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
                nbDef.Text = ((string)settings["nbDefault"]).Split('!')[2];
                //MessageBox.Show((string)settings["nbDefault"]);
            }
            //NavigationService.Navigate(new Uri("/NextBus.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Map_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Map.xaml", UriKind.RelativeOrAbsolute));
        }

        private void NextBust_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/NextBus.xaml", UriKind.RelativeOrAbsolute));    
        }
    }
}