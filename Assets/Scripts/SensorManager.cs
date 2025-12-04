using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SensorManager : MonoBehaviour
{
    private List<Toggle> toggles = new List<Toggle>();

    [SerializeField] private Transform togglesParent;
    [SerializeField] private List<string> oids;
    [SerializeField] private List<TMP_InputField> sensorsNames;
    [SerializeField] private List<TMP_Text> displays;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text criticalTempText;    

    [Inject] private GameManager gameManager;

    private void Awake()
    {
        foreach (Transform child in togglesParent)
        {
            toggles.Add(child.GetComponent<Toggle>());
        }

        slider.onValueChanged.AddListener(SliderValueChanged);
    }

    private void SliderValueChanged(float value)
    {
        criticalTempText.text = value.ToString("0");
    }

    private void OnEnable() => gameManager.OnGameStateChanged += GameStateChanged;

    private void OnDisable() => gameManager.OnGameStateChanged -= GameStateChanged;

    private void GameStateChanged(GameState state)
    {
        if (state == GameState.init)
        {
            LoadData();
        }
    }

    private void LoadData()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            toggles[i].isOn = gameManager.PlayerData.activeSensors[i];
            displays[i].gameObject.SetActive(gameManager.PlayerData.activeSensors[i]);
        }

        for (int i = 0; i < sensorsNames.Count; i++)
        {
            if (gameManager.PlayerData.sensorNames[i] == null)
            {
                gameManager.PlayerData.sensorNames[i] = sensorsNames[i].text;
            }
            else
            {
                sensorsNames[i].text = gameManager.PlayerData.sensorNames[i];
            }
        }

        slider.value = gameManager.PlayerData.criticalTemperature;
    }

    public void SaveData()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            gameManager.PlayerData.activeSensors[i] = toggles[i].isOn;
            displays[i].gameObject.SetActive(gameManager.PlayerData.activeSensors[i]);
        }

        gameManager.PlayerData.criticalTemperature = slider.value;
    }

    public List<string> GetActiveOids()
    {
        List<string> activeOids = new List<string>();

        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
            {
                activeOids.Add(oids[i]);
            }
        }

        return activeOids;
    }

    public List<TMP_Text> GetDisplays()
    {
        return displays;
    }

    public List<string> GetAllOids()
    {
        return oids;
    }

    public bool GetActiveSensorChack()
    {
        bool activeSensor = false;

        for (int i = 0; i < gameManager.PlayerData.activeSensors.Length; i++)
        {
            if (gameManager.PlayerData.activeSensors[i] == true)
            {
                activeSensor = true;
                break;
            }
        }

        return activeSensor;
    }

    public float GetCriticalTemperature()
    {
        return slider.value;
    }
}
