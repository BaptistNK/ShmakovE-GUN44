using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public bool IsBusy {  get; private set; }
    private BattleController _battleController;
    [Inject]
    public void Construct(BattleController battleController)
    {
        _battleController = battleController;
    }

    public void ExecuteMove(Cell sourceCell, Cell targetCell, Cell killedCell)
    {
        if(IsBusy) return;
        Unit movingUnit = sourceCell.CurrentUnit;
        if(movingUnit==null) return;
        IsBusy = true;
        sourceCell.CurrentUnit = null;
        targetCell.CurrentUnit = movingUnit;
        movingUnit.CurrentCell = targetCell;

        DG.Tweening.Sequence moveSequence = DOTween.Sequence();

        if(killedCell!=null && killedCell.CurrentUnit != null )
        {
            Unit deadUnit = killedCell.CurrentUnit;
            killedCell.CurrentUnit = null;
            // Плавное уменьшение масштаба съеденной фигуры
            moveSequence.Join(deadUnit.transform.DOScale(Vector3.zero, 0.2f));
            moveSequence.OnComplete(() => Destroy(deadUnit.gameObject));
        }

        Vector3 targetPosition = targetCell.transform.position;
        targetPosition.y = movingUnit.transform.position.y;
        // Плавное перемещение фигуры к новой клетке
        moveSequence.Append(movingUnit.transform.DOMove(targetPosition, 0.4f)
                .SetEase(Ease.OutQuad));

        moveSequence.OnComplete(() =>
        {
            // Проверяем превращение в дамку (если шашка дошла до противоположного края)
            CheckKingTransformation(movingUnit);

            // Снимаем блокировку ввода
            IsBusy = false;

            // Передаем ход следующему игроку
            _battleController.SwitchTurn();
        });
    }

    private void CheckKingTransformation(Unit unit)
    {
        if(unit.Type == UnitType.King) return;
        Vector2Int coords = unit.CurrentCell.Coordinates;

        if((unit.Team == Team.White&&coords.y==7) || (unit.Team==Team.Black && coords.y ==0))
        {
            unit.Type = UnitType.King;

            unit.transform.DOScale(unit.transform.localScale * 1.3f, 0.3f);
            Debug.Log($"[PlayerController] Фишка на {coords} превратилась в дамку");
        }
    }

}
