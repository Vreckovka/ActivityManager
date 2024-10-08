﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VCore.Standard.Modularity.Interfaces;

namespace ActivityManager.Views.Modals
{
  /// <summary>
  /// Interaction logic for AddActivity.xaml
  /// </summary>
  public partial class AddActivity : UserControl, IView
  {
    public AddActivity()
    {
      InitializeComponent();

      Loaded += AddActivity_Loaded;
    }

    private void AddActivity_Loaded(object sender, RoutedEventArgs e)
    {
      DatePicker_Activity.Language = XmlLanguage.GetLanguage("sk-SK");
    }
  }
}
