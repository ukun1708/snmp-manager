using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using mySdk;
using Cysharp.Threading.Tasks;

public class LocalisationText : MonoBehaviour
{
    [SerializeField] private string _key;

    [Inject] private MySdk sdk;

    private async void Start()
    {
        await UniTask.WaitUntil(() => sdk.IsInitialized);

        if (TryGetComponent(out TMP_Text textMeshProUGUI))
            textMeshProUGUI.text = sdk.LocalisationManager.GetText(_key);
        else if (TryGetComponent(out Text textLegacy))
            textLegacy.text = sdk.LocalisationManager.GetText(_key);
    }
}

