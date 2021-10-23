using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ActivityManager.Core;
using ActivityManager.Domain;
using ActivityManager.Providers;
using ActivityManager.ViewModels.Modals;
using ActivityManager.Views;
using ActivityManager.Views.Modals;
using VCore;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Helpers;
using VCore.WPF.Interfaces.Managers;
using VCore.WPF.ItemsCollections;
using VCore.WPF.Managers;
using VCore.WPF.Misc;
using VCore.WPF.Modularity.RegionProviders;
using VCore.WPF.ViewModels;

namespace ActivityManager.ViewModels.Activities
{
  public class ActivitiesViewModel : RegionViewModel<ActivitiesView>
  {
    private readonly IWindowManager windowManager;
    private readonly IViewModelsFactory viewModelsFactory;
    private readonly IActivitiesProvider activitiesProvider;


    public ActivitiesViewModel(
      IRegionProvider regionProvider,
      IWindowManager windowManager,
      IViewModelsFactory viewModelsFactory,
      IActivitiesProvider activitiesProvider) : base(regionProvider)
    {
      this.windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
      this.activitiesProvider = activitiesProvider ?? throw new ArgumentNullException(nameof(activitiesProvider));
    }

    #region Properties

    public override string Header => "Aktivity";
    public override string RegionName { get; protected set; } = RegionNames.MainContent;

    #region Activities

    private ItemsViewModel<ActivityViewModel> activities = new ItemsViewModel<ActivityViewModel>();

    public ItemsViewModel<ActivityViewModel> Activities
    {
      get
      {
        return activities;
      }
      set
      {
        if (value != activities)
        {
          activities = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region FilterByType

    private FilterActivityType filterByType;

    public FilterActivityType FilterByType
    {
      get
      {
        return filterByType;
      }
      set
      {
        if (value != filterByType)
        {
          filterByType = value;
          Filter();
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Commands

    #region AddActivity

    private ActionCommand addActivity;

    public ICommand AddActivity
    {
      get
      {
        return addActivity ??= new ActionCommand(OnAddActivity);
      }
    }

    private void OnAddActivity()
    {
      var vm = viewModelsFactory.Create<NewActivityViewModel>();

      int newOrder = 1;

      if (Activities.ViewModels.Any())
      {
        newOrder = Activities.ViewModels.Max(x => x.Model.Id) + 1;
      }

      vm.NewActivity.Id = newOrder;

      var result = windowManager.ShowQuestionPrompt<AddActivity, NewActivityViewModel>(vm);

      if (result == VCore.WPF.ViewModels.Prompt.PromptResult.Ok)
      {
        var model = vm.GetFinalActivity();
        var newVm = viewModelsFactory.Create<ActivityViewModel>(model);


        Activities.Add(newVm);
        activitiesProvider.AddActivityToCache(model);

        newVm.GetDuration();
        SaveAcitvities();
        OrderActivities();
      }
    }

    #endregion

    #region DeleteActivity

    private ActionCommand<ActivityViewModel> deleteActivity;

    public ICommand DeleteActivity
    {
      get
      {
        return deleteActivity ??= new ActionCommand<ActivityViewModel>(OnDeleteActitity);
      }
    }

    private void OnDeleteActitity(ActivityViewModel activityViewModel)
    {
      var result = windowManager.ShowDeletePrompt(activityViewModel.Model.Id.ToString(), "Odstránenie aktivity", "Skutočne chcete odstrániť aktivitu ");

      if (result == VCore.WPF.ViewModels.Prompt.PromptResult.Ok)
      {
        Activities.Remove(activityViewModel);

        activitiesProvider.AddActivityToCache(activityViewModel.Model);

        SaveAcitvities();
        OrderActivities();
      }
    }

    #endregion

    #region EditActivity

    private ActionCommand<ActivityViewModel> editActivity;

    public ICommand EditActivity
    {
      get
      {
        return editActivity ??= new ActionCommand<ActivityViewModel>(OnEditActivity);
      }
    }

    private void OnEditActivity(ActivityViewModel activityViewModel)
    {
      var vm = viewModelsFactory.Create<NewActivityViewModel>();

      vm.NewActivity = activityViewModel.Model;

      var result = windowManager.ShowQuestionPrompt<AddActivity, NewActivityViewModel>(vm);

      if (result == VCore.WPF.ViewModels.Prompt.PromptResult.Ok)
      {
        var model = vm.GetFinalActivity();
        var newVm = viewModelsFactory.Create<ActivityViewModel>(model);

        var index = Activities.ViewModels.IndexOf(activityViewModel);

        Activities.RemoveAt(index);
        Activities.Insert(index, newVm);

        newVm.GetDuration();
        SaveAcitvities();
        OrderActivities();
      }
    }

    #endregion

    #endregion

    #region Methods

    #region Initialize

    public override void Initialize()
    {
      base.Initialize();

      Task.Run(LoadActivities);
    }

    #endregion

    #region LoadActivities

    private async void LoadActivities()
    {
      var loadedActivities = (await activitiesProvider.LoadActivitiesAsync()).ToList();

      Application.Current.Dispatcher.Invoke(() =>
      {
        var vms = loadedActivities.Select(x => viewModelsFactory.Create<ActivityViewModel>(x)).ToList();
        vms = GetOrderedActivities(vms).ToList();

        Activities.AddRange(vms);
      });
    }

    #endregion

    private IEnumerable<ActivityViewModel> GetOrderedActivities(IEnumerable<ActivityViewModel> activityViewModels)
    {
      var list = activityViewModels.ToList();
      var oredered = list.OrderBy(x => x.Model.Created).ToList();

      for (int i = 0; i < oredered.Count; i++)
      {
        oredered[i].Model.Id = i + 1;
      }

      return list.OrderByDescending(x => x.Model.Created);
    }

    private void OrderActivities()
    {
      var vms = Activities.ViewModels.ToList();
      Activities.Clear();
      Activities.AddRange(GetOrderedActivities(vms));
    }

    #region SaveAcitvities

    private async void SaveAcitvities()
    {
      await activitiesProvider.SaveAcitvitiesAsync();
    }

    #endregion

    #region Filter

    private void Filter()
    {
      Activities.Filter((x) => FilterItemsByType(x, FilterByType));
    }

    #endregion

    #region FilterItemsByType

    private bool FilterItemsByType(ActivityViewModel activitiesViewModel, FilterActivityType filterActivityType)
    {
      if (filterActivityType == FilterActivityType.None)
      {
        return true;
      }
      else
      {
        var type = GetFilterActivityType(activitiesViewModel.Model.Type);

        return filterActivityType == type;
      }
    }

    #endregion

    #region GetFilterActivityType

    private FilterActivityType? GetFilterActivityType(ActivityType activityType)
    {
      switch (activityType)
      {
        case ActivityType.Run:
          return FilterActivityType.Run;
        case ActivityType.Bicycle:
          return FilterActivityType.Bicycle;
        case ActivityType.Walk:
          return FilterActivityType.Walk;
      }

      return null;
    }

    #endregion

    #endregion
  }
}