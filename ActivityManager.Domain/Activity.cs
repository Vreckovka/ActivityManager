using System;
using DomainCore.BaseDomainClasses;

namespace ActivityManager.Domain
{
  public class Activity : DatedEntity
  {
    public int Order { get; set; }

    public ActivityType Type { get; set; }
  }
}
