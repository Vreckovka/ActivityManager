using System.Collections.Generic;
using ActivityManager.Domain;
using VCore.Standard;

namespace ActivityManager.ViewModels.Statistics
{
  public class ActivityStatisticsGroupViewModel : ViewModel
  {
    public ActivityType? ActivityType { get; set; }

    public ActivityStatisticsViewModel Total { get; set; }
    public List<ActivityStatisticsViewModel> ActivityStatistics { get; set; }
  }
}