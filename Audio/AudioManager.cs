using DesignPatterns.Singleton;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// オーディオを鳴らすクラス
/// 汎用的な音はプール化している
/// </summary>
public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    public enum AudioType
    {
        BGM = 0,
        UIChose = 8,
        UIEnter = 9,
        BulletCharacterHitSE = 1,
        BulletObjectHitSE = 2,
        DashSE = 3,
        ParrySE = 4,
        MoveSE = 5,
        JumpSE = 6,
        WallHitSE = 7,
        Death = 10,
        Damage = 11,
    }

    [Header("BGM")]
    [SerializeField] private AudioSource _bgm;
    public AudioSource Bgm => _bgm;

    [Header("UI")]
    [SerializeField] private AudioSource _chose;
    [SerializeField] private AudioSource _enter;

    [Header("Action")]
    [SerializeField] private AudioSource _bulletCharacterHitSE;
    [SerializeField] private AudioSource _bulletObjectHitSE;
    [SerializeField] private AudioSource _dashSE;
    [SerializeField] private AudioSource _parrySE;
    [SerializeField] private AudioSource _moveSE;
    [SerializeField] private AudioSource _jumpSE;
    [SerializeField] private AudioSource _damageSE;
    [SerializeField] private AudioSource _wallHitSE;
    [SerializeField] private AudioSource _deathSE;

    private List<AudioSource> _playingAudioSources = new List<AudioSource>();


    private Dictionary<AudioType, ObjectPool<PooledSouce>> @pooledAudioDic = new();

    //シーン変更時に初期化されるので, 初期化された場合は再セットアップする
    private Dictionary<AudioType, ObjectPool<PooledSouce>> _pooledAudioDic
    {
        get
        {
            if (@pooledAudioDic.Count == 0) SetPool();
            return @pooledAudioDic;
        }
    }
    private Dictionary<AudioSource, ObjectPool<PooledSouce>> _dynamicPooledAudioDic { get; set; } = new();


    private void Awake()
    {
    }

    public void SetPool()
    {
        AddPoolToDic(AudioType.BulletCharacterHitSE, _bulletCharacterHitSE);
        AddPoolToDic(AudioType.BulletObjectHitSE, _bulletObjectHitSE);
        AddPoolToDic(AudioType.DashSE, _dashSE);
        AddPoolToDic(AudioType.ParrySE, _parrySE);
        AddPoolToDic(AudioType.MoveSE, _moveSE);
        AddPoolToDic(AudioType.JumpSE, _jumpSE);
        AddPoolToDic(AudioType.WallHitSE, _wallHitSE);
        AddPoolToDic(AudioType.UIChose, _chose);
        AddPoolToDic(AudioType.UIEnter, _enter);
        AddPoolToDic(AudioType.Death, _deathSE);
        AddPoolToDic(AudioType.Damage, _damageSE);
    }

    /// <summary>
    /// オブジェクトプールAudioオブジェクトの作成
    /// </summary>
    /// <returns></returns>
    private void AddPoolToDic(AudioType type, AudioSource audioSource = null)
    {
        if (audioSource == null)
        {
            Debugger.LogWarning($"{nameof(AudioManager)} : Not Set {type.ToString()} SE ");
            return;
        }
        PooledAudio pooledAudio = CreatePooledObject(audioSource);

        @pooledAudioDic.Add(type, CreatePool(pooledAudio, transform));
    }

    private PooledAudio CreatePooledObject(AudioSource source)
    {
        PooledAudio pooledAudio = source.gameObject.GetComponent<PooledAudio>();
        if (!pooledAudio)
        {
            pooledAudio = source.gameObject.AddComponent<PooledAudio>();
        }
        return pooledAudio;
    }
    public void CommonPooledSEPlay(AudioType type, Vector3? position = null, Transform parent = null)
    {
        if (_pooledAudioDic.TryGetValue(type, out var audioPool))
        {
            PlayPooledAudio(audioPool.Get(), position, parent);
        }
        else
        {
            Debugger.LogWarning($"{GetType().Name}に{type.ToString()}が設定されていません");
        }
    }
    public void RemovePooled()
    {
        foreach (AudioType type in _pooledAudioDic.Keys)
        {
            _pooledAudioDic[type].Clear();
        }
        _pooledAudioDic.Clear();

        RemoveUniqePooled();
    }




    /// <summary>
    /// AudioTypeには無い各クラス固有の音をプールして鳴らす場合
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    public void UniqePooledSEPlay(AudioSource audioSource, Vector3? position = null, Transform parent = null)
    {
        if (audioSource == null) return;
        if (!_dynamicPooledAudioDic.ContainsKey(audioSource))
        {
            _dynamicPooledAudioDic.Add(audioSource, CreatePool(CreatePooledObject(audioSource), transform));
        }
        PlayPooledAudio(_dynamicPooledAudioDic[audioSource].Get(), position, parent);
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// クラス固有はシーン遷移とかで消したほうがいいので、ステージクリア時に呼ばれるようにする
    /// </remarks>
    public void RemoveUniqePooled()
    {
        foreach (KeyValuePair<AudioSource, ObjectPool<PooledSouce>> dic in _dynamicPooledAudioDic)
        {
            dic.Value.Clear();
        }
        _dynamicPooledAudioDic.Clear();
    }

    private void PlayPooledAudio(PooledSouce pooledSouce, Vector3? position = null, Transform parent = null)
    {
        PooledAudio pooledAudio = pooledSouce as PooledAudio;
        pooledAudio?.Play(position, parent);
    }


    /// <summary>
    /// 任意の音を鳴らす(ループで)
    /// </summary>
    /// <param name="audioSource">鳴らしたい音</param>
    /// <param name="playPosition">鳴らしたい場所</param>
    /// <returns>
    /// 再生したオーディオ情報
    /// </returns>
    /// <remarks>
    /// 再生している音はListで保存している。
    /// </remarks>
    public AudioSource PlayAnyAudio(AudioSource audioSource, Vector3? playPosition = null)
    {
        if (!audioSource)
        {
            Debug.LogWarning($"{audioSource.gameObject.name} のオーディオが未設定");
            return null;
        }
        AudioSource audio = Instantiate(audioSource, transform);
        if (playPosition.HasValue)
        {
            audio.transform.position = playPosition.Value;
        }
        audio.Play();
        Debug.Log($"{audioSource.clip.name} Play!");
        _playingAudioSources.Add(audio);
        return audio;
    }

    /// <summary>
    /// 再生停止
    /// </summary>
    /// <param name="audioSource">止めたい音</param>
    /// <remarks>
    /// 再生中Listから削除する
    /// この性質上 , 同じAudioSouceを重複して鳴らして停止しようとするとどちらが停止するか不明
    /// </remarks>
    public void Stop(AudioSource audioSource)
    {
        AudioSource stopAudio = _playingAudioSources.Find(audio => audio == audioSource);
        if (!stopAudio)
        {
            Debug.LogWarning($"{audioSource} は再生されていません");
            return;
        }
        stopAudio.Stop();
        Debug.Log($"{audioSource.clip.name} Stop!");
        _playingAudioSources.Remove(stopAudio);
    }

    /// <summary>
    /// なっている(SE以外)音全停止
    /// </summary>
    public void StopAll()
    {
        foreach (AudioSource audioSource in _playingAudioSources)
        {
            audioSource.Stop();
        }
        _playingAudioSources.Clear();
    }


    /// <summary>
    /// 音を鳴らす(1度だけ)
    /// </summary>
    /// <param name="audioSource">鳴らしたい音</param>
    /// <param name="playPosition">鳴らしたい場所</param>
    public void PlayOneShot(AudioSource audioSource, Vector3? playPosition)
    {
        if (!audioSource)
        {
            Debug.LogWarning($"{audioSource.gameObject.name} のオーディオが未設定");
            return;
        }

        AudioSource audio = Instantiate(audioSource, transform);

        if (playPosition.HasValue)
        {
            audio.transform.position = playPosition.Value;
        }

        Debug.Log($"{audioSource.clip.name} Play!");
        audio.PlayOneShot(audio.clip);
        Destroy(audio.gameObject, audio.clip.length * Time.timeScale);
    }


    /// <summary>
    /// 汎用音源のオブジェクトプール作製処理
    /// </summary>
    /// <param name="pooledAudioSource">プールしたい音源</param>
    /// <returns>音源のオブジェクトプール</returns>
    private ObjectPool<PooledSouce> CreatePool(PooledAudio pooledAudioSource, Transform baseTr)
    {
        return ObjectPoolCreator.CreatePool(pooledAudioSource, baseTr);
    }

}
