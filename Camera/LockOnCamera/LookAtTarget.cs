using UnityEngine;

/// <summary>
/// カクツキ防止のためChinemachineのLookAtにセットする用
/// 速い速度で敵に体当たりしたときのコライダの抜けによるカメラ反転防止
/// </summary>
public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private GameCameraComp _cameraComp;
    [SerializeField] private GameObject _follow;
    /// <summary>
    /// Targetオブジェクト
    /// </summary>
    public Transform Target;

    [SerializeField] private Vector3 _offset = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    private void FixedUpdate()
    {
        if (!_cameraComp.IsLockOn) return;
        if (Target) transform.LookAt(Target);

        //ダッシュ衝突時にFollow(操作キャラ)が敵のポジションを通り過ぎて一瞬カメラが反転するのを防ぐ処理

        Vector3 distance = (Target.position - _follow.transform.position);
        Vector3 distanceXZ = new Vector3(distance.x, 0, distance.z);
        Plane targetDistancePlane = new Plane(distanceXZ.normalized, Target.position);

        float followDistance = targetDistancePlane.GetDistanceToPoint(_follow.transform.position);
        float thisDistance = targetDistancePlane.GetDistanceToPoint(transform.position);

        //通常時 Enemy -> Player -> Cameraの時
        Vector3 targetFollowDistance = (_follow.transform.position - Target.position).normalized;
        if ((followDistance >= 0 && thisDistance >= 0) || (followDistance < 0 && thisDistance < 0))
        {
            Vector3 offetDirection = new Vector3(targetFollowDistance.x, 0, targetFollowDistance.z).normalized * new Vector2(_offset.x, _offset.z).magnitude;
            transform.position = _follow.transform.position + new Vector3(offetDirection.x, _offset.y, offetDirection.z);
        }

        //異常時 Player -> Enemy -> Cameraの時
        else
        {
            targetFollowDistance = -targetFollowDistance;
            Vector3 offetDirection = new Vector3(targetFollowDistance.x, 0, targetFollowDistance.z).normalized * new Vector2(_offset.x, _offset.z).magnitude;
            transform.position = _follow.transform.position + new Vector3(offetDirection.x, _offset.y, offetDirection.z);
        }

    }
}
