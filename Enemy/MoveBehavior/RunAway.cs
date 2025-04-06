using System.Collections;
using UnityEngine;



/// <summary>
/// プレイヤーから逃げる敵
/// </summary>
public class RunAway : EnemyBehaviorBase
{
    [SerializeField] private float _moveSpeed = 40f;
    [SerializeField] private float _distanceThreshold = 3.0f;

    [SerializeField] private RayLength _checkObstacleRay;
    [SerializeField] private KnockBack.Params _knockBackParams;


    [Header("攻撃")]
    [SerializeField] private float _moveAttackInertiaTime = 0.5f;
    [SerializeField] private float _moveAttackIntervalTime = 3f;
    [SerializeField] private AttackBase _moveAttack;


    private CharacterParam _characterParam;
    private float _sqrDistanceThreshold;
    private CharacterParam _target;
    private CharacterParam _attacker;

    private Coroutine _mainCoroutine;
    private Coroutine _knockBackCoroutine;

    private RigidVelocityImpulseMove.Params _moveParams = new();


    private IMove _move;
    private ICoroutineMove _knockBack;


    private float _rayLength;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
    }


    // Start is called before the first frame update
    private void Start()
    {
        _attacker = GetComponent<CharacterParam>();
        _target = MainGameSystem.Instance.Player;

        _characterParam = GetComponent<CharacterParam>();
        MainGameSystem.Instance.OnMainStageEntry.AddListener(WakeUp);
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(StopBehavior);

        _attacker.OnDamage.AddListener(Dameged);
        _attacker.OnDeath.AddListener(attackObj => StopBehavior());

        _moveParams.MoveSpeed = _moveSpeed;
        _moveParams.EnableAntiProcess = false;
        _move = new RigidVelocityImpulseMove(_moveParams, _characterParam.Rb);


        _knockBack = new KnockBack(_knockBackParams, _attacker);

        _sqrDistanceThreshold = _distanceThreshold * _distanceThreshold;
    }
    private void WakeUp()
    {
        if (_characterParam.Hp.Hp > 0) _mainCoroutine = StartCoroutine(Behavior());
    }

    private void StopBehavior()
    {
        Debugger.Log($"{GetType().Name} : {System.Reflection.MethodBase.GetCurrentMethod().Name}");
        if (_mainCoroutine != null)
        {
            StopCoroutine(_mainCoroutine);
            _move.Stop();

            if (_moveAttackCoroutine != null) StopCoroutine(_moveAttackCoroutine);
            if (_moveAttack) _moveAttack.Finish();
        }
    }


    //逃げる処理
    private IEnumerator Behavior()
    {
        //逃げる方向 , ループ中は回転していくので 更新しない
        Vector3 directionV3Normalize;
        Vector2 directionV2Normalize;
        Vector3 directionV3;
        Vector3 tmp;

        GetDirection();

        //逃げる閾値
        while (directionV3.sqrMagnitude >= _sqrDistanceThreshold)
        {
            GetDirection();
            yield return StaticCommonParams.Yielders.FixedUpdate;
        }

        //逃げる処理 , 壁に当たったら右回り回転
        Debugger.Log($"Start {directionV3Normalize}");
        MoveAttack();
        while (true)
        {
            bool canMove = MoveCommonProcess.CheckObstacle(directionV3Normalize, _checkObstacleRay, _moveParams.MoveSpeed, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });
            while (canMove)
            {
                _move.Stop();
                //移動可能になるまで回転する
                directionV3Normalize = Quaternion.AngleAxis(100, transform.up) * directionV3Normalize;
                canMove = MoveCommonProcess.CheckObstacle(directionV3Normalize, _checkObstacleRay, _moveParams.MoveSpeed, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });
                yield return null;
            }
            //Debugger.Log($"in check {directionV3Normalize}");

            _moveAttackDirection = directionV3Normalize;
            directionV2Normalize = new Vector2(directionV3Normalize.x, directionV3Normalize.z);
            _move.Move(directionV2Normalize);

            yield return StaticCommonParams.Yielders.FixedUpdate;
        }

        void GetDirection()
        {
            tmp = transform.position - _target.transform.position;
            directionV3 = new Vector3(tmp.x, 0, tmp.z);
            directionV3Normalize = directionV3.normalized;
        }
    }


    private Coroutine _moveAttackCoroutine;
    private Vector3 _moveAttackDirection;
    private Coroutine MoveAttack()
    {
        if (!_moveAttack) return null;

        if (_moveAttackCoroutine != null)
        {
            StopCoroutine(_moveAttackCoroutine);
            _moveAttackCoroutine = null;
        }
        return _moveAttackCoroutine = StartCoroutine(MoveAttackCoroutine());
    }
    private IEnumerator MoveAttackCoroutine()
    {
        while (true)
        {
            OnAttackAnticipation?.Invoke();
            yield return StaticCommonParams.Yielders.Get(_moveAttackInertiaTime);
            OnAttack?.Invoke();
            yield return _moveAttack.Attack(_attacker, _target);
            yield return StaticCommonParams.Yielders.Get(_moveAttackIntervalTime);
        }
    }


    private void Dameged(AttackObjectBase attacker)
    {
        DamegedKnockBack(attacker);
    }

    private void DamegedKnockBack(AttackObjectBase hitObject)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(DamegedKnockBackCoroutine(hitObject));
    }

    private IEnumerator DamegedKnockBackCoroutine(AttackObjectBase attackData)
    {

        //攻撃挙動停止
        StopBehavior();

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _knockBack.Move(new Vector3(direction.x, 0, direction.z));

        //ほんの少し
        yield return StaticCommonParams.Yielders.Get(0.3f);


        //攻撃再開 , コルーチン初めから
        _mainCoroutine = StartCoroutine(Behavior());
    }

}
