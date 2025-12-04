using mySdk;
using UnityEngine;
using Zenject;

public class SdkInstaller : MonoInstaller
{
    [SerializeField] private MySdk sdk;

    public override void InstallBindings()
    {
        Container.BindInstance(sdk);
    }
}