using System;
using System.Collections.Generic;
using System.Text;
using ActivityManager.Domain;
using VCore.Standard;

namespace ActivityManager.ViewModels
{
  public class ActivityViewModel : ViewModel<Activity>
  {
    public ActivityViewModel(Activity model) : base(model)
    {
    }
  }
}
