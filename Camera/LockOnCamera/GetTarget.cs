using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ロックオンターゲットの取得処理
/// </summary>
public class GetLockOnTarget : IGetLockOnTarget
{
    [Serializable]
    public class Param
    {
        //カメラ
        public Transform Camera;

        //Lockon関係
        //ロックオン可能距離
        public float LockonRange = 1000;
        /// <summary>
        /// ロックオン可能レイヤー
        /// </summary>
        public LayerMask LockonLayers = 0;
        /// <summary>
        /// 視線ブロックオブジェクトレイヤー
        /// </summary>
        public LayerMask LockonObstacleLayers = 0;
        /// <summary>
        /// ターゲットに対する距離の重みづけ
        /// </summary>
        public float LockonFactor = 0.0006f;

        /// <summary>
        /// ターゲット出来る角度の閾値
        /// </summary>
        public float LockonAngleThreshold = 0.5f;
    }

    private Param _param;

    public GetLockOnTarget(Param param)
    {
        _param = param;
    }

    /// <summary>
    /// ロックオン対象の計算処理を行い取得する
    /// 計算は3つの工程に分かれる
    /// </summary>
    /// <returns></returns>
    public Transform GetTarget()
    {
        Collider[] _hitTargetColliders = Physics.OverlapSphere(_param.Camera.position, _param.LockonRange, _param.LockonLayers);

        //ターゲットの存在確認
        if (_hitTargetColliders.Length == 0) return null;

        //ターゲットとの間に視線が通っているものだけ取得
        List<GameObject> targetableObjects = MakeListLineOfSight(Array.ConvertAll(_hitTargetColliders,
                new Converter<Collider, GameObject>(value => value.gameObject)
                )
            );

        if (targetableObjects.Count == 0) return null;

        //一番画面の中央に近い物を返す
        (float, GameObject) tupleData = GetOptimalTarget(targetableObjects);

        float degreeMinimum = tupleData.Item1;
        GameObject target = tupleData.Item2;

        //角度が閾値より低い場合 値を返す
        if (Mathf.Abs(degreeMinimum) <= _param.LockonAngleThreshold)
        {
            return target.transform;
        }
        else
        {
            return null;
        }
    }



    /// <summary>
    /// 視線が通っているか,間にオブジェクトが挟まっていないかの判断
    /// </summary>
    /// <param name="hits"></param>
    /// <returns>視線が通っているオブジェト</returns>
    private List<GameObject> MakeListLineOfSight(GameObject[] hits)
    {
        List<GameObject> lineOfSightObject = new List<GameObject>();
        foreach (GameObject hitObject in hits)
        {
            if (!Physics.Linecast(_param.Camera.position, hitObject.transform.position, _param.LockonObstacleLayers)) continue;
            lineOfSightObject.Add(hitObject);
        }

        return lineOfSightObject;
    }

    private (float, GameObject) GetOptimalTarget(List<GameObject> targets)
    {
        //world xz 平面上から見た カメラの x ,z の前方向の角度 
        float worldXZPlaneCameraAngle = Mathf.Atan2(_param.Camera.forward.x, _param.Camera.forward.z);
        float degreeMinimum = Mathf.PI * 2;
        GameObject returnTarget = null;

        foreach (GameObject target in targets)
        {
            Vector3 playerCameraToTarget = target.transform.position - _param.Camera.position;
            playerCameraToTarget.y = 0.0f;
            Vector3 playerCameraToTargetNomalize = playerCameraToTarget.normalized;

            //world xz 平面上から見た  camera から見た ターゲットの方向の角度 
            float worldXZPlaneCameraToTargetAngle = Mathf.Atan2(playerCameraToTarget.x, playerCameraToTarget.z);

            // カメラとターゲットの角度差分
            float distanceCameraTargetAngle = AngleNomalize(worldXZPlaneCameraAngle, worldXZPlaneCameraToTargetAngle);

            //距離による重みを考慮した値 
            float calcAngle = distanceCameraTargetAngle + distanceCameraTargetAngle * (playerCameraToTarget.magnitude) * _param.LockonFactor;

            if (Mathf.Abs(degreeMinimum) >= Mathf.Abs(calcAngle))
            {
                degreeMinimum = calcAngle;
                returnTarget = target;
            }
        }

        return (degreeMinimum, returnTarget);
    }



    /// <summary>
    /// ベースアングルを基準にしたTargetAngleを取得する。
    /// その値は -180 ~ 180 に変換される
    /// </summary>
    /// <param name="targetAngle"></param>
    /// <param name="baseAngle"></param>
    /// <returns></returns>
    private float AngleNomalize(float targetAngle, float baseAngle)
    {
        float returnAngle;
        returnAngle = baseAngle - targetAngle;
        //baseAngle (カメラの前方ベクトル) と targetAngle (カメラから敵へのベクトル) との角度差が180°以上
        //角度差分 から360°引いて正規化(-180から180に制限)
        if (Mathf.PI <= returnAngle)
        {
            returnAngle -= Mathf.PI * 2;
        }
        else if (-Mathf.PI >= returnAngle)
        {
            returnAngle += Mathf.PI * 2;
        }
        return returnAngle;
    }


}
