using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField] private PlayerSettings _playerSettings;
    public override void InstallBindings()
    {
        if(_playerSettings == null)
        {
            Debug.LogError("[SceneInstaller] Нет ассета PlayerSettings");
        }
        Container.Bind<PlayerSettings>().FromScriptableObject(_playerSettings).AsSingle();
    }
}