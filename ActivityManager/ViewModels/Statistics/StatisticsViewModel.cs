using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ActivityManager.Core;
using ActivityManager.Domain;
using ActivityManager.Providers;
using ActivityManager.ViewModels.Activities;
using ActivityManager.ViewModels.Modals;
using ActivityManager.Views;
using ActivityManager.Views.Modals;
using VCore.WPF.ItemsCollections;
using VCore.WPF.Misc;
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
        var stats = await CalculateStatistics(Statistics.ViewModels);

        Application.Current.Dispatcher.Invoke(() =>
        {
          Statistics.Clear();
          Statistics.AddRange(stats);
        });
      });
    }

    #region Refresh

    private ActionCommand refresh;

    public ICommand Refresh
    {
      get
      {
        return refresh ??= new ActionCommand(OnAddActivity);
      }
    }

    private void OnAddActivity()
    {
      Task.Run(async () =>
      {
        var stats = await CalculateStatistics(Statistics.ViewModels);

        Application.Current.Dispatcher.Invoke(() =>
        {
          Statistics.Clear();
          Statistics.AddRange(stats);
        });
      });
    }

    #endregion

    #region CalculateStatistics

    public Task<IEnumerable<ActivityStatisticsGroupViewModel>> CalculateStatistics(IEnumerable<ActivityStatisticsGroupViewModel> old)
    {
      return Task.Run(async () =>
      {
        var list = new List<ActivityStatisticsGroupViewModel>();
        var oldList = old?.ToList();

        var activities = (await activitiesProvider.LoadActivitiesAsync()).ToList();

        foreach (ActivityType activityType in Enum.GetValues(typeof(ActivityType)))
        {
          var allTypedActivities = activities.Where(x => x.Type == (activityType)).ToList();

          ActivityStatisticsGroupViewModel newGroupVm = null;

          if (oldList != null)
          {
            newGroupVm = oldList.SingleOrDefault(x => x.ActivityType == activityType);
          }

          if (newGroupVm == null)
          {
            newGroupVm = new ActivityStatisticsGroupViewModel()
            {
              ActivityType = activityType,
              ActivityStatistics = new List<ActivityStatisticsViewModel>()
            };
          }

          foreach (StatisticsRange range in Enum.GetValues(typeof(StatisticsRange)))
          {
            var newVm = new ActivityStatisticsViewModel()
            {
              Range = range,
              ActivityType = activityType,
              Date = DateTime.Now,
              Activities = allTypedActivities
            };
           

            if (newGroupVm.ActivityStatistics.Count > 0)
            {
              var oldVm = newGroupVm.ActivityStatistics.SingleOrDefault(x => x.Range == range);

              if(oldVm != null)
              {
                oldVm.Activities = allTypedActivities;
                newVm = oldVm;
              }
              else
              {
                if (range != StatisticsRange.Total)
                {
                  newGroupVm.ActivityStatistics.Add(newVm);
                }
                else
                {
                  newGroupVm.Total = newVm;
                }
              }
            }
            else
            {
              if(range != StatisticsRange.Total)
              {
                newGroupVm.ActivityStatistics.Add(newVm);
              }
              else
              {
                newGroupVm.Total = newVm;
              }
            }

            await newVm.CalculateStats();
          }

          list.Add(newGroupVm);
        }

        var totalGroup = new ActivityStatisticsGroupViewModel();

        if (oldList != null)
        {
          totalGroup = oldList.SingleOrDefault(x => x.ActivityType == null);

          if(totalGroup == null)
          {
            totalGroup = new ActivityStatisticsGroupViewModel()
            {
              ActivityStatistics = new List<ActivityStatisticsViewModel>()
            };
          }  
        }

        foreach (StatisticsRange range in Enum.GetValues(typeof(StatisticsRange)))
        {
          var newVm = new ActivityStatisticsViewModel()
          {
            Range = range,
            Date = DateTime.Now,
            Activities = activities
          };
    

          if (totalGroup.ActivityStatistics.Count > 0)
          {
            var oldVm = totalGroup.ActivityStatistics.SingleOrDefault(x => x.Range == range);

            if (oldVm != null)
            {
              oldVm.Activities = activities;
              newVm = oldVm;
            }
            else
            {
              if (range != StatisticsRange.Total)
              {
                totalGroup.ActivityStatistics.Add(newVm);
              }
              else
              {
                totalGroup.Total = newVm;
              }
            }
           
          }
          else
          if (range != StatisticsRange.Total)
          {
            totalGroup.ActivityStatistics.Add(newVm);
          }
          else
          {
            totalGroup.Total = newVm;
          }

          await newVm.CalculateStats();
        }


        list.Add(totalGroup);


        return list.AsEnumerable();
      });
    }

    #endregion


  }
}
