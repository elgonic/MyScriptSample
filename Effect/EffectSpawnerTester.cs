using UnityEngine;

/// <summary>
/// EffectSpawnerテスト用
/// </summary>
public class EffectSpawnerTester : MonoBehaviour
{
    [SerializeField] private EffectSpawner.EffectType type;
    [SerializeField] private PooledEffectSource _dynamicTestEffectSouce;
    [SerializeField] private Vector3 _positon;
    [SerializeField] private bool _playCommon = false;
    [SerializeField] private bool _playDynamic = false;

    private EffectSpawner _manager;
    private void Start()
    {
        _manager = GetComponent<EffectSpawner>();
        if (_manager == null)
        {
            Debugger.LogError($"{GetType().Name} は {typeof(EffectSpawner).Name}が存在するオブジェクトに配置してください");
        }
    }

    private void Update()
    {
        if (_manager == null) return;
        if (_playCommon == true)
        {
            _playCommon = false;
            _manager.CommonPooledEffectPlay(type, _positon, Quaternion.identity, transform);
        }
        if (_playDynamic == true)
        {
            _playDynamic = false;
            _manager.UniqePooledEffectPlay(_dynamicTestEffectSouce, _positon, Quaternion.identity, transform);
        }

        //性能テスト用
        //_manager.UniqePooledEffectPlay(_dynamicTestEffectSouce, _positon, Quaternion.identity, transform);
        // _manager.CommonPooledEffectPlay(type, _positon ,Quaternion.identity ,transform);
        // Instantiate(_dynamicTestEffectSouce, _positon, Quaternion.identity, transform);
    }
}
