using UnityEngine;


/// <summary>
/// キャラクタ本体に接触したときに攻撃判定を付ける
/// </summary>
/// <remarks>
/// HitColldierにつけるかCharacterスクリプトに当たり判定があるならそちらに付けてもOK!
/// </remarks>
public class CharacterAttack : AttackObjectBase
{

    [SerializeField] private CharacterParam _attacker;

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
    private void Awake()
    {
        AttackData = new AttackData(_attacker.Transform.position, _attacker, false);
    }
    public override void Hit(GameObject hitObject)
    {

    }

    public override void Parryed(AttackObjectBase attackerData)
    {

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
}
