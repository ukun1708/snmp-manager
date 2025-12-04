using Cysharp.Threading.Tasks;
using mySdk;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public enum GameState
{
    init,
    start
}

public class GameManager : MonoBehaviour
{
    [Inject] private MySdk sdk;

    public GameState State;
    public event Action<GameState> OnGameStateChanged;
    public PlayerData PlayerData { get; set; }

    private async void Start()
    {
        await UniTask.WaitUntil(() => sdk.IsInitialized);
        PlayerData = sdk.SaveSystem.Load<PlayerData>();

        UpdateGameState(GameState.init);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void SavePlayerData(bool forcePushOnServer = false) => sdk.SaveSystem.Save(PlayerData, forcePushOnServer);
}
