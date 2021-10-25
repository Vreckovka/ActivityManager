using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ActivityManager.Modularity.Ninject;
using ActivityManager.ViewModels;
using ActivityManager.Views;
using Ninject;
using PCloudClient;
using VCore.WPF;
using VCore.WPF.Views.SplashScreen;

namespace ActivityManager
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  ///

  public class ActivityManagerApp : VApplication<MainWindow, ActivityManagerMainWindowViewModel, SplashScreenView>
  {
    protected override void LoadModules()
    {
      base.LoadModules();

      Kernel.Load<ActivityManagerNinjectModule>();

      Kernel.Bind<IPCloudService>().To<PCloudService>()
        .InSingletonScope()
        .WithConstructorArgument(ConfigurationManager.AppSettings["PCloudPath"])
        .OnActivation(x => x.Initilize());
    }
  }

  public partial class App : ActivityManagerApp
  {
  }
}
