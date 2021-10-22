using System;
using System.Collections.Generic;
using System.Text;
using ActivityManager.Providers;
using ActivityManager.ViewModels;
using VCore.Standard.Modularity.NinjectModules;

namespace ActivityManager.Modularity.Ninject
{
  public class ActivityManagerNinjectModule : BaseNinjectModule
  {
    public override void RegisterProviders()
    {
      base.RegisterProviders();

      Kernel.Bind<IActivitiesProvider>().To<ActivitiesProvider>().InSingletonScope();
    }
  }
}
