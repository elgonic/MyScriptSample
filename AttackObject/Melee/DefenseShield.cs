using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 防御用シールド
/// </summary>
public class DefenseShield : MeleeAttackObjectBase
{
    [SerializeField] private GameObject _shield;
    [SerializeField] private CharacterParam _attacker;
    [SerializeField] private bool _isHitBreak = false;
    [SerializeField] private bool _isParryBreak = false;
    private Collider __collider;
    private Collider _collider
    {
        get
        {
            if (!__collider)
            {
                __collider = GetComponent<Collider>();

                if (__collider == null)
                {
                    Debugger.LogError($"{gameObject.name} に 当たり判定が存在しません!");

                }
            }
            return __collider;
        }
    }

    //シールドイベント
    public UnityEvent OnEnable;
    public UnityEvent OnDisable;
    public UnityEvent OnBreak;

    private void Reset()
    {
        ParryMode = AttackObjectBase.ParryKinds.DisAble;

        Transform target = transform;

        _attacker = target.GetComponent<CharacterParam>();
        while (target != transform.root && _attacker == null)
        {
            target = GetParent(target);
            _attacker = target.GetComponent<CharacterParam>();
        }

        if (_attacker == null)
        {
            Debugger.LogError($"{gameObject.name} : 親かこのオブジェクトに{nameof(CharacterParam)}が存在しません");
        }

        Transform GetParent(Transform target)
        {
            return target.parent;
        }


    }

    private void Update()
    {
        if (_attacker.Hp.IsInvincible)
        {
            Enable();
        }
        else if (gameObject.activeSelf)
        {
            Disable();
        }
    }


    public override void Hit(GameObject hitObject)
    {
        OnHit?.Invoke();
        if (_isHitBreak) Break();
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        OnParryed?.Invoke(attackerData);
        if (_isParryBreak) Break();
    }


    public override Vector3 GetHitedObjectKnockBackDirection(GameObject hitObject = null)
    {

        Vector3 nockBackDirection = _attacker.MoveDirection;

        // Vector3.zero 以外を返したい
        if (_attacker.MoveDirection != Vector3.zero) return nockBackDirection;
        CharacterParam characterParam = hitObject.GetComponent<CharacterParam>();

        if (characterParam && characterParam.MoveDirection != Vector3.zero) return -characterParam.MoveDirection;
        Rigidbody rb = hitObject.GetComponent<Rigidbody>();

        if (rb && rb.velocity != Vector3.zero) return -rb.velocity;

        return hitObject.transform.position - _attacker.transform.position;

    }

    private void Enable()
    {
        OnEnable?.Invoke();
        if (!_collider.enabled) _collider.enabled = true;
        if (!_shield.activeSelf) _shield.SetActive(true);

    }

    private void Disable()
    {
        OnDisable?.Invoke();
        if (_collider.enabled) _collider.enabled = false;
        if (_shield.activeSelf) _shield.SetActive(false);
    }

    private void Break()
    {
        OnBreak?.Invoke();
        if (_collider.enabled) _collider.enabled = false;
        if (_shield.activeSelf) _shield.SetActive(false);
    }

}
