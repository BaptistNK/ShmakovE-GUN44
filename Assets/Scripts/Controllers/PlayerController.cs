using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public bool IsInputBlocked {  get; private set; }
    [SerializeField] private float moveSpeed = 2f; 
    private DiContainer _container;
    [Inject]
    public void Construct(DiContainer container)
    {
        this._container = container;
    }
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
        IsInputBlocked = true; 
        Cell oldCell = checker.CurrentCell;
        if (oldCell != null) oldCell.SetChecker(null);
        
        checker.MoveToCell(targerCell);
        targerCell.SetChecker(checker);
        
        checker.transform.SetParent(targerCell.transform);

        Vector3 targetPosition = targerCell.transform.position + Vector3.up * .6f;

        while (Vector3.Distance(checker.transform.position, targetPosition) > .01f)
        {
            checker.transform.position = Vector3.MoveTowards(checker.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        checker.transform.position = targetPosition;

        if (checkerToRemove != null) 
        {
            Destroy(checkerToRemove.gameObject);
        }
        
        if(!checker.IsKing)
        {
            if ((checker.checkerTeam == Team.White && targerCell.Coordinates.y == 7) || (checker.checkerTeam == Team.Black && targerCell.Coordinates.y == 0)) 
            {
                Debug.Log($"[PlayerController] Шашка дошла до края! Координаты Y: {targerCell.Coordinates.y}. Запускаем превращение.");
                checker.PromoteToKing();
            }
        }
        
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
                var moveCmd = _container.Instantiate<MoveCommand>(new object[] {targerCell,nextJumps, true});
                battleController.SetCommand(moveCmd);
                IsInputBlocked = false;
                yield break;
            }
        }

        IsInputBlocked = false;
        battleController.SwitchTurn();
    }    
}
