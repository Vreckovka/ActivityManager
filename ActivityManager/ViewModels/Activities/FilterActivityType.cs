using System.ComponentModel;

namespace ActivityManager.ViewModels.Activities
{
  public enum FilterActivityType
  {
    [Description("Žiadny")]
    None,
    [Description("Beh")]
    Run,
    [Description("Bicykel")]
    Bicycle,
    [Description("Chôdza")]
    Walk
  }
}