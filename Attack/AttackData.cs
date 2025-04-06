using UnityEngine;

/// <summary>
/// 攻撃時に受け渡されるデータ
/// </summary>
public class AttackData
{
    public Vector3 AttackPosition { get; private set; }
    public CharacterParam Attacker { get; private set; }

    public bool IsKnockBackDirectionToAttacker { get; private set; }



    public AttackData(Vector3 attackPosition, CharacterParam attacker, bool isKnockBackDirectionToAttacker = false)
    {
        AttackPosition = attackPosition;
        Attacker = attacker;
        IsKnockBackDirectionToAttacker = isKnockBackDirectionToAttacker;
    }
}
