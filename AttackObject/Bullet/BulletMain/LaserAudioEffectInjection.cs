using UnityEngine;

/// <summary>
/// レーザーに効果音とエフェクトの注入
/// </summary>
public class LaserAudioEffectInjection : MonoBehaviour
{

    [SerializeField] private Laser _laser;

    [Header("SE")]
    [SerializeField] private AudioSource _anticipationSE;
    [SerializeField] private AudioSource _fireSE;


    private void Reset()
    {
        _laser = GetComponent<Laser>();
    }


    private void Awake()
    {
        _laser.OnAnticipation.AddListener(Anticipation);
        _laser.OnFire.AddListener(Fire);
    }

    private void Hit()
    {
    }

    private void Parry(AttackObjectBase attackObject)
    {
    }

    private void Anticipation()
    {
        AudioManager.Instance.UniqePooledSEPlay(_anticipationSE, _laser.transform.position, _laser.transform);
    }

    private void Fire()
    {
        AudioManager.Instance.UniqePooledSEPlay(_fireSE, _laser.transform.position, _laser.transform);
    }

}
