using System;
using System.Collections.Generic;
using System.Linq;
using ActivityManager.Domain;
using VCore.Standard;
using VCore.Standard.Helpers;

namespace ActivityManager.ViewModels.Statistics
{
  public class ActivityStatisticsViewModel : ViewModel
  {
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



    private void OnDateChanged(DateTime dateTime)
    {
      CalculateStats(dateTime);
    }

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

    public void CalculateStats(DateTime dateTime)
    {
      if (Activities != null)
      {
        var ranged = Activities.Where(x => GetStatisticsRangePredicated(x, dateTime, Range)).ToList();

        TotalKmValue = ranged.Sum(x => x.DistanceInKm);
        TotalTime = TimeSpan.FromTicks(ranged.Sum(x => x.DurationTicks));
      }
    }

    #endregion
  }

  public class ActivityStatisticsGroupViewModel : ViewModel
  {
    public ActivityType? ActivityType { get; set; }

    public IEnumerable<ActivityStatisticsViewModel> ActivityStatistics { get; set; }
  }
}