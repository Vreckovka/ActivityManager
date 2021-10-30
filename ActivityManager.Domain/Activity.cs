using System;
using DomainCore.BaseDomainClasses;

namespace ActivityManager.Domain
{
  public class Activity : DatedEntity
  {
    public ActivityType Type { get; set; }

    public double DistanceInKm { get; set; }

    public double Inclination { get; set; }

    public long DurationTicks { get; set; }
  }
}
