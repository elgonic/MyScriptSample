using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using UnityEngine;

/// <summary>
/// 近距離攻撃と遠距離攻撃の合わせた挙動 + レーザー縄跳び
/// </summary>
public class NearAttackAndBulletLaserCircle : EnemyBehaviorBase
{
    /// 近接攻撃
    [SerializeField] private NearAttackBase _nearAttack;
    [SerializeField] private float _nearAttackDistanceThreshold;
    [SerializeField] private float _startDelayTime = 5f;


    //遠距離攻撃
    [SerializeField] private AttackBase _farAttack;


    //レーザー縄跳び攻撃
    [SerializeField] private CircleLaserAttack _circleLaserAttack;


    [SerializeField] private int _nearAttacksToTransitionToFarAttacks = 2;

    //中央の位置の差の許容
    [SerializeField] private float _centerPositionDiffBuff = 10f;


    /// <summary>
    /// 攻撃前の予備動作時間
    /// </summary>
    [SerializeField] private float _anticipationTime = 0f;
    [SerializeField] private float _attackAfterGapTime = 1f;
    [SerializeField] private float _downTime = 2f;
    [SerializeField] private KnockBack.Params _params;


    private Vector3 _centerPosition;


    private CharacterParam _attacker;
    private CharacterParam _target;

    private ICoroutineMove _knockBack;


    //一時変数
    private float _targetDistance;
    private Coroutine _coroutine;
    private bool _doneFirstAttack = false;

    //連続近接攻撃回数
    private int _continuousNearAttackCount = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nearAttackDistanceThreshold);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_centerPosition, _centerPositionDiffBuff);


    }

    private void Awake()
    {
        if (!isActiveAndEnabled) return;

        _attacker = GetComponent<CharacterParam>();
        _target = MainGameSystem.Instance.Player;



        MainGameSystem.Instance.OnMainStageEntry.AddListener(WakeUp);
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(() =>
        {
            StopBehavior();

            if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        }
        );

        _knockBack = new KnockBack(_params, _attacker);

        _nearAttack.OnParryed.AddListener(Parryed);
        OnDown.AddListener(() => { _nearAttack.gameObject.SetActive(false); });
        OnRecoveryDown.AddListener(() => { _nearAttack.gameObject.SetActive(true); });



        _attacker.OnDamage.AddListener(Dameged);
        _attacker.OnDeath.AddListener((hitObj) =>
        {
            StopBehavior();
            if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        });

        _centerPosition = _attacker.Transform.position;


    }

    //チャックボックス出すために
    private void Start()
    {

    }

    private void WakeUp()
    {
        if (_attacker.Hp.Hp > 0) _coroutine = StartCoroutine(Behavior());
        _attacker.Invincible(false);
    }


    private IEnumerator Behavior()
    {

        LookTarget(true);
        yield return StaticCommonParams.Yielders.Get(_startDelayTime);

        while (true)
        {
            _targetDistance = (_target.Transform.position - _attacker.Transform.position).magnitude;

            //攻撃条件確認 , 初回は遠距離固定
            if ((_nearAttackDistanceThreshold > _targetDistance && _continuousNearAttackCount < _nearAttacksToTransitionToFarAttacks) && _doneFirstAttack == true)
            {

                _continuousNearAttackCount++;
                //予備動作時間挟む
                OnAttackAnticipation?.Invoke();
                yield return StaticCommonParams.Yielders.Get(_anticipationTime);

                OnAttack?.Invoke();
                _doneFirstAttack = true;
                yield return _nearAttack.Attack(_attacker, _target);


                //スキ
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
            }
            else
            {
                _continuousNearAttackCount = 0;

                //一旦中央に戻る
                if ((_attacker.Transform.position - _centerPosition).sqrMagnitude > _centerPositionDiffBuff * _centerPositionDiffBuff)
                {
                    yield return LMotion.Create(_attacker.transform.position, _centerPosition, 1f).WithScheduler(MotionScheduler.FixedUpdate).BindToPosition(_attacker.Transform).ToYieldInteraction();
                }

                _doneFirstAttack = true;
                OnAttackAnticipation?.Invoke();
                yield return StaticCommonParams.Yielders.Get(_anticipationTime);
                OnAttack?.Invoke();

                _circleLaserAttack.Attack(_attacker, _target);
                yield return _farAttack.Attack(_attacker, _target);
                _circleLaserAttack.Finish();

                //スキ
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
            }



        }
    }


    private void StopBehavior()
    {
        if (_coroutine == null) return;

        _circleLaserAttack.Finish();
        StopCoroutine(_coroutine);
        _nearAttack.Finish();

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
        Debugger.Log("Parryed");
        ParryedKnockBack(attacker);
    }


    private void Dameged(AttackObjectBase attacker)
    {

        Debugger.Log("Dameged");
        DamegedKnockBack(attacker);
    }




    private Coroutine _knockBackCoroutine;
    private void ParryedKnockBack(AttackObjectBase hitObject)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(ParryedKnockBackCoroutine(hitObject));

    }

    private void DamegedKnockBack(AttackObjectBase hitObject)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(DamegedKnockBackCoroutine(hitObject));
    }

    /// <summary>
    /// ダウンする
    /// </summary>
    /// <param name="attackData"></param>
    /// <returns></returns>
    private IEnumerator ParryedKnockBackCoroutine(AttackObjectBase attackData)
    {

        LookTarget(false);

        OnDown?.Invoke();

        //攻撃挙動停止
        StopBehavior();

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _knockBack.Move(new Vector3(direction.x, 0, direction.z));



        yield return StaticCommonParams.Yielders.Get(_downTime);


        OnRecoveryDown?.Invoke();


        //攻撃再開 , コルーチン初めから
        _coroutine = StartCoroutine(Behavior());
    }

    /// <summary>
    /// ダウンしない
    /// </summary>
    /// <param name="attackData"></param>
    /// <returns></returns>
    private IEnumerator DamegedKnockBackCoroutine(AttackObjectBase attackData)
    {

        LookTarget(false);


        //攻撃挙動停止
        StopBehavior();

        Vector3 direction = attackData.GetHitedObjectKnockBackDirection(_attacker.gameObject);

        if (direction == Vector3.zero) direction = -_attacker.MoveDirection;

        yield return _knockBack.Move(new Vector3(direction.x, 0, direction.z));

        //ほんの少し
        yield return StaticCommonParams.Yielders.Get(0.3f);

        OnRecoveryDown?.Invoke();

        //攻撃再開 , コルーチン初めから
        _coroutine = StartCoroutine(Behavior());
    }



}
