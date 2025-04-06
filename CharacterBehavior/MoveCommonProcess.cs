using System;
using UnityEngine;



/// <summary>
/// 移動時の共通処理
/// </summary>
public static class MoveCommonProcess
{


    /// <summary>
    /// 進行方向障害物チェック
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="mover"></param>
    /// <param name="moveSpeed"></param>
    /// <returns></returns>   
    public static bool CheckObstacle(Vector3 moveDirection, RayLength rayInfo, float moveSpeed, string[] obstacleTagName)
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(rayInfo.StartPosition, moveDirection, moveSpeed * Time.fixedDeltaTime + rayInfo.Length);
        Debug.DrawLine(rayInfo.StartPosition, rayInfo.StartPosition + moveDirection.normalized * rayInfo.Length, Color.green, 1f, false);

        if (hits.Length <= 0) return false;

        foreach (var hit in hits)
        {
            if (Array.Exists(obstacleTagName, tagName => hit.transform.CompareTag(tagName)))
            {
                Debugger.Log($"Exist Obstacle  : {hit.transform.gameObject.name}");
                return true;
            }

        }
        return false;
    }
    /// <summary>
    ///移動時の 壁、床、坂などの障害物のすり抜け防磁 , 障害物方向の速度を打ち消す
    /// </summary>
    /// <param name="moveValue"></param>
    /// <param name="rayInfo"></param>
    /// <param name="raduis">Rayの半径</param>
    /// <param name="limitSlopAngle"></param>
    /// <param name="obstacleTagName"></param>
    /// <returns>処理された速度ベクトル</returns>
    public static Vector3 AntiObstacleProcess(Vector3 moveValue, RayLength rayInfo, float raduis, float limitSlopAngle, string[] obstacleTagName)
    {
        Vector3 returnVelocity = Vector3.zero;
        RaycastHit[] hits = Physics.SphereCastAll(rayInfo.StartPosition, raduis, moveValue, rayInfo.Length);

        Debug.DrawLine(rayInfo.StartPosition, rayInfo.StartPosition + new Vector3(moveValue.x, moveValue.y, moveValue.z).normalized * rayInfo.Length, Color.red, 1f, false);
        foreach (RaycastHit hit in hits)
        {
            foreach (string tagName in obstacleTagName)
            {
                //壁とかに沿って移動するための処理
                if (hit.transform.CompareTag(tagName))
                {
                    Vector3 nomal = hit.normal;//坂の登反角度制限
                    if (hit.transform.CompareTag(StaticCommonParams.GROUND_TAG) && Vector3.Angle(Vector3.up, hit.normal) > limitSlopAngle)
                    {
                        //進行方向を打ち消すベクトル
                        nomal = Vector3.Cross(-Vector3.up, Vector3.Cross(Vector3.up, hit.normal));
                    }

                    float moveDirectionToHitNomalAngle = Vector3.Angle(moveValue, -nomal);
                    //反力計算
                    float antiMag = moveValue.magnitude * Mathf.Cos(Mathf.Deg2Rad * moveDirectionToHitNomalAngle);
                    //反力追加
                    returnVelocity += nomal * antiMag;
                }
            }
        }
        Debug.DrawLine(rayInfo.StartPosition, rayInfo.StartPosition + (moveValue + returnVelocity).normalized * rayInfo.Length, Color.yellow, 1f, false);
        return moveValue + returnVelocity;
    }


}
