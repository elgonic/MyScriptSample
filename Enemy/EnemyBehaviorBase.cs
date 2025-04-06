using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 敵の挙動基底クラス
/// </summary>
public abstract class EnemyBehaviorBase : MonoBehaviour
{
    public UnityEvent OnAttackAnticipation;
    public UnityEvent OnAttack;
    public UnityEvent OnDown;
    public UnityEvent OnRecoveryDown;

}
