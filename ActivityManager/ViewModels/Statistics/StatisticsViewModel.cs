using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using ActivityManager.Core;
using ActivityManager.Domain;
using ActivityManager.Providers;
using ActivityManager.Views;
using VCore.WPF.ItemsCollections;
using VCore.WPF.Modularity.RegionProviders;
using VCore.WPF.ViewModels;

namespace ActivityManager.ViewModels.Statistics
{
  public class StatisticsViewModel : RegionViewModel<StatisticsView>
  {
    private readonly IActivitiesProvider activitiesProvider;

    public StatisticsViewModel(IRegionProvider regionProvider, IActivitiesProvider activitiesProvider) : base(regionProvider)
    {
      this.activitiesProvider = activitiesProvider ?? throw new ArgumentNullException(nameof(activitiesProvider));
    }

    public override string Header => "Štatistiky";
    public override string RegionName { get; protected set; } = RegionNames.MainContent;

    public ItemsViewModel<ActivityStatisticsGroupViewModel> Statistics { get; } = new ItemsViewModel<ActivityStatisticsGroupViewModel>();

    public override void OnActivation(bool firstActivation)
    {
      base.OnActivation(firstActivation);

      Task.Run(async () =>
      {
        var stats = await CalculateStatistics();

        Application.Current.Dispatcher.Invoke(() =>
        {
          Statistics.Clear();
          Statistics.AddRange(stats);
        });
      });

    }

    #region CalculateStatistics

    public Task<IEnumerable<ActivityStatisticsGroupViewModel>> CalculateStatistics()
    {
      return Task.Run(async () =>
      {
        var list = new List<ActivityStatisticsGroupViewModel>();

        var activities = (await activitiesProvider.LoadActivitiesAsync()).ToList();

        foreach (ActivityType activityType in Enum.GetValues(typeof(ActivityType)))
        {
          var allTypedActivities = activities.Where(x => x.Type == (activityType)).ToList();
          var groupStats = new List<ActivityStatisticsViewModel>();

          foreach (StatisticsRange range in Enum.GetValues(typeof(StatisticsRange)))
          {
            var newVm = new ActivityStatisticsViewModel()
            {
              Range = range,
              ActivityType = activityType
            };

            var ranged = allTypedActivities.Where(x => GetStatisticsRangePredicated(x, range)).ToList();

            newVm.TotalKmValue = ranged.Sum(x => x.DistanceInKm);
            newVm.TotalTime = TimeSpan.FromTicks(ranged.Sum(x => x.DurationTicks));

            groupStats.Add(newVm);
          }


          var newGroupVm = new ActivityStatisticsGroupViewModel()
          {
            ActivityType = activityType,
            ActivityStatistics = groupStats
          };



          list.Add(newGroupVm);
        }

        var totalGroup = new ActivityStatisticsGroupViewModel();
        var totalList = new List<ActivityStatisticsViewModel>();
        var allStats = list.SelectMany(x => x.ActivityStatistics).ToList();

        foreach (StatisticsRange range in Enum.GetValues(typeof(StatisticsRange)))
        {
          var totalKm = allStats.Where(x => x.Range == range).Sum(x => x.TotalKmValue);
          var totalTime = TimeSpan.FromTicks(allStats.Where(x => x.Range == range).Sum(x => x.TotalTime.Ticks));

          var newVm = new ActivityStatisticsViewModel()
          {
            Range = range,
            TotalKmValue = totalKm,
            TotalTime = totalTime
          };

          totalList.Add(newVm);
        }

        totalGroup.ActivityStatistics = totalList;
        list.Add(totalGroup);
        return list.AsEnumerable();
      });
    }

    #endregion

    #region GetStatisticsRangePredicated

    private bool GetStatisticsRangePredicated(Activity activity, StatisticsRange statisticsRange)
    {
      switch (statisticsRange)
      {
        case StatisticsRange.Day:
          return activity.Created != null && activity.Created.Value.Date == DateTime.Today;
        case StatisticsRange.Week:
          return activity.Created != null && activity.Created.Value.Date >= DateTime.Today.AddDays(-7);
        case StatisticsRange.Month:
          return activity.Created != null && activity.Created.Value.Date >= DateTime.Today.AddMonths(-1);
        case StatisticsRange.Year:
          return activity.Created != null && activity.Created.Value.Date >= DateTime.Today.AddYears(-1);
        case StatisticsRange.Total:
          return activity.Created != null;
      }

      return false;
    }

    #endregion
  }
}
