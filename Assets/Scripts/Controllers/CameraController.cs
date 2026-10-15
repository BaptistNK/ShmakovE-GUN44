using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    public void RotateToTeam(Team currentTeam, System.Action onCompleteCallback)
    {
        float targetYRotation = (currentTeam == Team.White) ? 0f : 180f;
        transform.DORotate(new Vector3(0, targetYRotation, 0), duration, RotateMode.Fast)
            .SetEase(easeType)
            .OnComplete(() => onCompleteCallback?.Invoke());

    }
}
