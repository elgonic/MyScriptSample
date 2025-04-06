using UnityEngine;



/// <summary>
/// 弾の軌跡管理クラス
/// </summary>
public class TraileController : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    private void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        _trailRenderer?.Clear();
    }

}
