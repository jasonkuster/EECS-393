using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Core;
using System.Windows.Controls;
using System.Windows.Resources;
using Microsoft.Phone.Controls.Maps.Platform;

namespace CWRUtility
{
    public partial class Map : PhoneApplicationPage
    {
        public Map()
        {
            InitializeComponent();

            Microsoft.Phone.Controls.Maps.Map actualMap = new Microsoft.Phone.Controls.Maps.Map();
            actualMap.CredentialsProvider = new ApplicationIdCredentialsProvider("Bing Maps Key");

            //GeoCoordinate mapCenter = new GeoCoordinate(41.51093, -81.60323);
            actualMap.Mode = new CWRUMapMode();

            //actualMap.SetView(mapCenter, 16);
            bmapGrid.Children.Add(actualMap);
        }

       /*private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            initialScale = atransform.ScaleX;
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            atransform.ScaleX = initialScale * e.DistanceRatio;
            atransform.ScaleY = initialScale * e.DistanceRatio;
        }*/

        public class CWRUMapMode : RoadMode
        {
            private Range<double> validLats;
            private Range<double> validLongs;

            public CWRUMapMode()
            {
                // The latitude value range (From = bottom most latitude, To = top most latitude)
                validLats = new Range<double>(41.499707, 41.515711);
                // The longitude value range (From = left most longitude, To = right most longitude)
                validLongs = new Range<double>(-81.615243, -81.598334);
            }

            protected override Range<double> GetZoomRange(GeoCoordinate center)
            {
                return new Range<double>(15, 20);
            }

            public override bool ConstrainView(GeoCoordinate center, ref double zoomLevel, ref double heading, ref double pitch)
            {
                bool isChanged = base.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);

                double newLatitude = center.Latitude;
                double newLongitude = center.Longitude;

                if (center.Longitude > validLongs.To)
                {
                    newLongitude = validLongs.To;
                }
                else if (center.Longitude < validLongs.From)
                {
                    newLongitude = validLongs.From;
                }

                if (center.Latitude > validLats.To)
                {
                    newLatitude = validLats.To;
                }
                else if (center.Latitude < validLats.From)
                {
                    newLatitude = validLats.From;
                }

                if (newLatitude != center.Latitude || newLongitude != center.Longitude)
                {
                    center.Latitude = newLatitude;
                    center.Longitude = newLongitude;
                    isChanged = true;
                }

                Range<double> range = GetZoomRange(center);
                if (zoomLevel > range.To)
                {
                    zoomLevel = range.To;
                    isChanged = true;
                }
                else if (zoomLevel < range.From)
                {
                    zoomLevel = range.From;
                    isChanged = true;
                }

                return isChanged;
            }

        }
    }
}