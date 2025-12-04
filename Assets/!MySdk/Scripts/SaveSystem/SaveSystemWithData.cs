using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace mySdk
{
    public class SaveSystemWithData : SaveSystem
    {
        public SaveSystemWithData(Platform platform) : base(platform) { }

        public override void Init()
        {
            Debug.Log("INIT SAVESYSTEM WITH DATA");

            PlayerData = Load<PlayerData>();
        }

        public override void ResetData()
        {
            PlayerPrefs.DeleteAll();
        }

        public override T Load<T>()
        {
            return LoadLocal();

            //T LoadCloud(string data) =>
            //    IsValidJson(data) ? JsonConvert.DeserializeObject<T>(data) : new();


            T LoadLocal()
            {
                var localStringData = PlayerPrefs.GetString(_keyLocalSave);
                return IsValidJson(localStringData)
                    ? JsonConvert.DeserializeObject<T>(localStringData)
                    : new();
            }
        }

        public override void Save<T>(T data, bool forcePushOnServer = false)
        {
            var stringData = JsonConvert.SerializeObject(data);
            
            if (_isCloudSaveAvailable)
                SaveCloud();
            else
                SaveLocal();
            return;

            void SaveLocal()
            {
                PlayerPrefs.SetString(_keyLocalSave, stringData);
            }

            void SaveCloud()
            {

            }
        }

        private bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                Debug.LogWarning("IsValidJson: false - IsNullOrWhiteSpace");
                return false;
            }

            jsonString = jsonString.Trim();


            if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) ||
                (jsonString.StartsWith("[") && jsonString.EndsWith("]")))
            {
                try
                {
                    var token = JToken.Parse(jsonString);

                    if (token.Type == JTokenType.Object && !token.HasValues)
                    {
                        Debug.LogWarning("IsValidJson: false - json has no value or object");
                        return false;
                    }
                    Debug.LogWarning("IsValidJson: true");
                    return true;
                }
                catch (JsonReaderException)
                {
                    Debug.LogWarning("IsValidJson: false - json cannot parse");
                    return false;
                }
            }
            Debug.LogWarning("IsValidJson: false - json doesn't starts or ends with { or [ symbols");
            return false;
        }
    }

    public class SaveData { }
}