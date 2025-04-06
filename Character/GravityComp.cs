using UnityEngine;



/// <summary>
/// 重力付与クラス
/// </summary>
/// <remarks>
/// このクラスで重力を与えることで,個々に独立した重力を与えられる
/// </remarks>
[RequireComponent(typeof(Rigidbody))]
public class GravityComp : MonoBehaviour
{
    public bool IsActive { get; private set; } = true;
    private CharacterParam _characterParam;

    [SerializeField] private float _initialGravityValue = 9.81f;
    [SerializeField, ReadOnly] private float _gravityValue;
    [SerializeField] private Vector3 _gravityDirection = Vector3.down;
    public float GravityValue { get { return _gravityValue; } set { _gravityValue = value; } }
    public Vector3 GravityDirection { get { return _gravityDirection; } set { _gravityDirection = value; } }



    private Rigidbody _rb;



    private void OnValidate()
    {
        _gravityValue = _initialGravityValue;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _characterParam = GetComponent<CharacterParam>();
        _gravityValue = _initialGravityValue;
    }

    private void FixedUpdate()
    {
        if (IsActive) _rb.AddForce(_gravityDirection * _gravityValue * _rb.mass);
    }
    public void Enable(float? gravityValue = null)
    {
        if (gravityValue.HasValue) _gravityValue = gravityValue.Value;
        IsActive = true;
    }

    public void Disable()
    {
        IsActive = false;
    }

    public void ResetGravityValue()
    {
        _gravityValue = _initialGravityValue;
    }
}
