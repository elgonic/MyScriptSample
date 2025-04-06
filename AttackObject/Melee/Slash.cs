using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 近接攻撃用
/// </summary>
[RequireComponent(typeof(Collider))]
public class Slash : MeleeAttackObjectBase
{
    public UnityEvent OnAttack;

    [Header("攻撃判定有効化するまでの遅延")]
    public float HitColliderEnableDelay = 0f;

    [Header("攻撃判定有効化継続時間")]
    [SerializeField] private float _hitColliderEnableTime = 0.1f;


    /// <summary>
    /// 遅延と有効か時間を合わせたトータルの攻撃時間
    /// </summary>
    public float AttackTime => HitColliderEnableDelay + _hitColliderEnableTime;

    private Collider @hitCollider;
    private Collider _hitCollider
    {
        get
        {
            if (@hitCollider == null)
            {
                @hitCollider = GetComponent<Collider>();
            }
            return @hitCollider;
        }
    }

    public void OnEnable()
    {
        _hitCollider.enabled = false;
    }

    private Coroutine _attackCoroutine;
    public Coroutine Attack()
    {
        return _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        OnAttack?.Invoke();
        yield return StaticCommonParams.Yielders.Get(HitColliderEnableDelay);
        _hitCollider.enabled = true;
        yield return StaticCommonParams.Yielders.Get(_hitColliderEnableTime);
        _hitCollider.enabled = false;


    }

    public void StopAttack()
    {
        if (_attackCoroutine == null) return;
        StopCoroutine(_attackCoroutine);
        _hitCollider.enabled = false;
    }
    public override void Hit(GameObject hitObject)
    {
        StopAttack();
        OnHit?.Invoke();
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        StopAttack();
        OnParryed?.Invoke(attackerData);
    }


    public override Vector3 GetHitedObjectKnockBackDirection(GameObject hitObject = null)
    {

        return transform.forward;

    }

}
