using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CellPaletteSettings _cellPaletteSettings;
    public override void InstallBindings()
    {         
        Controls controls = new Controls();

        controls.Enable();

        Container.Bind<Controls.GameActions>().FromInstance(controls.Game).AsSingle();
        Container.Bind<CellPaletteSettings>().FromInstance(_cellPaletteSettings).AsSingle();
    }
}