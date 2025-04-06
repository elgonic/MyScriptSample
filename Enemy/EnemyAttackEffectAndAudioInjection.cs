using UnityEngine;

/// <summary>
/// 敵の攻撃挙動にエフェクトと音を注入
/// </summary>
public class EnemyAttackEffectAndAudioInjection : MonoBehaviour
{
    [SerializeField] private AudioSource _attackSE;
    [SerializeField] private AudioSource _anticipationSE;
    [SerializeField] private EnemyBehaviorBase _enemyBehaviorBase;
    [SerializeField] private Transform __attackAnticipationEffectTr;
    private Transform _attackAnticipationEffectTr
    {
        get
        {
            if (__attackAnticipationEffectTr) return __attackAnticipationEffectTr;
            Debugger.LogWarning($"{nameof(Enemy)} : {gameObject.name} に {nameof(__attackAnticipationEffectTr)} が設定されていません . transformを使用します ");
            return transform;
        }
    }

    private void Reset()
    {
        _enemyBehaviorBase = GetComponent<EnemyBehaviorBase>();
    }

    private void Start()
    {
        _enemyBehaviorBase = GetComponent<EnemyBehaviorBase>();
        if (!_enemyBehaviorBase) Debugger.LogError($"{nameof(EnemyAttackEffectAndAudioInjection)} : {nameof(EnemyBehaviorBase)} が必要です!");

        _enemyBehaviorBase.OnAttackAnticipation.AddListener(OnAttackAnticipation);
        _enemyBehaviorBase.OnAttack.AddListener(OnAttack);

    }
    private void OnAttackAnticipation()
    {
        EffectSpawner.Instance.CommonPooledEffectPlay(EffectSpawner.EffectType.EnemyAttackAnticipation, _attackAnticipationEffectTr.position, _attackAnticipationEffectTr.rotation, _attackAnticipationEffectTr);
        AudioManager.Instance.UniqePooledSEPlay(_anticipationSE, _attackAnticipationEffectTr.position);
    }

    private void OnAttack()
    {
        AudioManager.Instance.UniqePooledSEPlay(_attackSE, _attackAnticipationEffectTr.position);
    }


}
