using System;

namespace mySdk
{
    public abstract class SaveSystem
    {
        protected const string KEY_SAVE_GAMEPUSH = "save";
        protected string _keyLocalSave = "saveLocal";
        protected static string _cloudStringData;
        protected bool _isCloudSaveAvailable;
        protected DateTime _lastSaveTime;
        protected readonly TimeSpan _timeForSave;
        private readonly string _keySaveTimeGamePush = "SAVE_TIME";
        public PlayerData PlayerData;

        protected SaveSystem(Platform platform)
        {
            _isCloudSaveAvailable = platform switch
            {
                Platform.None => false,
                _ => true,
            };
        }

        public abstract T Load<T>() where T : SaveData, new();
        public abstract void Save<T>(T data, bool forcePushOnServer = false) where T : SaveData, new();
        public abstract void Init();

        public abstract void ResetData();
    }
}