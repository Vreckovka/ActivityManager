using System;
using System.Collections.Generic;
using System.Text;
using ActivityManager.Core;
using ActivityManager.Views;
using VCore.Modularity.RegionProviders;
using VCore.ViewModels;

namespace ActivityManager.ViewModels
{
  public class StatisticsViewModel: RegionViewModel<StatisticsView>
  {
    public StatisticsViewModel(IRegionProvider regionProvider) : base(regionProvider)
    {
    }

    public override string Header => "Štatistiky";
    public override string RegionName { get; protected set; } = RegionNames.MainContent;
  }
}
