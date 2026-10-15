using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private Battlefield battlefield;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleController battleController;

    public override void InstallBindings()
    {
        Container.Bind<Battlefield>().FromInstance(battlefield).AsSingle();
        Container.Bind<PlayerController>().FromInstance(playerController).AsSingle();
        Container.Bind<BattleController>().FromInstance(battleController).AsSingle();
        Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
        Container.BindFactory<SelectCheckerCommand, PlaceholderFactory<SelectCheckerCommand>>();
    }
}