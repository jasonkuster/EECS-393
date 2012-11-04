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

namespace CWRUtility
{
    public partial class NewsStoryPage : PhoneApplicationPage
    {
        public NewsStoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string uri = this.NavigationContext.QueryString["storyUri"];
            if (!"".Equals(uri))
            {
                browser.Navigate(new Uri(uri));
            }
        }
    }
}