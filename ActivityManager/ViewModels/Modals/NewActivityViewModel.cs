using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using ActivityManager.Domain;
using VCore.WPF.Prompts;

namespace ActivityManager.ViewModels.Modals
{
  public class NewActivityViewModel : BasePromptViewModel
  {
    public NewActivityViewModel()
    {
      newActivity = new Activity()
      {
        Created = DateTime.Now
      };
    }

    #region NewActivity

    private Activity newActivity;

    public Activity NewActivity
    {
      get
      {
        return newActivity;
      }
      set
      {
        if (newActivity != value)
        {
          newActivity = value;
          SetDuration();
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region DurationHours

    private double durationHours;

    public double DurationHours
    {
      get
      {
        return durationHours;
      }
      set
      {
        if (durationHours != value)
        {
          durationHours = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region DurationMinutes

    private double durationMinutes;

    public double DurationMinutes
    {
      get
      {
        return durationMinutes;
      }
      set
      {
        if (durationMinutes != value)
        {
          durationMinutes = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region DurationSeconds

    private double durationSeconds;

    public double DurationSeconds
    {
      get
      {
        return durationSeconds;
      }
      set
      {
        if (durationSeconds != value)
        {
          durationSeconds = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region GetFinalActivity

    public Activity GetFinalActivity()
    {
      newActivity.DurationTicks = TimeSpan.FromSeconds(DurationSeconds + (DurationMinutes * 60) + (DurationHours * 60 * 60)).Ticks;

      return newActivity;
    }

    #endregion

    #region SetDuration

    private void SetDuration()
    {
      if (newActivity.DurationTicks > 0)
      {
        var timeSpan = TimeSpan.FromTicks(newActivity.DurationTicks);

        var hours = timeSpan.TotalHours;
        var onlyHours = Math.Truncate(hours);

        var minutes = (hours - onlyHours) * 60;
        var onlyMinutes = Math.Truncate(minutes);

        var seconds = Math.Round((minutes - onlyMinutes) * 60);

        DurationHours = onlyHours;
        DurationMinutes = onlyMinutes;
        DurationSeconds = seconds;
      }
    }

    #endregion
  }
}
