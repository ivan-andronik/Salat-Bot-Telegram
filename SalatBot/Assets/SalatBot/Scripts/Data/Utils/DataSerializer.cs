using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SalatBot.Scripts.Data.Utils
{
    public static class DataSerializer
    {
        public static T GetSerializedData<T>(string path) where T : new()
        {
            var item = new T();
            
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                item = JsonConvert.DeserializeObject<T>(json);

                Debug.Log(json);
            }

            return item;
        }

        public static void SerializeData<T>(T data, string path)
        {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, json);
        }
    }
}