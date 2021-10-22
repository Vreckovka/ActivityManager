using System;
using System.Collections.Generic;
using ActivityManager.Domain;
using VCore.Standard;

namespace ActivityManager.ViewModels.Statistics
{
  public class ActivityStatisticsViewModel : ViewModel
  {
    public ActivityType? ActivityType { get; set; }

    public double TotalKmValue { get; set; }

    public TimeSpan TotalTime { get; set; }

    public StatisticsRange Range { get; set; }

  }

  public class ActivityStatisticsGroupViewModel : ViewModel
  {
    public ActivityType? ActivityType { get; set; }

    public IEnumerable<ActivityStatisticsViewModel> ActivityStatistics { get; set; }
  }
}