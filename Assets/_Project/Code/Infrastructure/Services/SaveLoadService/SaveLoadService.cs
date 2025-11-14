using _Project.Code.Infrastructure.Data;
using _Project.Code.Infrastructure.Services.PersistentService;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Code.Infrastructure.Services.SaveLoadService
{
    public class SaveLoadService : ISaveLoadService
    {
        private const string PersistentKey = "Persistent";
        
        private readonly IPersistentService _persistentService;

        public SaveLoadService(IPersistentService persistent)
        {
            _persistentService = persistent;
        }
        
        public void Save()
        {
            string json = JsonConvert.SerializeObject(_persistentService.Data, Formatting.Indented, 
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            
            PlayerPrefs.SetString(PersistentKey, json);
        }

        public Data.PersistentData Load()
        {
            string json = PlayerPrefs.GetString(PersistentKey);

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<Data.PersistentData>(json);
        }
    }
}