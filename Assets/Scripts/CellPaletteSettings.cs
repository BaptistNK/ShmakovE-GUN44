using UnityEngine;

[CreateAssetMenu(fileName ="CellPaletteSettings", menuName ="Settings/Cell Palette")]
public class CellPaletteSettings : ScriptableObject
{
    [Header("Материалы выделения клеток")]
    [SerializeField] private Material _selectMaterial;
    [SerializeField] private Material _canMoveMaterial;
    [SerializeField] private Material _moveAndAttackMaterial;

    public Material GetMaterial(CellSelectType selectType)
    {
        return selectType switch
        {
            CellSelectType.Selected => _selectMaterial,
            CellSelectType.CanMove => _canMoveMaterial,
            CellSelectType.MoveAndAttack => _moveAndAttackMaterial,
            _ => null
        };
    }
}
