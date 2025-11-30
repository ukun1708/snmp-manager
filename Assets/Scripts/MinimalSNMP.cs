using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using TMPro;

public class MinimalSNMP : MonoBehaviour
{
    public string ip = "192.168.1.100";
    public string community = "public";
    public List<string> oids = new List<string> { "1.3.6.1.4.1.X.1.1.1.0", "1.3.6.1.4.1.X.1.1.2.0", "1.3.6.1.4.1.X.1.1.3.0" };
    public List<TMP_Text> displays;
    public Button readBtn;

    private List<float> temps = new List<float> { 0, 0, 0 };

    void Start()
    {
        if (readBtn != null)
        {
            readBtn.onClick.AddListener(() => StartCoroutine(ReadAll()));
        }
    }

    private IEnumerator ReadAll()
    {
        for (int i = 0; i < oids.Count; i++)
        {
            yield return StartCoroutine(ReadSensor(i));
            UpdateDisplay(i);
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
                    new IPEndPoint(IPAddress.Parse(ip), 161),
                    new OctetString(community),
                    new List<Variable> { new Variable(new ObjectIdentifier(oids[index])) },
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
                Debug.LogError($"Sensor {index} error: {ex.Message}");
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

    private void UpdateDisplay(int index)
    {
        if (index < displays.Count && index >= 0 && displays[index] != null)
        {
            displays[index].text = temps[index] > -100 ? $"{temps[index]:F1}°C" : "Error";
        }
    }
}