﻿using RaceHorologyLib;
using System;
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

namespace RaceHorology
{
  /// <summary>
  /// Interaction logic for TimingDeviceAlpenhundeUC.xaml
  /// </summary>
  public partial class TimingDeviceAlpenhundeUC : UserControl
  {
    private TimingDeviceAlpenhunde _timingDevice;
    public TimingDeviceAlpenhundeUC(TimingDeviceAlpenhunde timingDevice)
    {
      _timingDevice = timingDevice;
      InitializeComponent();
    }
  }
}
