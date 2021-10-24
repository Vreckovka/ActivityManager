using System.ComponentModel;

namespace ActivityManager.Domain
{
  public enum ActivityType
  {
    [Description("Beh")]
    Run,
    [Description("Bicykel")]
    Bicycle,
    [Description("Turistika")]
    Walk
  }
}