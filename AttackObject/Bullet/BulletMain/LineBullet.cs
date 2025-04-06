using UnityEngine;

/// <summary>
/// 直進する弾
/// </summary>
public class LineBullet : BulletBase
{

    [SerializeField] private LineBulletParams _defParams;
    private LineBulletParams _params;
    private IBulletBehavior _bulletBehavior;


    [SerializeField] private bool _fireTest = false;


    private BossFieldInfo _bossFieldInfo => MainGameSystem.Instance.BossFieldInfo;
    private Vector3 _resetLocalPosition;
    private Quaternion _resetLocalRotation;

    private bool _isFire = false;
    private float _fireTime = 0;

    private Transform _attacker;

    private void OnValidate()
    {
        if (_fireTest)
        {
            _fireTest = false;
            FireTest(transform.forward);
        }
    }
    private void Start()
    {
        _resetLocalPosition = transform.position;
        _resetLocalRotation = transform.rotation;
    }
    private void FixedUpdate()
    {
        if (!_isFire) return;

        if (LifeTimeCheck(_params.LifeTime, _fireTime, _params.IsEnableLifeTime))
        {
            OnLifeTime?.Invoke();
            ResetTransform();
        }

        if (_bossFieldInfo && CheckPlaneOutSide(_bossFieldInfo.FieldPlaneList, transform.position, _params.IsEnablePlaneHitCheck))
        {
            OnLifeTime?.Invoke();
            ResetTransform();
        }

        _bulletBehavior.OnUpdate();

    }

    public override void Fire(Vector3 fireDirection, AttackData attackData, Transform target = null)
    {
        if (!_params) _params = _defParams;


        //攻撃者の移動の慣性を使用する
        if (_params.IsConsiderationAttackerVelocity)
        {
            Vector3 fireDirectionAddAttackerVel = _params.Velocity * fireDirection.normalized + attackData.Attacker.Rb.velocity;
            float fireVel = fireDirectionAddAttackerVel.magnitude;
            _bulletBehavior = new LineBulleltBehavior(transform, fireDirectionAddAttackerVel, fireVel);

        }
        else _bulletBehavior = new LineBulleltBehavior(transform, fireDirection, _params.Velocity);


        gameObject.SetActive(true);
        _isFire = true;
        _fireTime = Time.time;
        AttackData = attackData;

    }



    public override void Hit(GameObject hitObject)
    {
        OnHit?.Invoke();
        gameObject.SetActive(false);
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        Debugger.Log($"{GetType().Name} の {System.Reflection.MethodBase.GetCurrentMethod().Name}は空です");
    }

    public override void ResetTransform()
    {
        if (_params.IsDestroyWhenDisable)
        {
            Destroy(gameObject);
        }
        else
        {
            _isFire = false;
            transform.position = _resetLocalPosition;
            transform.rotation = _resetLocalRotation;
            gameObject.SetActive(false);
        }
    }
}
