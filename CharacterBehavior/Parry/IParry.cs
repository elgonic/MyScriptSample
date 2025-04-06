using UnityEngine;


/// <summary>
/// パリッ挙動のインターフェース
/// </summary>
public interface IParry
{
    public Coroutine Parry(AttackObjectBase hitAttack);
    public void Stop();
}
