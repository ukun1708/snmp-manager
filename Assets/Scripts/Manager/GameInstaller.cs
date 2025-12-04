using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SensorManager sensorManager;

    public override void InstallBindings()
    {
        Container.BindInstance(gameManager);
        Container.BindInstance(sensorManager);
    }
}