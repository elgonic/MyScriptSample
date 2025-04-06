using UnityEngine;


/// <summary>
/// シールドに対して効果音とエフェクトの注入
/// </summary>
public class CombatShieldAudioEffectInjection : MonoBehaviour
{

    [SerializeField] private CombatShield _shield;

    [Header("Effect")]
    [SerializeField] private PooledEffectSource _break;
    [Header("SE")]
    [SerializeField] private AudioSource _hitSE;
    [SerializeField] private AudioSource _breakSE;


    private void Reset()
    {
        _shield = GetComponent<CombatShield>();
    }


    private void Awake()
    {
        _shield.OnHit.AddListener(Hit);
        _shield.OnParryed.AddListener(Parry);
    }

    private void OnDisable()
    {
        EffectSpawner.Instance.UniqePooledEffectPlay(_break, _shield.gameObject.transform.position, _shield.transform.rotation);
        AudioManager.Instance.UniqePooledSEPlay(_breakSE, _shield.gameObject.transform.position);
    }

    private void Hit()
    {
        AudioManager.Instance.UniqePooledSEPlay(_hitSE, _shield.gameObject.transform.position);
    }

    private void Parry(AttackObjectBase attackObject)
    {
        Hit();
    }



}
