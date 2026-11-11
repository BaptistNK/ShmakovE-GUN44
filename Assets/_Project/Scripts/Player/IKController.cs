using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private FootIKSettings _settings;

    [Header("IK Constraints & Targets")]
    [SerializeField] private TwoBoneIKConstraint _leftFootIK;
    [SerializeField] private TwoBoneIKConstraint _rightFootIK;
    [SerializeField] private Transform _leftFootTarget;
    [SerializeField] private Transform _rightFootTarget;

    [Header("Skeleton & Model")]
    [SerializeField] private Transform _modelRoot;
    [SerializeField] private Transform _leftFootBone;
    [SerializeField] private Transform _rightFootBone;

    private Animator _animator;
    private CharacterController _characterController;
    private float _leftFootWeight;
    private float _rightFootWeight;
    private float _currentPelvisOffset;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        if (_animator == null || _settings == null) return;

        // Проверяем, двигается ли персонаж (если бежит — плавно снижаем влияние IK)
        float moveSpeed = _characterController != null ? new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude : 0f;
        bool isMoving = moveSpeed > 0.15f;

        bool leftHit = GetFootHit(_leftFootBone, out RaycastHit leftHitInfo);
        bool rightHit = GetFootHit(_rightFootBone, out RaycastHit rightHitInfo);

        AdjustPelvis(leftHit, leftHitInfo, rightHit, rightHitInfo, isMoving);

        ApplyFootIK(_leftFootBone, _leftFootTarget, _leftFootIK, leftHit, leftHitInfo, isMoving, ref _leftFootWeight);
        ApplyFootIK(_rightFootBone, _rightFootTarget, _rightFootIK, rightHit, rightHitInfo, isMoving, ref _rightFootWeight);
    }

    private bool GetFootHit(Transform footBone, out RaycastHit hit)
    {
        hit = default;
        if (footBone == null) return false;

        Vector3 rayOrigin = footBone.position + Vector3.up * _settings.RaycastStartHeight;
        return Physics.Raycast(rayOrigin, Vector3.down, out hit, _settings.RaycastDistance, _settings.GroundLayer);
    }

    private void AdjustPelvis(bool leftHit, RaycastHit leftInfo, bool rightHit, RaycastHit rightInfo, bool isMoving)
    {
        if (_modelRoot == null) return;

        float targetOffset = 0f;

        if (!isMoving && leftHit && rightHit)
        {
            float leftDelta = leftInfo.point.y - transform.position.y;
            float rightDelta = rightInfo.point.y - transform.position.y;
            float lowestDelta = Mathf.Min(leftDelta, rightDelta);

            if (lowestDelta < -0.02f)
            {
                targetOffset = lowestDelta;
            }
        }

        _currentPelvisOffset = Mathf.Lerp(_currentPelvisOffset, targetOffset, Time.deltaTime * _settings.PelvisBlendSpeed);

        Vector3 localPos = _modelRoot.localPosition;
        localPos.y = _currentPelvisOffset;
        _modelRoot.localPosition = localPos;
    }


    private void ApplyFootIK(Transform footBone, Transform ikTarget, TwoBoneIKConstraint constraint, bool hasHit, RaycastHit hit, bool isMoving, ref float currentWeight)
    {
        if (footBone == null || ikTarget == null || constraint == null) return;

        if (hasHit && !isMoving)
        {
            // Берем X и Z текущей стопы, а Y ставим по поверхности с учетом смещения
            Vector3 targetPosition = new Vector3(footBone.position.x, hit.point.y + _settings.FootOffset, footBone.position.z);

            float heightDiff = footBone.position.y - hit.point.y;
            float targetWeight = (heightDiff < _settings.MaxStepHeight) ? 1f : 0f;

            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * _settings.IKBlendSpeed);
            constraint.weight = currentWeight;

            ikTarget.position = targetPosition;
            ikTarget.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * footBone.rotation;
        }
        else
        {
            // При ходьбе и беге отключаем IK, чтобы анимация шага играла полностью свободно
            currentWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * _settings.IKBlendSpeed);
            constraint.weight = currentWeight;

            // Держим Target рядом со стопой, чтобы при остановке не было рывка
            ikTarget.position = footBone.position;
            ikTarget.rotation = footBone.rotation;
        }
    }
}
