using Cysharp.Threading.Tasks;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using mySdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MinimalSNMP : MonoBehaviour
{
    [SerializeField] private string community = "public";

    private List<float> temps = new List<float> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    [Header("UI References")]
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private Image validationIndicator;
    [SerializeField] private TextMeshProUGUI validationText;
    [SerializeField] private Button readBtn;
    [SerializeField] private TMP_Text btnText;
    [SerializeField] private TMP_Text temperLimitText;
    [SerializeField] private Button settingsButton;
    [SerializeField] private TMP_Text sensorSelectIndicator;
    [SerializeField] private TMP_InputField updateRate;

    [Header("Colors")]
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    private bool launch = false;
    private float timer;
    private Coroutine coroutine;

    [Inject] private GameManager gameManager;
    [Inject] private MySdk sdk;
    [Inject] private SensorManager sensorManager;

    private void OnEnable() => gameManager.OnGameStateChanged += GameStateChanged;

    private void OnDisable() => gameManager.OnGameStateChanged -= GameStateChanged;

    private void GameStateChanged(GameState state)
    {
        if (state == GameState.init)
        {
            Init();
        }
    }

    private void Init()
    {
        ipInput.onValueChanged.AddListener(OnIPInputChanged);

        LoadData();

        if (readBtn != null)
        {
            readBtn.onClick.AddListener(() =>
            {
                if (sensorManager.GetActiveSensorChack())
                {
                    print(sensorManager.GetCriticalTemperature());

                    foreach (var display in sensorManager.GetDisplays())
                    {
                        display.text = "0°C";
                    }

                    sensorSelectIndicator.text = "";

                    if (launch == false)
                    {
                        launch = true;
                        btnText.text = "Приостановить";

                        ipInput.interactable = false;

                        settingsButton.interactable = false;

                        SavePlayerData(true);
                    }
                    else
                    {
                        launch = false;
                        btnText.text = "Подключится";

                        ipInput.interactable = true;

                        settingsButton.interactable = true;

                        timer = 2f;

                        if (coroutine != null)
                        {
                            StopCoroutine(coroutine);
                        }
                    }
                }
                else
                {
                    sensorSelectIndicator.text = $"<color=green>Нет выбранных датчиков</color>";
                }
            });
        }
    }

    private void LoadData()
    {
        ipInput.text = gameManager.PlayerData.ip;
        updateRate.text = gameManager.PlayerData.updateRate;
        timer = 2;
    }

    public void SavePlayerData(bool forcePushOnServer = false)
    {
        gameManager.PlayerData.ip = ipInput.text;
        gameManager.PlayerData.updateRate = updateRate.text;

        gameManager.SavePlayerData(true);
    }

    public void ResetData()
    {
        sdk.SaveSystem.ResetData();
    }

    private void Update()
    {
        if (launch == true)
        {
            timer += Time.deltaTime;

            if (timer >= Convert.ToSingle(updateRate.text))
            {
                timer = 0f;
                coroutine = StartCoroutine(ReadAll());
            }
        }
    }

    private void OnIPInputChanged(string input)
    {
        bool isValid = ValidateIPv4(input);

        // Визуальная обратная связь
        validationIndicator.color = isValid ? validColor : invalidColor;
        validationText.text = isValid ? "✓ Корректный IP" : "✗ Неверный формат";
        validationText.color = isValid ? validColor : invalidColor;

        // Активируем/деактивируем кнопку
        readBtn.interactable = isValid;
    }

    public static bool ValidateIPv4(string ipString)
    {
        if (string.IsNullOrWhiteSpace(ipString))
            return false;

        string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        return Regex.IsMatch(ipString, pattern);
    }

    private IEnumerator ReadAll()
    {
        List<string> oids = new List<string>();

        oids = sensorManager.GetAllOids();

        if (ipInput.text == null)
        {
            for (int i = 0; i < oids.Count; i++) // oidInputs.Count
            {
                if (temps[i] == -99)
                {
                    sensorManager.GetDisplays()[i].text = "<color=green> X </color>";
                }
                else
                {
                    if (temps[i] > sensorManager.GetCriticalTemperature())
                    {
                        sensorManager.GetDisplays()[i].text = $"<color=red> {temps[i]}°C </color>";
                        print("Alarm Critical Temperature");
                    }
                    else
                    {
                        UpdateDisplay(i, true);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < oids.Count; i++) // oidInputs.Count
            {
                yield return StartCoroutine(ReadSensor(i));

                if (temps[i] == -99)
                {
                    sensorManager.GetDisplays()[i].text = "<color=green> x </color>";
                }
                else
                {
                    if (temps[i] > sensorManager.GetCriticalTemperature())
                    {
                        sensorManager.GetDisplays()[i].text = $"<color=red> {temps[i]}°C </color>";
                        print("Alarm Critical Temperature");
                    }
                    else
                    {
                        UpdateDisplay(i);
                    }
                }
            }
        }
    }

    private IEnumerator ReadSensor(int index)
    {
        bool done = false;

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var result = Messenger.Get(
                    VersionCode.V1,
                    new IPEndPoint(IPAddress.Parse(ipInput.text), 161),
                    new OctetString(community),
                    new List<Variable> { new Variable(new ObjectIdentifier(sensorManager.GetAllOids()[index])) },
                    3000
                );

                if (result != null && result.Count > 0)
                {
                    temps[index] = ParseValue(result[0].Data);
                }
                else
                {
                    temps[index] = -999f;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Sensor {index}<color=green> error: </color>{ex.Message}");
                temps[index] = -999f;
            }
            done = true;
        });

        yield return new WaitUntil(() => done);
    }

    private float ParseValue(ISnmpData data)
    {
        if (data is OctetString octet)
        {
            if (float.TryParse(octet.ToString(), out float val))
                return val;
        }
        else if (data is Integer32 int32)
            return int32.ToInt32();
        else if (data is Gauge32 gauge)
            return (int)gauge.ToUInt32(); // Исправлено здесь
        else if (data is TimeTicks ticks)
            return (int)ticks.ToUInt32();

        return -999f;
    }

    private void UpdateDisplay(int index, bool ip = false)
    {
        if (ip == true)
        {
            if (index < sensorManager.GetDisplays().Count && index >= 0 && sensorManager.GetDisplays()[index] != null)
            {
                if (temps[index] > -100)
                {
                    //sensorManager.GetDisplays()[index].text = $"Датчик {index}: <color=green>{temps[index]:F1}°C</color>";
                    sensorManager.GetDisplays()[index].text = $"<color=green>{temps[index]:F1}°C</color>";
                }
                else
                {
                    sensorManager.GetDisplays()[index].text = "Введите IP";
                }
            }
        }
        else
        {
            if (index < sensorManager.GetDisplays().Count && index >= 0 && sensorManager.GetDisplays()[index] != null)
            {
                if (temps[index] > -100)
                {
                    //sensorManager.GetDisplays()[index].text = $"Датчик {index}: <color=green>{temps[index]:F1}°C</color>";
                    sensorManager.GetDisplays()[index].text = $"<color=green>{temps[index]:F1}°C</color>";
                }
                else
                {
                    sensorManager.GetDisplays()[index].text = "<color=green> error: </color>";
                }
            }
        }
    }
}