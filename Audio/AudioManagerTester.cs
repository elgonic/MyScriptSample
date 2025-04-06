using UnityEngine;

/// <summary>
/// AudioMangerテスト用
/// </summary>
public class AudioManagerTester : MonoBehaviour
{
    [SerializeField] private AudioManager.AudioType type;
    [SerializeField] private bool _play = false;

    private AudioManager _audioManager;
    private void Start()
    {
        _audioManager = GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debugger.LogError($"{GetType().Name} は {typeof(AudioManager).Name}が存在するオブジェクトに配置してください");
        }
    }

    private void Update()
    {
        if (_audioManager == null) return;
        if (_play == false) return;
        _play = false;
        _audioManager.CommonPooledSEPlay(type, Vector3.zero, transform);
    }
}
