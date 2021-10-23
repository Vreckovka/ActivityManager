using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ActivityManager.ViewModels.Statistics;
using VCore.Standard.Modularity.Interfaces;
using Calendar = System.Windows.Controls.Calendar;

namespace ActivityManager.Views
{
  /// <summary>
  /// Interaction logic for StatisticsView.xaml
  /// </summary>
  public partial class StatisticsView : UserControl, IView
  {
    public StatisticsView()
    {
      InitializeComponent();

    }

    private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
    {
      var datepicker = sender as DatePicker;

      if (datepicker != null && datepicker.DataContext is ActivityStatisticsViewModel activity && datepicker.IsDropDownOpen)
      {
        var popup = datepicker.Template.FindName("PART_Popup", datepicker) as Popup;
        if (popup != null && popup.Child is Calendar)
        {
          if (activity.Range == StatisticsRange.Day)
            ((Calendar)popup.Child).DisplayMode = CalendarMode.Month;

          if (activity.Range == StatisticsRange.Week)
            ((Calendar)popup.Child).DisplayMode = CalendarMode.Month;

          if (activity.Range == StatisticsRange.Month)
            ((Calendar)popup.Child).DisplayMode = CalendarMode.Year;

          if (activity.Range == StatisticsRange.Year)
            ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
        }

        var calendar = GetDatePickerCalendar(datepicker);

        calendar.DisplayModeChanged += Calendar_DisplayModeChanged;

      }
    }

    private void Calendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs calendarModeChangedEventArgs)
    {
      if (sender is Calendar calendar)
      {
        if (calendar.DataContext is ActivityStatisticsViewModel activity)
        {
          if (calendar.DisplayMode != CalendarMode.Decade && activity.Range == StatisticsRange.Year)
          {

            activity.Date = new DateTime(calendar.DisplayDate.Year, 1, 1);
            activity.IsOpen = false;

            calendar.DisplayModeChanged -= Calendar_DisplayModeChanged;
            calendar.DisplayMode = CalendarMode.Decade;
          }
          else if (calendar.DisplayMode != CalendarMode.Year && activity.Range == StatisticsRange.Month)
          {

            activity.Date = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1);
            activity.IsOpen = false;

            calendar.DisplayModeChanged -= Calendar_DisplayModeChanged;
            calendar.DisplayMode = CalendarMode.Year;
          }
        }
      }
    }

    private static Calendar GetDatePickerCalendar(object sender)
    {
      var datePicker = (DatePicker)sender;
      var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
      return ((Calendar)popup.Child);
    }

   
  }
  
}
