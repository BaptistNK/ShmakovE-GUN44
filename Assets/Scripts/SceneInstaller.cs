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
<<<<<<< HEAD

        Container.Bind<InputManager>().AsSingle().NonLazy();

=======
        Container.Bind<CameraController>().FromComponentInHierarchy().AsSingle();
>>>>>>> 9ad8cf956a5709aa7b3251a11928408e187ff2be
        Container.BindFactory<SelectCheckerCommand, PlaceholderFactory<SelectCheckerCommand>>();
        Container.Bind<MoveCommand>().AsTransient();
    }
}