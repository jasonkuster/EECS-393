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
        public NextBus()
        {
            InitializeComponent();
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
            Dispatcher.BeginInvoke(delegate
            {
            predictions = extractPredictions(busPredictions);
            favBox.ItemsSource = predictions;
            goButton.IsEnabled = true;
            });
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

                return bpTags;
            }
            else
                throw new ArgumentNullException();
        }
    }
}