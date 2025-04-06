using UnityEngine;

/// <summary>
/// 近接攻撃に効果音とエフェクトを注入
/// </summary>
[RequireComponent(typeof(Slash))]
public class SlashAudioEffectInjection : MonoBehaviour
{

    [SerializeField] private PooledEffectSource _effect;
    [Header("エフェクト生成位置 , 無ければこのオブジェクトの位置")]
    [SerializeField] private Transform @effectPosition;

    private Transform _effectPosition
    {
        get
        {
            if (@effectPosition) return @effectPosition;
            return transform;
        }
    }

    private Slash _slash;

    private void Awake()
    {
        _slash = GetComponent<Slash>();
        _slash.OnAttack.AddListener(Attack);

    }

    private void Start()
    {

    }

    private void Attack()
    {
        PooledEffectSource instantiateEffect = EffectSpawner.Instance.UniqePooledEffectPlay(_effect, _effectPosition.position, _effectPosition.rotation);
    }

}
