﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_timespanpicker
{
    /// <summary>
    /// Interaction logic for TwoDigitsDisplay.xaml
    /// </summary>
    public partial class TwoDigitsDisplay : UserControl
    {
        public TwoDigitsDisplay()
        {
            InitializeComponent();
        }

        internal void SetForeground(Brush b)
        {
            tb1.Foreground = tb2.Foreground = b;
        }
    }
}
