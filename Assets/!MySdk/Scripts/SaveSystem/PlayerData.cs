using System;

namespace mySdk
{
    [Serializable]
    public class PlayerData : SaveData
    {
        public string ip;
        public bool[] activeSensors;
        public string[] sensorNames;
        public float criticalTemperature;
        public string updateRate;

        public PlayerData() : base()
        {
            ip = null;
            activeSensors = new bool[16];
            sensorNames = new string[16];
            criticalTemperature = 5f;
            updateRate = "3";
        }
    }
}

