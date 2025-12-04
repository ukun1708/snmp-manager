using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace mySdk
{
    public class MySdk : MonoBehaviour
    {
        public Platform Platform;
        private Language language;
        public SaveSystem SaveSystem { get; private set; }
        public AdManager AdManager { get; private set; }
        public LeaderboardManager LeaderboardManager { get; private set; }
        public AnaliticsManager AnaliticsManager { get; private set; }
        public LocalisationManager LocalisationManager { get; private set; }
        public bool IsInitialized { get; private set; }

        private const string SDK_VERSION = "0.0.1";

        public bool IsMobile { get; private set; }

        private List<LocalisationDataSO> _localisationDatas = new();

        private void Awake()
        {
            InitSdkAsync();
        }

        private async void InitSdkAsync()
        {
            await CreateManagersAsync();

            SaveSystem.Init();
            //AdManager.Init();
            //LeaderboardManager.Init();
            //AnaliticsManager.Init();
            //language = GetLanguage();
            //_localisationDatas.AddRange(LoadLocalisationDatas());
            //LocalisationManager = new(language, _localisationDatas);
            //LocalisationManager.CreateDictionary();

            //Application.targetFrameRate = 60;

            await UniTask.Yield();

            Debug.Log($"SDK version:{SDK_VERSION} init");

            IsInitialized = true;
        }

        private Language GetLanguage()
        {
            if (Application.systemLanguage == SystemLanguage.Russian)
            {
                return Language.Russian;
            }
            else
            {
                return Language.English;
            }
        }

        private LocalisationDataSO[] LoadLocalisationDatas() => Resources.LoadAll<LocalisationDataSO>("Localisation");

        private async UniTask CreateManagersAsync()
        {
            SaveSystem = new SaveSystemWithData(platform: Platform);
            //AdManager = new();
            //LeaderboardManager = new();
            //AnaliticsManager = new();

            await UniTask.Yield();
        }
    }
    public enum Platform
    {
        None
    }
    public enum Language
    {
        Russian,
        English
    }
}

