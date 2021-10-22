using System.ComponentModel;

namespace ActivityManager.ViewModels.Statistics
{
  public enum StatisticsRange
  {
    [Description("Deň")]
    Day,
    [Description("Týždeň")]
    Week,
    [Description("Mesiac")]
    Month,
    [Description("Rok")]
    Year,
    [Description("Celkovo")]
    Total
  }
}