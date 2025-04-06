using UnityEngine;


/// <summary>
/// ターゲットを追従する
/// ターゲット未設定なら直進する
/// </summary>
public class Missile : BulletBase
{


    [SerializeField] private MissileParams _defParams;
    [Header("Parry(Bounce)された後のパラメータ(オプション)")]
    [SerializeField] private MissileParams _defParryedParams;



    private BossFieldInfo _bossFieldInfo => MainGameSystem.Instance.BossFieldInfo;

    private MissileParams @params;


    /// <summary>
    /// 使用するパラメーター保存用
    /// </summary>
    /// <remarks>
    /// 元となるパラメーターは変更したくないのでコピー
    /// </remarks>
    private MissileParams _params
    {
        get
        {
            if (!@params)
            {
                _params = _defParams;
            }
            return @params;
        }
        set { @params = value; }
    }
    private IBulletBehavior _bulletBehavior;
    [SerializeField] private bool _isPlayerHitDisable = false;
    private bool _isFire = false;


    [Header("Debug用")]
    [SerializeField] private bool _fireTest = false;

    private Transform _target;
    private Transform _transform;
    private float _fireTime;


    /// <summary>
    /// パリッ時の当たり判定有効化するため
    /// </summary>
    private Rigidbody _rb;

    /// <summary>
    /// 初回代入だけ有効にしたい
    /// </summary>
    private Vector3? _resetPosition;
    private Quaternion? _resetRotation;

    private void OnValidate()
    {
        if (_fireTest)
        {
            _fireTest = false;
            Transform player = GameObject.FindFirstObjectByType<Player>().transform;
            FireTest(transform.up, player);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        OnUpdate();
    }

    private void Start()
    {
        Debugger.Log("Start!");
        _transform = transform;
        if (!_resetPosition.HasValue)
        {
            _resetPosition = transform.position;
            _resetRotation = transform.rotation;
        }

    }


    private void OnUpdate()
    {

        if (!_isFire) return;
        if (LifeTimeCheck(_params.LifeTime, _fireTime, _params.IsEnableLifeTime))
        {
            ResetTransform();
            OnLifeTime?.Invoke();
        }
        if (_bossFieldInfo && CheckPlaneOutSide(_bossFieldInfo.FieldPlaneList, _transform.position, _params.IsEnablePlaneHitCheck))
        {
            ResetTransform();
            OnLifeTime?.Invoke();
        }

        _bulletBehavior.OnUpdate();


        //ターゲットが未設定なら
        if (!_target || _params.AngleVelocity == 0) return;
        //ミサイルがプレイヤーを通過したらUnkockする(基準は弾を発射した時に射手が居た場所(発射位置と車種が同じ位置に入るとは限らない)と現在のターゲットの位置)
        Plane _targetPlane = new Plane((AttackData.AttackPosition - _target.position), _target.position);
        if (_targetPlane.GetDistanceToPoint(_transform.position) <= 0)
        {
            Debugger.Log($"{typeof(Missile).Name}:{gameObject.name}:Missle ThrouTarget so Unlock!");
            _bulletBehavior = new MissleBehavior(_transform, MoveDirection, _params.Velocity, _params.AngleVelocity);
            _target = null;
        }

    }

    public override void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {

        OnFire?.Invoke();

        if (!_resetPosition.HasValue)
        {
            _resetPosition = transform.position;
            _resetRotation = transform.rotation;
        }

        _isFire = true;
        _transform = transform;
        _fireTime = Time.time;
        AttackData = attackData;
        _target = target;

        _bulletBehavior = new MissleBehavior(_transform, fireDirection, _params.Velocity, _params.AngleVelocity, _target);

        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public override void Hit(GameObject hitObject)
    {
        Debugger.Log("Hit");
        OnHit?.Invoke();

        //Playerに当たっても消えない場合
        if (_isPlayerHitDisable && hitObject.CompareTag(StaticCommonParams.PLAYER_TAG)) return;


        ResetTransform();
    }
    public override void Parryed(AttackObjectBase attackerData)
    {
        OnParryed?.Invoke(attackerData);
        if (_defParryedParams) _params = _defParryedParams;

        Vector3 fireDirection = attackerData.AttackData.Attacker.MoveDirection;
        if (fireDirection == Vector3.zero)
        {
            fireDirection = attackerData.AttackData.Attacker.transform.forward;
        }

        //置物として置いたときは AttackData = nullなので
        if (AttackData != null) Fire(fireDirection, new AttackData(attackerData.AttackData.Attacker.transform.position, attackerData.AttackData.Attacker, false), AttackData.Attacker.transform);
        else Fire(fireDirection, new AttackData(attackerData.AttackData.Attacker.transform.position, attackerData.AttackData.Attacker, false));

        //反射時はRigitbodyを有効化する
        _rb = GetComponent<Rigidbody>();
        if (_rb) return;
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;


    }

    private void OnTriggerEnter(Collider other)
    {
        //RigitBodyない場合は消される側なので無視
        if (_rb == null) return;


        //敵の弾衝突時に消す
        BulletBase bulletBase = other.GetComponent<BulletBase>();
        if (bulletBase)
        {
            //AtttackDataがNullの奴も消す
            if (AttackData.Attacker != bulletBase.AttackData?.Attacker)
            {
                bulletBase.Hit(gameObject);
            }
        }

    }



    public override void ResetTransform()
    {
        if (_rb) Destroy(_rb);

        if (_params.IsDestroyWhenDisable)
        {
            Destroy(gameObject);
        }
        else
        {
            _isFire = false;
            _params = null;
            _target = null;
            _transform.position = _resetPosition.Value;
            _transform.rotation = _resetRotation.Value;
            gameObject.SetActive(false);
        }
    }
}
