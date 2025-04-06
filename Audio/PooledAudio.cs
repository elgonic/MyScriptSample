using System.Collections;
using UnityEngine;

/// <summary>
/// Poolされる音源
/// </summary>
public class PooledAudio : PooledSouce
{
    private AudioSource _souce;
    public AudioSource Souce
    {
        get
        {
            if (!_souce)
            {
                _souce = GetComponent<AudioSource>();
            }
            return _souce;
        }
    }

    /// <summary>
    /// オブジェクトプールを使用している汎用音源を鳴らす処理
    /// </summary>
    /// <param name="sePool">対象音源のオブジェクトプール</param>
    /// <param name="position"></param>
    public void Play(Vector3? position = null, Transform parent = null)
    {

        CoroutineHandler.StartStaticCoroutine(PlayCorutine(position, parent));
    }

    private IEnumerator PlayCorutine(Vector3? position = null, Transform parent = null)
    {

        if (position.HasValue)
        {
            Souce.transform.position = position.Value;
        }

        if (parent) Souce.transform.SetParent(parent);

        gameObject.SetActive(true);
        if (!Souce.playOnAwake) Souce.Play();

        float time = Time.deltaTime;
        yield return StaticCommonParams.Yielders.Get(Souce.clip.length);

        Release();

    }

    public override void Release()
    {
        Pool?.Release(this);
    }
}
