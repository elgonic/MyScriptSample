using System.Collections;
using UnityEngine;


/// <summary>
/// Poolされるエフェクト
/// </summary>
public class PooledEffectSource : PooledSouce
{

    [SerializeField] private GameObject _effectObject;
    [SerializeField] private bool IsEnableLifeTime = true;
    public float Length = 1;
    public GameObject Effect { get => _effectObject; }

    private bool _isRelease = false;


    private void Reset()
    {
        _effectObject = gameObject;
    }

    /// <summary>
    /// エフェクト再生処理
    /// </summary>
    /// <param name="sePool">対象音源のオブジェクトプール</param>
    /// <param name="position"></param>
    public void Play(Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {
        _isRelease = false;
        StartCoroutine(PlayCorutine(position, rotation, parent));
    }
    private IEnumerator PlayCorutine(Vector3? position = null, Quaternion? rotation = null, Transform parent = null)
    {

        if (position.HasValue)
        {
            transform.position = position.Value;
        }

        if (rotation.HasValue)
        {
            transform.rotation = rotation.Value;
        }

        if (parent) transform.SetParent(parent);

        ///Effentの具体的処理

        gameObject.SetActive(true);

        if (IsEnableLifeTime)
        {
            yield return StaticCommonParams.Yielders.Get(Length);

            Release();
        }
    }

    public void Stop()
    {
        gameObject.SetActive(false);
        Release();
    }

    public override void Release()
    {
        if (_isRelease) return;
        _isRelease = true;
        Pool?.Release(this);
    }
}
