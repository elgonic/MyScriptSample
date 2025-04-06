using UnityEngine;

/// <summary>
/// 敵にエフェクトと音を注入
/// </summary>
public class EnemyEffectAndAudioInjection : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;

    [Header("Effects")]
    [SerializeField] private PooledEffectSource _damageEffectPrefab;
    [SerializeField] private PooledEffectSource _deathEffectPrefab;
    [Header("SE")]
    [SerializeField] private AudioSource _damageSEPrefab;
    [SerializeField] private AudioSource _deathSEPrefab;



    private void Reset()
    {
        _enemy = GetComponent<Enemy>();
        if (_enemy == null) Debugger.LogError($" {nameof(EnemyAttackEffectAndAudioInjection)} :{gameObject.name}: に {nameof(Enemy):が存在しません}");
    }

    private void Start()
    {
        _enemy.CharacterParam.OnDamage.AddListener(OnDamage);
        _enemy.CharacterParam.OnDeath.AddListener(OnDeath);
    }

    private void OnDamage(AttackObjectBase attackObj)
    {
        EffectSpawner.Instance.UniqePooledEffectPlay(_damageEffectPrefab, attackObj.gameObject.transform.position, Quaternion.identity, _enemy.transform);
        AudioManager.Instance.UniqePooledSEPlay(_damageSEPrefab, attackObj.gameObject.transform.position);
    }

    private void OnDeath(AttackObjectBase attackObj)
    {
        EffectSpawner.Instance.UniqePooledEffectPlay(_deathEffectPrefab, attackObj.gameObject.transform.position, Quaternion.identity);
        AudioManager.Instance.UniqePooledSEPlay(_deathSEPrefab, attackObj.gameObject.transform.position);
    }
}
