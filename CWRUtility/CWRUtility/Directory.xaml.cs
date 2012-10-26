﻿using System;
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
    public partial class Directory : PhoneApplicationPage
    {
        public Directory()
        {
            InitializeComponent();
        }
    }

    public class Resource
    {
        public Resource(string name, string phone, string info)
        {
            this.name = name;
            this.phone = phone;
            this.info = info;
        }
        public string name { get; set; }
        public string phone { get; set; }
        public string info { get; set; }
    }
}