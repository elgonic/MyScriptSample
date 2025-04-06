using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
///範囲にない入ったらスラッシュ攻撃
/// </summary>
public class SimpleSlashEnemy : EnemyBehaviorBase
{
    [SerializeField] private NearAttackBase _attack;
    [SerializeField] private DefenseShield _shield;
    [SerializeField] private float _attackDistanceThreshold;
    [SerializeField] private bool _disableDistanceThreshold = false;
    [SerializeField] private float _startDelayTime = 5f;

    [SerializeField] private float _nearDistance;


    /// <summary>
    /// 攻撃前の予備動作時間
    /// </summary>
    [SerializeField] private float _anticipationTime = 0f;
    [SerializeField] private float _attackAfterGapTime = 1f;

    [Header("通常移動")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Ease _moveEase;

    [Header("パリィ時")]
    [SerializeField] private KnockBack.Params _parryKnockBackParams;
    [Header("ダメージ時")]
    [SerializeField] private KnockBack.Params _damageKnockBackParams;

    private CharacterParam _attacker;
    private CharacterParam _target;


    private IMove _move;
    private ICoroutineMove _attackMove;
    private ICoroutineMove _damageKnockBack;
    private ICoroutineMove _parryKnockBack;

    //イベント
    public UnityEvent OnSlashAttackInertia;
    public UnityEvent OnSlashAttack;


    //Getter
    public float SlashAnticipationTime => _anticipationTime;

    //一時変数
    private float _targetDistance;
    private Coroutine _coroutine;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _nearDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackDistanceThreshold);
    }

    private void Awake()
    {

        if (!isActiveAndEnabled) return;
        _attacker = GetComponent<CharacterParam>();
        _target = MainGameSystem.Instance.Player;



        MainGameSystem.Instance.OnMainStageEntry.AddListener(WakeUp);
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(StopBehavior);

        _damageKnockBack = new KnockBack(_damageKnockBackParams, _attacker);
        _parryKnockBack = new KnockBack(_parryKnockBackParams, _attacker);

        _attack.OnParryed.AddListener(Parryed);
        OnDown.AddListener(() => { _attack.gameObject.SetActive(false); });
        OnRecoveryDown.AddListener(() => { _attack.gameObject.SetActive(true); });

        _shield.OnParryed.AddListener(Parryed);


        _attacker.OnDamage.AddListener(Dameged);
        _attacker.OnDeath.AddListener((hitObj) =>
        {
            StopBehavior();
        });
    }
    private void Start()
    {

    }
    private void WakeUp()
    {
        if (_attacker.Hp.Hp > 0) _coroutine = StartCoroutine(Behavior());
    }

    private IEnumerator Behavior()
    {
        Debugger.Log("Start");
        _attacker.Invincible(true);
        LookTarget(true);
        yield return StaticCommonParams.Yielders.Get(_startDelayTime);
        while (true)
        {
            Vector3 targetDirection = _target.Transform.position - _attacker.Transform.position;
            targetDirection = new Vector3(targetDirection.x, 0, targetDirection.z);
            _targetDistance = targetDirection.magnitude;

            //攻撃条件確認
            if (_disableDistanceThreshold || _attackDistanceThreshold > _targetDistance)
            {
                Debugger.Log("Attack!");

                float moveDistance = (_targetDistance - _nearDistance);
                float moveTime = _targetDistance / _moveSpeed;

                //予備動作時間挟む
                OnAttackAnticipation?.Invoke();

                yield return LMotion.Create(transform.position, transform.position + (targetDirection.normalized * (_targetDistance - _nearDistance)), moveTime).WithScheduler(MotionScheduler.FixedUpdate).BindToPosition(transform).ToYieldInteraction();

                LookTarget(false);

                OnAttack?.Invoke();

                yield return _attack.Attack(_attacker, _target);


                Debugger.Log("AttackFinish!");
                //スキ
                _attacker.Invincible(false);
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
                _attacker.Invincible(true);
            }
            yield return null;
        }
    }
    private Coroutine _lookTarget;
    private void LookTarget(bool enabled)
    {
        if (enabled && _lookTarget == null)
        {
            _lookTarget = StartCoroutine(LookTargetCoroutine());
        }
        else if (!enabled && _lookTarget != null)
        {
            StopCoroutine(_lookTarget);
            _lookTarget = null;
        }
    }

    private IEnumerator LookTargetCoroutine()
    {
        while (true)
        {
            Quaternion toRotation = Quaternion.LookRotation(_target.Transform.position - _attacker.Transform.position, Vector3.up);
            _attacker.transform.rotation = Quaternion.Slerp(_attacker.Transform.rotation, toRotation, 5f * Time.deltaTime);
            yield return StaticCommonParams.Yielders.FixedUpdate;

        }
    }

    private Coroutine _knockBackCoroutine;
    public void Parryed(AttackObjectBase attacker)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(ParryKnockBackCoroutine(attacker));

    }


    private void Dameged(AttackObjectBase attacker)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(DamageKnockBackCoroutine(attacker));

    }



    //コルーチンは親を止めても入れ子のコルーチンは停止しないので,別で停止する必要がある
    private IEnumerator DamageKnockBackCoroutine(AttackObjectBase attackData)
    {


        _damageKnockBack.Stop();


        LookTarget(false);

        OnDown?.Invoke();

        //攻撃挙動停止
        StopCoroutine(_coroutine);

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _damageKnockBack.Move(new Vector3(direction.x, 0, direction.z));



        OnRecoveryDown?.Invoke();


        //攻撃再開 , コルーチン初めから
        Debugger.Log("Restart");
        _coroutine = StartCoroutine(Behavior());
    }
    private IEnumerator ParryKnockBackCoroutine(AttackObjectBase attackData)
    {

        _damageKnockBack.Stop();


        LookTarget(false);

        OnDown?.Invoke();

        //攻撃挙動停止
        StopCoroutine(_coroutine);

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _parryKnockBack.Move(new Vector3(direction.x, 0, direction.z));

        OnRecoveryDown?.Invoke();


        //攻撃再開 , コルーチン初めから
        Debugger.Log("Restart");
        _coroutine = StartCoroutine(Behavior());
    }



    private void StopBehavior()
    {

        if (_coroutine != null)
        {
            Debugger.Log("Stop Coroutine");
            StopCoroutine(_coroutine);
        }
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _attack.Finish();

    }

}
