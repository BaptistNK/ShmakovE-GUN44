using UnityEngine;

[CreateAssetMenu(fileName = "FootIKSettings", menuName = "Configs/FootIKSettings")]
public class FootIKSettings : ScriptableObject
{
    [Header("Raycast Configuration")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _raycastStartHeight = 0.5f;
    [SerializeField] private float _raycastDistance = 1.2f;

    [Header("Foot Placement & Blending")]
    [SerializeField] private float _footOffset = 0.08f;
    [SerializeField] private float _ikBlendSpeed = 12f;
    [SerializeField] private float _maxStepHeight = 0.25f;

    [Header("Pelvis Adjustment")]
    [SerializeField] private float _pelvisBlendSpeed = 10f;

    public LayerMask GroundLayer => _groundLayer;
    public float RaycastStartHeight => _raycastStartHeight;
    public float RaycastDistance => _raycastDistance;
    public float FootOffset => _footOffset;
    public float IKBlendSpeed => _ikBlendSpeed;
    public float MaxStepHeight => _maxStepHeight;
    public float PelvisBlendSpeed => _pelvisBlendSpeed;
}
