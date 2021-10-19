using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ActivityManager.Core;
using ActivityManager.Domain;
using ActivityManager.ViewModels.Modals;
using ActivityManager.Views;
using ActivityManager.Views.Modals;
using VCore;
using VCore.ItemsCollections;
using VCore.Modularity.RegionProviders;
using VCore.Standard.Factories.ViewModels;
using VCore.ViewModels;
using VCore.WPF.Managers;
using VPlayer.WindowsPlayer.Behaviors;

namespace ActivityManager.ViewModels
{
  public class ActivitiesViewModel : RegionViewModel<ActivitiesView>
  {
    private readonly IWindowManager windowManager;
    private readonly IViewModelsFactory viewModelsFactory;
    private string path = "Data/Activities.txt";

    public ActivitiesViewModel(IRegionProvider regionProvider, IWindowManager windowManager, IViewModelsFactory viewModelsFactory) : base(regionProvider)
    {
      this.windowManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));
      this.viewModelsFactory = viewModelsFactory ?? throw new ArgumentNullException(nameof(viewModelsFactory));
    }

    #region Properties

    public override string Header => "Aktivity";
    public override string RegionName { get; protected set; } = RegionNames.MainContent;

    #region Activities

    private RxObservableCollection<ActivityViewModel> activities = new RxObservableCollection<ActivityViewModel>();

    public RxObservableCollection<ActivityViewModel> Activities
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

      if (Activities.Any())
      {
        newOrder = Activities.Max(x => x.Model.Order) + 1;
      }
    
      vm.NewActivity.Order = newOrder;

      var result = windowManager.ShowQuestionPrompt<AddActivity, NewActivityViewModel>(vm);

      if (result == VCore.WPF.ViewModels.Prompt.PromptResult.Ok)
      {
        var model = vm.NewActivity;
        var newVm = viewModelsFactory.Create<ActivityViewModel>(model);

        Activities.Add(newVm);

        SaveAcitvities();
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

    private void LoadActivities()
    {
      if (File.Exists(path))
      {
        var json = File.ReadAllText(path);

        var activities = JsonSerializer.Deserialize<IEnumerable<Activity>>(json);

        Application.Current.Dispatcher.Invoke(() =>
        {
          Activities =
            new RxObservableCollection<ActivityViewModel>(activities.Select(x =>
              viewModelsFactory.Create<ActivityViewModel>(x)));

        });
      }
    }

    #endregion

    #region SaveAcitvities

    private void SaveAcitvities()
    {
      var json = JsonSerializer.Serialize(Activities.Select(x => x.Model));

      path.EnsureDirectoryExists();

      File.WriteAllText(path, json);
    }

    #endregion 

    #endregion
  }
}