using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 連続斬撃攻撃
/// </summary>
public class ContinuousSlashAttack : NearAttackBase
{
    /// <summary>
    /// 個々の斬撃攻撃データ
    /// </summary>
    [Serializable]
    public class SlashData
    {
        /// <summary>
        /// 攻撃までの猶予時間
        /// </summary>
        public float InertiaTime = 0f;
        /// <summary>
        /// コライダの有効か時間
        /// </summary>
        public float AttackTime
        {
            get
            {
                if (SlashObjects.Count > 0)
                {
                    return SlashObjects.Max(x => x.AttackTime);
                }
                return 0;
            }
        }
        public List<Slash> SlashObjects;


        public void Attack()
        {
            foreach (Slash slash in SlashObjects)
            {
                slash.Attack();
            }
        }

    }

    [SerializeField] private List<SlashData> _slashData;


    // イベント
    public UnityEvent OnInertia;
    public UnityEvent OnAttack;



    private CharacterParam _attacker;
    private CharacterParam _target;
    private Coroutine _attackCoroutine;






    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);

    }

    private void Awake()
    {

    }


    private void Start()
    {
        foreach (SlashData slashData in _slashData)
        {
            foreach (Slash slash in slashData.SlashObjects)
            {
                slash.OnHit.AddListener(Hitted);
                slash.OnParryed.AddListener(Parryed);
            }
        }
    }

    public override Coroutine Attack(CharacterParam attacker, CharacterParam target = null)
    {
        if (!gameObject.activeSelf) return null;
        _attacker = attacker;
        _target = target;
        return _attackCoroutine = StartCoroutine(AttackProcess(attacker, target));
    }

    private IEnumerator AttackProcess(CharacterParam attacker, CharacterParam target = null)
    {

        foreach (SlashData slashData in _slashData)
        {
            LookTarget(true);
            OnInertia?.Invoke();
            yield return StaticCommonParams.Yielders.Get(slashData.InertiaTime);
            LookTarget(false);
            OnAttack?.Invoke();
            slashData.Attack();
            yield return StaticCommonParams.Yielders.Get(slashData.AttackTime);

        }

    }


    private void ResetTransform()
    {
    }



    public override void Hitted()
    {
        OnHitted?.Invoke();

    }
    public override void Parryed(AttackObjectBase parryer)
    {
        OnParryed?.Invoke(parryer);
    }

    public override Coroutine Finish()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }

        LookTarget(false);

        foreach (SlashData slashData in _slashData)
        {
            foreach (Slash slash in slashData.SlashObjects)
            {
                slash.StopAttack();
            }
        }
        ResetTransform();
        return null;
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
        if (_target == null) yield break;
        while (true)
        {
            Quaternion toRotation = Quaternion.LookRotation(_target.Transform.position - _attacker.Transform.position, Vector3.up);
            _attacker.transform.rotation = Quaternion.Slerp(_attacker.Transform.rotation, toRotation, 5f * Time.deltaTime);
            yield return StaticCommonParams.Yielders.FixedUpdate;

        }
    }
}
