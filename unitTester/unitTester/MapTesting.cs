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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Device.Location;

namespace unitTester
{
    [TestClass]
    public class MapTesting : SilverlightTest
    {
        [TestMethod]
        public void CWRUMapModeTest()
        {
            Random rand = new Random();
            var mm = new CWRUtility.CWRUMapMode();
            GeoCoordinate center = new GeoCoordinate(22.0, 22.0);
            double zoom = 15.0;
            double heading = rand.NextDouble() * rand.Next(101);
            double pitch = rand.NextDouble() * rand.Next(101);

            Assert.IsTrue(mm.ConstrainView(center, ref zoom, ref heading, ref pitch), "MapMode ConstrainView test");

            for (int i = 0; i < 100; i++)
            {
                pitch = rand.NextDouble() * rand.Next(101);
                heading = rand.NextDouble() * rand.Next(101);
                double randLat = rand.NextDouble() * rand.Next(91);
                double randLong = rand.NextDouble() * rand.Next(181);

                int shouldFlip = rand.Next(2);
                randLong = shouldFlip == 1 ? randLong : randLong * -1;

                GeoCoordinate randCenter = new GeoCoordinate(randLat, randLong);
                if ((randLat < 41.499707 || randLat > 41.515711) || (randLong < -81.615243 || randLong > -81.598334))
                {
                    Assert.IsTrue(mm.ConstrainView(randCenter, ref zoom, ref heading, ref pitch));
                }
                else
                {
                    Assert.IsFalse(mm.ConstrainView(randCenter, ref zoom, ref heading, ref pitch));
                }
            }

            center = new GeoCoordinate(41.5, -81.6);
            for (int i = 0; i < 100; i++)
            {
                pitch = rand.NextDouble() * rand.Next(101);
                heading = rand.NextDouble() * rand.Next(101);
                zoom = rand.NextDouble() * rand.Next(51);

                if (zoom < 15.0 || zoom > 20.0)
                {
                    Assert.IsTrue(mm.ConstrainView(center, ref zoom, ref heading, ref pitch));
                }
                else
                {
                    Assert.IsFalse(mm.ConstrainView(center, ref zoom, ref heading, ref pitch));
                }
            }

        }
    }
}
