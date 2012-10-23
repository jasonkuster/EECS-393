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
            ContentPanel.Children.Add(actualMap);
        }

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

                // The latitude value range (From = bottom most latitude, To = top most latitude)
                // validLats = new Range<double>(47.58346, 47.62877);
                // The longitude value range (From = left most longitude, To = right most longitude)
                // validLongs = new Range<double>(-122.36298, -122.28599);
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

        public class SeattleMapMode : AerialMode
        {
            private Range<double> validLatitudeRange;
            private Range<double> validLongitudeRange;

            public SeattleMapMode()
            {
                // The latitude value range (From = bottom most latitude, To = top most latitude)
                validLatitudeRange = new Range<double>(47.58346, 47.62877);
                // The longitude value range (From = left most longitude, To = right most longitude)
                validLongitudeRange = new Range<double>(-122.36298, -122.28599);
            }

            // Restricts the map view.
            protected override Range<double> GetZoomRange(GeoCoordinate center)
            {
                // The allowable zoom levels - 14 to 21.
                return new Range<double>(14, 21);
            }


            // This method is called when the map view changes on Keyboard 
            // and Navigation Bar events.
            public override bool ConstrainView(GeoCoordinate center, ref double zoomLevel, ref double heading, ref double pitch)
            {
                bool isChanged = base.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);

                double newLatitude = center.Latitude;
                double newLongitude = center.Longitude;

                // If the map view is outside the valid longitude range,
                // move the map back within range.
                if (center.Longitude > validLongitudeRange.To)
                {
                    newLongitude = validLongitudeRange.To;
                }
                else if (center.Longitude < validLongitudeRange.From)
                {
                    newLongitude = validLongitudeRange.From;
                }

                // If the map view is outside the valid latitude range,
                // move the map back within range.
                if (center.Latitude > validLatitudeRange.To)
                {
                    newLatitude = validLatitudeRange.To;
                }
                else if (center.Latitude < validLatitudeRange.From)
                {
                    newLatitude = validLatitudeRange.From;
                }

                // The new map view location.
                if (newLatitude != center.Latitude || newLongitude != center.Longitude)
                {
                    center.Latitude = newLatitude;
                    center.Longitude = newLongitude;
                    isChanged = true;
                }

                // The new zoom level.
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