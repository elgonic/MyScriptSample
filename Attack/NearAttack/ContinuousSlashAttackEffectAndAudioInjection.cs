using UnityEngine;

/// <summary>
/// 連続斬撃攻撃にエフェクトと音を注入
/// </summary>
public class ContinuousSlashAttackEffectAndAudioInjection : MonoBehaviour
{
    [SerializeField] private ContinuousSlashAttack _attack;

    [Header("Effects")]
    [SerializeField] private PooledEffectSource _inertialEffect;
    [Header("エフェクト生成時に使用する親オブジェクト , 未設定だとTransform使用")]
    [SerializeField] private Transform @inertialEffectPosition;
    private Transform _inertialEffectPosition
    {
        get
        {
            if (@inertialEffectPosition)
            {
                return @inertialEffectPosition;
            }
            else
            {
                return transform;
            }
        }
    }

    [Header("SE")]
    [SerializeField] private AudioSource _inertiaSE;
    [SerializeField] private AudioSource _attackSE;




    private void Reset()
    {
        _attack = GetComponent<ContinuousSlashAttack>();
        if (_attack == null) Debugger.LogError($" {GetType().Name} :{gameObject.name}: に {GetType().Name}:が存在しません");
    }

    private void Awake()
    {
        _attack.OnInertia.AddListener(OnInertia);
        _attack.OnAttack.AddListener(OnAttack);
    }



    private void OnInertia()
    {
        EffectSpawner.Instance.UniqePooledEffectPlay(_inertialEffect, _inertialEffectPosition.position, Quaternion.identity);
        AudioManager.Instance.UniqePooledSEPlay(_inertiaSE, _inertialEffectPosition.position);
    }

    private void OnAttack()
    {
        AudioManager.Instance.UniqePooledSEPlay(_attackSE, _attack.transform.position);
    }
}
