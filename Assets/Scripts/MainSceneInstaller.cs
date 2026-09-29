using Zenject;
using UnityEngine;

public class MainSceneInstaller : MonoInstaller
{
    [SerializeField] private SceneController _sceneController;
    public override void InstallBindings()
    {
        Container.Bind<SceneController>().FromInstance(_sceneController).AsSingle();
    }
}
