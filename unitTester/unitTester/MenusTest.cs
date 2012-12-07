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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Silverlight.Testing;

namespace unitTester
{
    [TestClass]
    public class MenusTest : SilverlightTest
    {
        [TestMethod]
        public void testMenus()
        {
            CWRUtility.NextBus nbTest = new CWRUtility.NextBus();
            Assert.IsTrue(nbTest.buses != null);
            Assert.IsTrue(nbTest.settings.Contains("nbFavorites"));
        }

    }
}