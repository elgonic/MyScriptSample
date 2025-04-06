using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// PlayerやEnemyなどのキャラクター管理クラス
/// </summary>

[RequireComponent(typeof(GravityComp)), RequireComponent(typeof(HpComp)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(MoveDirection))]
public class CharacterParam : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _isStandingGround;

    /// <summary>
    /// GroucCheckをRayCastで行うときは Olayerのコライダーの内側に配置する事
    /// 注意: Raycast は、その原点がコライダーの内側にある場合そのコライダーを検知しません。
    /// </summary>
    [Header("地面接地確認用Ray")]
    [SerializeField] private RayLength _chackGroundRay;
    [SerializeField] private float _checkGroundRadius = 0.5f;
    private LayerMask _groundLayer = StaticCommonParams.GroundLayer;


    //イベント
    public UnityEvent<AttackObjectBase> OnDamage;
    public UnityEvent<AttackObjectBase> OnDeath;



    private Rigidbody _rb;
    public Rigidbody Rb
    {
        get
        {
            if (!_rb) _rb = GetComponent<Rigidbody>();
            return _rb;
        }
        private set { _rb = value; }
    }

    public Transform Transform => transform;

    private GravityComp _gravityComp;
    public GravityComp Gravity
    {
        get
        {
            if (!_gravityComp) _gravityComp = GetComponent<GravityComp>();
            return _gravityComp;
        }
        private set { _gravityComp = value; }
    }

    private HpComp _hpComp;
    public HpComp Hp
    {
        get
        {
            if (!_hpComp) _hpComp = GetComponent<HpComp>();
            return _hpComp;
        }
        private set { _hpComp = value; }
    }

    private MoveDirection _moveDirection;
    public MoveDirection MoveDirectionComp
    {
        get
        {
            if (!_moveDirection) _moveDirection = GetComponent<MoveDirection>();
            return _moveDirection;
        }
        private set
        { _moveDirection = value; }
    }

    public bool IsFreeze { get; private set; }

    public bool IsStandingGround => _isStandingGround;

    public Vector3 MoveDirection => MoveDirectionComp.Direction;
    private Vector3 _beforPosition;

    private RigidbodyConstraints _constrainsCache = RigidbodyConstraints.None;

    //重力を使用するかキャッシュ
    private bool _isUseGravity = true;



    private void Awake()
    {
        if (Gravity.enabled) _isUseGravity = true;
        //死んだときの処理
        Hp.OnChange.AddListener(() =>
        {
            if (Hp.Hp <= 0)
            {
                if (Gravity.IsActive) Gravity.Disable();
                if (!IsFreeze) Freeze();

            }
            else if (Hp.Hp > 0)
            {
                if (!Gravity.IsActive && _isUseGravity) Gravity.Enable();
                if (IsFreeze) UnFreeze();
            }
        }
        );
    }

    private void Update()
    {
        if (CheckHitGround(_chackGroundRay.Length))
        {
            //地面抜け防止
            if (!_isStandingGround)
            {
                Rb.velocity = new Vector3(Rb.velocity.x, 0, Rb.velocity.z);
            }

            _isStandingGround = true;
        }
        else _isStandingGround = false;


    }


    public void Damage(int amount, AttackObjectBase attackObject)
    {
        if (_hpComp.IsInvincible)
        {
            return;
        }
        Hp.Damage(amount);
        if (Hp.Hp <= 0)
        {
            OnDeath?.Invoke(attackObject);
        }
        else OnDamage?.Invoke(attackObject);
    }

    public void Freeze()
    {
        IsFreeze = true;
        _constrainsCache = Rb.constraints;
        Rb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void UnFreeze()
    {
        IsFreeze = false;
        Rb.constraints = _constrainsCache;
    }

    private Coroutine _invincibleCoroutine;
    /// <summary>
    /// キャラの無敵のOn OFF
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public Coroutine Invincible(bool condition, float waitTime = 0)
    {
        if (_invincibleCoroutine != null)
        {
            StopCoroutine(_invincibleCoroutine);
        }
        return _invincibleCoroutine = StartCoroutine(InvincibleCoroutine(condition, waitTime));
    }

    private IEnumerator InvincibleCoroutine(bool condition, float waitTime)
    {
        yield return StaticCommonParams.Yielders.Get(waitTime);
        Hp.IsInvincible = condition;
    }

    public bool CheckHitGround(float layLength, float coyoteRadius = 0, bool isDebugMode = false)
    {
        return CheckGround.SphereCheck(_chackGroundRay.StartPosition, layLength, _checkGroundRadius, _groundLayer, Rb, coyoteRadius, isDebugMode);
    }

}
