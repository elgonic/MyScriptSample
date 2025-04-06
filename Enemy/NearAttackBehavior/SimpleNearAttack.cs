using System.Collections;
using UnityEngine;

/// <summary>
/// 一定距離内に入ったら攻撃する
/// </summary>
public class SimpleNearAttack : EnemyBehaviorBase
{
    [SerializeField] private NearAttackBase _attack;
    [SerializeField] private float _attackDistanceThreshold;
    [SerializeField] private bool _disableDistanceThreshold = false;
    [SerializeField] private float _startDelayTime = 5f;
    /// <summary>
    /// 攻撃前の予備動作時間
    /// </summary>
    [SerializeField] private float _anticipationTime = 0f;
    [SerializeField] private float _attackAfterGapTime = 1f;
    [SerializeField] private float _downTime = 2f;
    [Header("ノックバック設定")]
    [SerializeField] private KnockBack.Params _params;

    private CharacterParam _attacker;
    private CharacterParam _target;

    private ICoroutineMove _knockBack;

    //イベント


    //一時変数
    private float _targetDistance;
    private Coroutine _coroutine;
    private void Awake()
    {

        if (!isActiveAndEnabled) return;
        _attacker = GetComponent<CharacterParam>();
        _target = MainGameSystem.Instance.Player;



        MainGameSystem.Instance.OnMainStageEntry.AddListener(WakeUp);
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(StopBehavior);

        _knockBack = new KnockBack(_params, _attacker);

        _attack.OnParryed.AddListener(Parryed);
        OnDown.AddListener(() => { _attack.gameObject.SetActive(false); });
        OnRecoveryDown.AddListener(() => { _attack.gameObject.SetActive(true); });


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
        _attacker.Invincible(false);
        if (_attacker.Hp.Hp > 0) _coroutine = StartCoroutine(Behavior());
    }

    private IEnumerator Behavior()
    {
        Debugger.Log("Start");
        LookTarget(true);
        yield return StaticCommonParams.Yielders.Get(_startDelayTime);
        while (true)
        {
            _targetDistance = (_target.Transform.position - _attacker.Transform.position).magnitude;

            //攻撃条件確認
            if (_disableDistanceThreshold || _attackDistanceThreshold > _targetDistance)
            {
                Debugger.Log("Attack!");
                //予備動作時間挟む
                OnAttackAnticipation?.Invoke();
                yield return StaticCommonParams.Yielders.Get(_anticipationTime);




                OnAttack?.Invoke();
                _attacker.Invincible(true);

                yield return _attack.Attack(_attacker, _target);

                _attacker.Invincible(false);

                Debugger.Log("AttackFinish!");
                //スキ
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
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

    public void Parryed(AttackObjectBase attacker)
    {
        KnockBack(attacker);
    }


    private void Dameged(AttackObjectBase attacker)
    {
        KnockBack(attacker);
    }


    private Coroutine _knockBackCoroutine;
    private void KnockBack(AttackObjectBase attackData)
    {
        Debugger.Log("KnockBack");
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(KnockBackCoroutine(attackData));

    }


    //コルーチンは親を止めても入れ子のコルーチンは停止しないので,別で停止する必要がある
    private IEnumerator KnockBackCoroutine(AttackObjectBase attackData)
    {

        _attacker.Invincible(false, 0.5f);

        _knockBack.Stop();


        LookTarget(false);

        OnDown?.Invoke();

        //攻撃挙動停止
        StopCoroutine(_coroutine);

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _knockBack.Move(new Vector3(direction.x, 0, direction.z));

        yield return StaticCommonParams.Yielders.Get(_downTime);


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
