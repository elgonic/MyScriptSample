using UnityEngine.Events;


/// <summary>
/// 近接攻撃基底クラス
/// </summary>
public abstract class NearAttackBase : AttackBase
{

    public UnityEvent OnHitted;
    public UnityEvent<AttackObjectBase> OnParryed;

    public abstract void Hitted();
    public abstract void Parryed(AttackObjectBase parryerData);
}
