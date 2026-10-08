using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsInputBlocked {  get; private set; }
    [SerializeField] private float moveSpeed = 2f; //скорость движения шашки

    public void MoveChecker(Checker checker, Cell targetCell, BattleController battleController, Battlefield battlefield, Checker checkerToRemove, bool didCapture)
    {
        StartCoroutine(MoveRoutine(checker, targetCell, battleController, battlefield, checkerToRemove, didCapture));
    }

    public void SetBlock(bool _isBlock)
    {
        IsInputBlocked = _isBlock;
    }
    private IEnumerator MoveRoutine(Checker checker, Cell targerCell, BattleController battleController, Battlefield battlefield, Checker checkerToRemove, bool didCapture)
    {
        IsInputBlocked = true; //Запрещаем инпут
        //Отвязываем шашку от клетки
        Cell oldCell = checker.CurrentCell;
        if (oldCell != null) oldCell.SetChecker(null);
        //привязываепм шашку на новую клетку
        checker.MoveToCell(targerCell);
        targerCell.SetChecker(checker);
        //Делаем шашку дочкой клетки
        checker.transform.SetParent(targerCell.transform);

        Vector3 targetPosition = targerCell.transform.position + Vector3.up * .6f;

        while (Vector3.Distance(checker.transform.position, targetPosition) > .01f)
        {
            checker.transform.position = Vector3.MoveTowards(checker.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        //фиксируем точную позицию в конце движения
        checker.transform.position = targetPosition;

        if (checkerToRemove != null) 
        {
            Destroy(checkerToRemove.gameObject);
        }
        //превращение в дамку
        if(!checker.IsKing)
        {
            if ((checker.checkerTeam == Team.White && targerCell.Coordinates.y == 7) || (checker.checkerTeam == Team.Black && targerCell.Coordinates.y == 0)) 
            {
                Debug.Log($"[PlayerController] Шашка дошла до края! Координаты Y: {targerCell.Coordinates.y}. Запускаем превращение.");
                checker.PromoteToKing();
            }
        }
        //комбо
        if(didCapture)
        {
            List<Cell> nextJumps = SelectCheckerCommand.GetValidJumps(targerCell, battlefield, checker.checkerTeam);
            if(nextJumps.Count > 0)
            {
                battlefield.HighlightCell(targerCell, true, Color.red);
                foreach (Cell jumpCell in nextJumps)
                {
                    battlefield.HighlightCell(jumpCell, true, Color.green);
                }
                battleController.SetCommand(new MoveCommand(battlefield, battleController, this, targerCell, nextJumps));
                IsInputBlocked = false;
                yield break;
            }
        }
        //завершение хода
        IsInputBlocked = false;
        battleController.SwitchTurn();//Смена хода
    }    
}
