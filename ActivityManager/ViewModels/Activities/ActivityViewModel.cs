using System;
using ActivityManager.Domain;
using VCore.Standard;

namespace ActivityManager.ViewModels.Activities
{
  public class ActivityViewModel : SelectableViewModel<Activity>
  {
    public ActivityViewModel(Activity model) : base(model)
    {
    
    }

    public override void Initialize()
    {
      base.Initialize();

      GetDuration();

    }

    public void GetDuration()
    {
      Duration = TimeSpan.FromTicks(Model.DurationTicks);
    }

    #region Duration

    private TimeSpan duration;

    public TimeSpan Duration
    {
      get
      {
        return duration;
      }
      set
      {
        if (value != duration)
        {
          duration = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion


  }
}
