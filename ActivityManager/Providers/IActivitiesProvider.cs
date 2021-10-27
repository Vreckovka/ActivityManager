using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityManager.Domain;

namespace ActivityManager.Providers
{
  public interface IActivitiesProvider
  {
    Task<IEnumerable<Activity>> LoadActivitiesAsync();
    Task SaveAcitvitiesAsync();
    string GetJsonActivities();
    void AddActivityToCache(Activity activity);
    void RemoveActivityFromCache(Activity activity);
  }
}