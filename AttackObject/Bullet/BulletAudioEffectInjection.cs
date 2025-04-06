using UnityEngine;

/// <summary>
/// 弾に対しての効果音とエフェクトの設定
/// </summary>
[RequireComponent(typeof(BulletBase))]
public class BulletAudioEffectInjection : MonoBehaviour
{
    [SerializeField] private PooledEffectSource _hitEffect;

    private BulletBase _bulletBase;


    private bool _isDispCamera = false;

    private void Start()
    {
        _bulletBase = GetComponent<BulletBase>();
        _bulletBase.OnFire.AddListener(Fire);
        _bulletBase.OnHit.AddListener(Hit);
        _bulletBase.OnLifeTime.AddListener(Hide);
    }

    private void OnBecameVisible()
    {
        _isDispCamera = true;
    }

    private void OnBecameInvisible()
    {
        _isDispCamera = false;
    }

    private void Fire()
    {
    }

    private void Hit()
    {
        if (_isDispCamera) MainGameSystem.Instance.EffectSpawner.UniqePooledEffectPlay(_hitEffect, transform.position, Quaternion.identity);
    }

    private void Hide()
    {
        if (_isDispCamera) MainGameSystem.Instance.EffectSpawner.UniqePooledEffectPlay(_hitEffect, transform.position, Quaternion.identity);
    }

}
