using LitMotion;
using LitMotion.Extensions;
using System.Collections;
using UnityEngine;

/// <summary>
/// 近距離攻撃と遠距離攻撃の合わせた挙動
/// </summary>
public class NearAttackAndBulletAttack : EnemyBehaviorBase
{
    /// 近接攻撃
    [SerializeField] private NearAttackBase _nearAttack;
    [SerializeField] private float _nearAttackDistanceThreshold;
    [SerializeField] private float _startDelayTime = 5f;

    //遠距離攻撃
    [SerializeField] private AttackBase _farAttack;

    [SerializeField] private int _nearAttacksToTransitionToFarAttacks = 2;

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
    private Coroutine _mainCoroutine;
    private bool _doneFirstAttack = false;




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nearAttackDistanceThreshold);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_centerPosition, _centerPositionDiffBuff);


    }

    private void Awake()
    {
        if (!this.isActiveAndEnabled) return;


        _attacker = GetComponent<CharacterParam>();
        _target = MainGameSystem.Instance.Player;



        MainGameSystem.Instance.OnMainStageEntry.AddListener(WakeUp);
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(Stop);

        _knockBack = new KnockBack(_params, _attacker);

        _nearAttack.OnParryed.AddListener(Parryed);
        OnDown.AddListener(() => { _nearAttack.gameObject.SetActive(false); });
        OnRecoveryDown.AddListener(() => { _nearAttack.gameObject.SetActive(true); });



        _attacker.OnDamage.AddListener(Dameged);
        _attacker.OnDeath.AddListener((hitObj) => Stop());

        _centerPosition = _attacker.Transform.position;


    }

    //チャックボックス出すために
    private void Start()
    {

    }

    private void WakeUp()
    {
        _attacker.Invincible(false);
        if (_attacker.Hp.Hp > 0) _mainCoroutine = StartCoroutine(Behavior());
    }

    private void Stop()
    {
        if (_mainCoroutine != null) StopCoroutine(_mainCoroutine);
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);

    }

    private IEnumerator Behavior()
    {

        LookTarget(true);
        yield return StaticCommonParams.Yielders.Get(_startDelayTime);
        //連続近接攻撃回数
        int continuousnearAttackCount = 0;

        while (true)
        {
            _targetDistance = (_target.Transform.position - _attacker.Transform.position).magnitude;



            //攻撃条件確認 初めは遠距離から
            if ((_nearAttackDistanceThreshold > _targetDistance && continuousnearAttackCount < _nearAttacksToTransitionToFarAttacks) && _doneFirstAttack == true)
            {

                continuousnearAttackCount++;
                //予備動作時間挟む
                OnAttackAnticipation?.Invoke();
                yield return StaticCommonParams.Yielders.Get(_anticipationTime);

                OnAttack?.Invoke();
                _doneFirstAttack = true;
                _attacker.Invincible(true);
                yield return _nearAttack.Attack(_attacker, _target);
                _attacker.Invincible(false);


                //スキ
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
            }
            else
            {
                continuousnearAttackCount = 0;

                //一旦中央に戻る
                if ((_attacker.Transform.position - _centerPosition).sqrMagnitude > _centerPositionDiffBuff * _centerPositionDiffBuff)
                {
                    yield return LMotion.Create(_attacker.transform.position, _centerPosition, 1f).WithScheduler(MotionScheduler.FixedUpdate).BindToPosition(_attacker.Transform).ToYieldInteraction();
                }

                _doneFirstAttack = true;
                OnAttackAnticipation?.Invoke();
                yield return StaticCommonParams.Yielders.Get(_anticipationTime);
                OnAttack?.Invoke();
                yield return _farAttack.Attack(_attacker, _target);

                //スキ
                yield return StaticCommonParams.Yielders.Get(_attackAfterGapTime);
            }





        }
    }
    private void StopBehavior()
    {

        if (_mainCoroutine == null) return;
        StopCoroutine(_mainCoroutine);

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
        _attacker.Invincible(false, 0.5f);
        ParryedKnockBack(attacker);
    }


    private void Dameged(AttackObjectBase attacker)
    {
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
        _mainCoroutine = StartCoroutine(Behavior());
    }

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
        _mainCoroutine = StartCoroutine(Behavior());
    }


}
