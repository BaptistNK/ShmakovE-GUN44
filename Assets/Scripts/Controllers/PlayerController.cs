using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsInputBlocked {  get; private set; }
    [SerializeField] private float moveSpeed = 2f; //скорость движения шашки

    public void MoveChecker(Checker checker, Cell targetCell, BattleController battleController, Battlefield battlefield)
    {
        StartCoroutine(MoveCoroutine(checker, targetCell, battleController, battlefield));
    }

    public void SetBlock(bool _isBlock)
    {
        IsInputBlocked = _isBlock;
    }
    private IEnumerator MoveCoroutine(Checker checker, Cell targerCell, BattleController battleController, Battlefield battlefield)
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
        while (Vector3.Distance(checker.transform.position, targetPosition) > moveSpeed)
        {
            checker.transform.position = Vector3.MoveTowards(checker.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        //фиксируем точную позицию в конце движения
        checker.transform.position = targetPosition;
        //завершение хода
        IsInputBlocked = false;
        battleController.SwitchTurn();//Смена хода
    }
}
