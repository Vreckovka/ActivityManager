using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ActivityManager.Domain;
using ActivityManager.ViewModels;
using VCore;

namespace ActivityManager.Providers
{
  public class ActivitiesProvider : IActivitiesProvider
  {
    private string path = "Data/Activities.txt";
    public List<Activity> CachedActivities { get; private set; } = new List<Activity>();

    #region LoadActivitiesAsync

    public Task<IEnumerable<Activity>> LoadActivitiesAsync()
    {
      return Task.Run(() =>
      {
        if (File.Exists(path) && CachedActivities.Count == 0)
        {
          var json = File.ReadAllText(path);

          var loadedActivitiesFromFile = JsonSerializer.Deserialize<IEnumerable<Activity>>(json);

          CachedActivities = new List<Activity>(loadedActivitiesFromFile);
        }

        return CachedActivities.AsEnumerable();
      });
    }

    #endregion

    #region SaveAcitvitiesAsync

    public Task SaveAcitvitiesAsync()
    {
      return Task.Run(() =>
      {
        var json = GetJsonActivities();
        
        path.EnsureDirectoryExists();

        File.WriteAllText(path, json);
      });
    }

    #endregion

    public string GetJsonActivities()
    {
      return JsonSerializer.Serialize(CachedActivities);
    }

    #region AddActivityToCache

    public void AddActivityToCache(Activity activity)
    {
      CachedActivities.Add(activity);
    }

    #endregion

    #region RemoveActivityFromCache

    public void RemoveActivityFromCache(Activity activity)
    {
      CachedActivities.Remove(activity);
    }

    #endregion
  }
}