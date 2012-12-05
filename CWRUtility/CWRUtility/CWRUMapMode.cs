using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;

namespace CWRUtility
{
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
            return new Range<double>(14.5, 20);
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
