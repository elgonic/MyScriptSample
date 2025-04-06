using UnityEngine;

/// <summary>
/// 進行方向の管理クラス
/// </summary>
/// <remarks>
/// IsLookMoveDirection は　重力制御してない弾とかに使うといいかも
/// 重力制御とかしてるやつはうまく動かんかも
/// </remarks>
public class MoveDirection : MonoBehaviour
{

    public bool IsLookMoveDirection = false;
    public bool WhenLookMoveDirectionIgnoreY = false;
    [Header("オブジェクト位置がFixedUpdateで更新されているとき")]
    [SerializeField] private bool _isFixedUpdate = true;

    /// <summary>
    /// 静止状態でも RigidBodyの velocity などが 微小な値を取るので この値以下を0とする
    /// </summary>
    private static readonly float _directionThreshold = 0.0001f;
    private float _directionThreshold_Sqr = _directionThreshold * _directionThreshold;


    /// <summary>
    /// 移動量も考慮した方向
    /// </summary>
    public Vector3 MoveValue
    {
        get
        {
            if (_direction.sqrMagnitude < _directionThreshold_Sqr)
            {
                return Vector3.zero;
            }
            return _direction;
        }

    }


    /// <summary>
    /// 単位ベクトルの方向
    /// </summary>
    //public Vector3 Direction => _direction.normalized;
    public Vector3 Direction
    {
        get
        {
            if (_direction.sqrMagnitude < _directionThreshold_Sqr)
            {
                return Vector3.zero;
            }
            return _direction.normalized;
        }
    }


    private Rigidbody _rb;

    private Vector3 _direction;
    private Vector3 _lookDirection;
    private Transform _transform;
    private Vector3 _beforPosition;


    private void OnValidate()
    {
        if (WhenLookMoveDirectionIgnoreY)
        {
            IsLookMoveDirection = true;
        }
    }
    private void Awake()
    {
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _transform = transform;
        _beforPosition = _transform.position;
        _direction = _transform.position - _beforPosition;
    }


    private void Update()
    {
        if (!_isFixedUpdate) OnUpdate();

    }

    private void FixedUpdate()
    {
        if (_isFixedUpdate) OnUpdate();
    }


    private void OnUpdate()
    {
        //RigidBodyがある場合はvelocityを使う
        if (_rb)
        {
            _direction = _rb.velocity;
        }
        else
        {
            //RigidBody無い場合はTransformを使用する
            _direction = _transform.position - _beforPosition;
        }

        //進行方向を向く処理
        if (IsLookMoveDirection)
        {
            _lookDirection = _direction;
            if (WhenLookMoveDirectionIgnoreY)
            {
                _lookDirection = new Vector3(_direction.x, 0, _direction.z);
            }

            if (IsLookMoveDirection && _lookDirection != Vector3.zero)
            {
                _transform.rotation = Quaternion.LookRotation(_lookDirection);
            }
        }


        _beforPosition = _transform.position;
    }
}
