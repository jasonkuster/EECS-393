using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps.Core;
using System.Windows.Controls;
using System.Windows.Resources;
using Microsoft.Phone.Controls.Maps.Platform;
using System.Windows.Media;

namespace CWRUtility
{
    public partial class Map : PhoneApplicationPage
    {
        private MapLayer outline;
        private GeoCoordinateWatcher loc = null;

        public Map()
        {
            InitializeComponent();
            actualMap.CredentialsProvider = new ApplicationIdCredentialsProvider("Bing Maps Key");

            //GeoCoordinate mapCenter = new GeoCoordinate(41.51093, -81.60323);
            actualMap.Mode = new CWRUMapMode();
            outline = getCampusOutline();
            actualMap.Children.Add(outline);

            //actualMap.SetView(mapCenter, 16);
            //bmapGrid.Children.Add(actualMap);
        }

        private void map1_LoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            ApplicationBar.IsVisible = (e.Item == bingMap);
        }

        private static void addCWRUPinsToLayer(MapLayer layer)
        {
            Pushpin northSide = new Pushpin();
            northSide.Location = new GeoCoordinate(41.513453, -81.605529);
            northSide.Content = "North Side";
            layer.Children.Add(northSide);

            Pushpin matherQuad = new Pushpin();
            matherQuad.Location = new GeoCoordinate(41.509018, -81.607954);
            matherQuad.Content = "Mather Quad";
            layer.Children.Add(matherQuad);

            Pushpin theQuad = new Pushpin();
            theQuad.Location = new GeoCoordinate(41.503297, -81.608050);
            theQuad.Content = "The Quad";
            layer.Children.Add(theQuad);

            Pushpin southSide = new Pushpin();
            southSide.Location = new GeoCoordinate(41.501144, -81.601581);
            southSide.Content = "South Side";
            layer.Children.Add(southSide);
        }

        private static MapLayer getCampusOutline()
        {
            MapLayer campLayer = new MapLayer();
            MapPolyline outline = new MapPolyline();
            outline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            outline.StrokeThickness = 5;
            outline.Opacity = 1;
            outline.Locations = new LocationCollection() {
                    new GeoCoordinate(41.504697, -81.610809),
                    new GeoCoordinate(41.505163, -81.610208),
                    new GeoCoordinate(41.509357, -81.610444),
                    new GeoCoordinate(41.510514, -81.609178),
                    new GeoCoordinate(41.511799, -81.609200),
                    new GeoCoordinate(41.512586, -81.609758),
                    new GeoCoordinate(41.513486, -81.608492),
                    new GeoCoordinate(41.514547, -81.608320),
                    new GeoCoordinate(41.514289, -81.606904),
                    new GeoCoordinate(41.515928, -81.606839),
                    new GeoCoordinate(41.515944, -81.603041),
                    new GeoCoordinate(41.511927, -81.601582),
                    new GeoCoordinate(41.508103, -81.606196),
                    new GeoCoordinate(41.505452, -81.602398),
                    new GeoCoordinate(41.503234, -81.603921),
                    new GeoCoordinate(41.502977, -81.604458),
                    new GeoCoordinate(41.502624, -81.604393),
                    new GeoCoordinate(41.501611, -81.603170),
                    new GeoCoordinate(41.503347, -81.601217),
                    new GeoCoordinate(41.501450, -81.597698),
                    new GeoCoordinate(41.499683, -81.600574),
                    new GeoCoordinate(41.499956, -81.606539),
                    new GeoCoordinate(41.500652, -81.607838),
                    new GeoCoordinate(41.502018, -81.608321),
                    new GeoCoordinate(41.504697, -81.610809)
                };
            campLayer.Children.Add(outline);

            addCWRUPinsToLayer(campLayer);
            return campLayer;
        }

        private void toggleLayerButton_Click(object sender, EventArgs e)
        {
            if (outline.Visibility == System.Windows.Visibility.Visible)
            {
                outline.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                outline.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void my_location_Click(object sender, EventArgs e)
        {
            if (loc == null) { 
                loc = new GeoCoordinateWatcher(GeoPositionAccuracy.Default); 
                loc.StatusChanged += loc_StatusChanged; 
            } if (loc.Status == GeoPositionStatus.Disabled) { 
                loc.StatusChanged -= loc_StatusChanged; 
                MessageBox.Show("Location services must be enabled on your phone."); 
                return; 
            } 
            
            loc.Start();
        }

        void loc_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {

            if (e.Status == GeoPositionStatus.Ready)
            {
                Pushpin p = new Pushpin();
                p.Template = this.Resources["pinMyLoc"] as ControlTemplate;
                p.Location = loc.Position.Location;
                mapControl.Items.Add(p);
                actualMap.SetView(loc.Position.Location, 17.0);
                loc.Stop();
            }
        }

        

        /* private void scroller_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ScrollViewer scroll = (ScrollViewer) sender;
            //((Image) sender).RenderTransformOrigin = new Point(0,0);
            Image scrollMap = (Image) scroll.Content;
            scrollMap.Height = 100;
            scroll.Content = scrollMap;
            //
            //(((Image) sender).RenderTransform as CompositeTransform).ScaleY = 0.5;
        }*/

        /*private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
         initialScale = atransform.ScaleX;
        }

         private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
         atransform.ScaleX = initialScale * e.DistanceRatio;
         atransform.ScaleY = initialScale * e.DistanceRatio;
        }*/
    }
}