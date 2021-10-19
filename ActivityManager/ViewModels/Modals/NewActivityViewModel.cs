using System;
using System.Collections.Generic;
using System.Text;
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
          RaisePropertyChanged();
        }
      }
    } 
  }
}
