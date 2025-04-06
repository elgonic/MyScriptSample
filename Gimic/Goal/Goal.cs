using System.Collections;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// ゴール
/// </summary>
public class Goal : MonoBehaviour
{
    /// <summary>
    /// ゴール出現から有効になるまでの少しの待ち時間
    /// </summary>
    /// <remarks>
    /// ゴール出現位置とプレーヤーが重なっていると即次のステージに移動してしまうので必要
    /// </remarks>
    [Header("ゴール出現から有効になるまでの少しの待ち時間")]
    [SerializeField] private float _enabaleWaiteTie = 0f;

    /// <summary>
    /// ゴールのオブジェクト
    /// </summary>
    [SerializeField] private BaseGoalObjet _goalObjet;
    /// <summary>
    /// カメラのターゲット用
    /// </summary>
    [SerializeField] private Transform _lockonTarget;
    public Transform LockOnTarget => _lockonTarget;

    public UnityEvent OnGoal;


    private bool _isEbabled = false;

    private void Awake()
    {
        _lockonTarget.gameObject.SetActive(false);
    }

    public void Ebable()
    {
        StartCoroutine(EnableCoroutine());
    }

    private IEnumerator EnableCoroutine()
    {
        _lockonTarget.gameObject.SetActive(true);
        yield return _goalObjet.Enable();
        yield return StaticCommonParams.Yielders.Get(_enabaleWaiteTie);
        _isEbabled = true;
    }

    public void Disable()
    {
        _goalObjet.Disable();
        _lockonTarget.gameObject.SetActive(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (_isEbabled && other.CompareTag(StaticCommonParams.PLAYER_TAG))
        {
            _isEbabled = false;
            OnGoal?.Invoke();
        }
    }
}
