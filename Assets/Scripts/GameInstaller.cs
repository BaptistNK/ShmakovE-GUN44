using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CellPaletteSettings _cellPaletteSettings;

    [Header("Scene References")]
    [SerializeField] private BattleController _battleController;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Battlefield _battlefield;

    public override void InstallBindings()
    {         
        Controls controls = new Controls();
        controls.Enable();
        Container.Bind<Controls.GameActions>().FromInstance(controls.Game).AsSingle();
        Container.Bind<CellPaletteSettings>().FromInstance(_cellPaletteSettings).AsSingle();
        Container.Bind<BattleController>().FromInstance(_battleController).AsSingle();
        Container.Bind<PlayerController>().FromInstance(_playerController).AsSingle();
        Container.Bind<Battlefield>().FromInstance(_battlefield).AsSingle();
        Container.Bind<IGameplayCommand>().To<CheckersMoveCommand>().AsSingle();
    }
}