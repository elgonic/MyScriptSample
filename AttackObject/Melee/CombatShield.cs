using UnityEngine;


/// <summary>
/// 近接攻撃用の盾
/// </summary>
public class CombatShield : MeleeAttackObjectBase
{

    public override void Hit(GameObject hitObject)
    {
        OnHit?.Invoke();
    }

    public override void Parryed(AttackObjectBase attackerData)
    {
        OnParryed?.Invoke(attackerData);
    }


    public override Vector3 GetHitedObjectKnockBackDirection(GameObject hitObject = null)
    {

        CharacterParam characterParam = hitObject.GetComponent<CharacterParam>();
        if (characterParam && characterParam.MoveDirection != Vector3.zero) return -characterParam.MoveDirection;
        if (MoveDirection != Vector3.zero) return MoveDirection;
        return transform.forward;

    }

}
