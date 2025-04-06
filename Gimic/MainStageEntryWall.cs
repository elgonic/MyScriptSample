using UnityEngine;


/// <summary>
/// プレイヤーのメインステージの侵入を検知する壁
/// </summary>
[RequireComponent(typeof(Collider))]
public class MainStageEntryWall : MonoBehaviour
{

    private Collider _collider;
    private bool _isEntry = false;

    private LayerMask _defaultExcuteLayer;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        Reset();
        MainGameSystem.Instance.OnPlayerRespwn.AddListener(Reset);
        _defaultExcuteLayer = _collider.excludeLayers;
    }

    /// <summary>
    /// PlayerがZ方向側に出ていたら壁判定を有効化する
    /// </summary>

    private void Update()
    {
        if (!_isEntry && MainGameSystem.Instance.Player.transform.position.z - transform.position.z > 0)
        {
            _collider.excludeLayers = StaticCommonParams.Player;
            _isEntry = true;
            MainGameSystem.Instance.MainStageEntry();
        }
    }

    private void Reset()
    {

        _collider.excludeLayers = _defaultExcuteLayer;
        _collider.enabled = true;
        _isEntry = false;
    }

}
