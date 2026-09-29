using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsInputBlocked {  get; private set; }

    public void SetBlock(bool _isBlock)
    {
        IsInputBlocked = _isBlock;
    }
}
