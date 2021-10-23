using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ActivityManager.Domain;
using LiveCharts;
using LiveCharts.Wpf;
using VCore.Standard;
using VCore.Standard.Helpers;

namespace ActivityManager.ViewModels.Statistics
{
  public class ActivityStatisticsViewModel : ViewModel
  {
    #region Properties

    public ActivityType? ActivityType { get; set; }
    public StatisticsRange Range { get; set; }
    public IEnumerable<Activity> Activities { get; set; }


    #region IsOpen

    private bool isOpen;

    public bool IsOpen
    {
      get { return isOpen; }
      set
      {
        if (value != isOpen)
        {
          isOpen = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region TotalKmValue

    private double totalKmValue;

    public double TotalKmValue
    {
      get { return totalKmValue; }
      set
      {
        if (value != totalKmValue)
        {
          totalKmValue = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region TotalTime

    private TimeSpan totalTime;

    public TimeSpan TotalTime
    {
      get { return totalTime; }
      set
      {
        if (value != totalTime)
        {
          totalTime = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Date

    private DateTime date = DateTime.Today;

    public DateTime Date
    {
      get { return date; }
      set
      {
        if (value != date)
        {
          date = value;
          OnDateChanged(date);
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region ChartValues

    private IChartValues chartValues;

    public IChartValues ChartValues
    {
      get { return chartValues; }
      set
      {
        if (value != chartValues)
        {
          chartValues = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region Labels

    private string[] labels;

    public string[] Labels
    {
      get { return labels; }
      set
      {
        if (value != labels)
        {
          labels = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region YFormatter

    private Func<double, string> yFormatter;

    public Func<double, string> YFormatter
    {
      get { return yFormatter; }
      set
      {
        if (value != yFormatter)
        {
          yFormatter = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    #region OnDateChanged


    private async void OnDateChanged(DateTime dateTime)
    {
      await CalculateStats(dateTime);
    }

    #endregion

    #region GetStatisticsRangePredicated

    private bool GetStatisticsRangePredicated(Activity activity, DateTime activityDate, StatisticsRange statisticsRange)
    {
      DateTime startDate = DateTime.Now;
      DateTime endDate = DateTime.Now;

      if (activity.Created != null)
      {
        switch (statisticsRange)
        {
          case StatisticsRange.Day:
            return activity.Created.Value.Date == activityDate.Date;
          case StatisticsRange.Week:
            startDate = activityDate.StartOfWeek(DayOfWeek.Monday);
            endDate = startDate.AddDays(7);
            break;
          case StatisticsRange.Month:
            startDate = new DateTime(activityDate.Year, activityDate.Month, 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
            break;
          case StatisticsRange.Year:
            startDate = new DateTime(activityDate.Year, 1, 1);
            endDate = startDate.AddYears(1).AddDays(-1);
            break;
          case StatisticsRange.Total:
            return activity.Created != null;
        }

        return activity.Created.Value.Date >= startDate.Date && endDate.Date > activity.Created.Value.Date;
      }


      return false;
    }

    #endregion

    #region CalculateStats

    public async Task CalculateStats(DateTime? dateTime = null)
    {
      if (dateTime == null)
      {
        dateTime = Date;
      }

      if (Activities != null)
      {
        var ranged = Activities.Where(x => GetStatisticsRangePredicated(x, dateTime.Value, Range)).ToList();

        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
          TotalKmValue = ranged.Sum(x => x.DistanceInKm);
          TotalTime = TimeSpan.FromTicks(ranged.Sum(x => x.DurationTicks));

          if (Range == StatisticsRange.Total)
          {
            var kms = new List<double>();
            var dates = new List<string>();

            double total = 0;

            foreach (var item in ranged)
            {
              total += item.DistanceInKm;

              dates.Add(item.Created.Value.ToShortDateString());
              kms.Add(total);
            }

            ChartValues = new ChartValues<double>(kms);
            Labels = dates.ToArray();
            YFormatter = value => value.ToString("N1");
          }
        });
      }
    }

    #endregion

    #endregion

  }
}