using Zenject;
using UnityEngine;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private InputSettings _inputSettings;
    public override void InstallBindings()
    {
        if(_inputSettings==null)
        {
            Debug.LogError("[ProjectInstaller] нет ассета InputSettings");
            return;
        }
        //InputService
        Container.Bind<InputSettings>().FromScriptableObject(_inputSettings).AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle().NonLazy();
    }
}
