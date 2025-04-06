using System;
using UnityEngine;

/// <summary>
/// 設定速度に瞬間的に加速する
/// </summary>
public class RigidVelocityImpulseMove : IMove
{
    private Rigidbody _rb;
    private Params _params;

    private Vector2 _moveValue;
    public RigidVelocityImpulseMove(Params moveParams, Rigidbody rb)
    {
        _rb = rb;
        _params = moveParams;
    }

    public Vector3 Move(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero) return Vector3.zero;
        _moveValue = moveDirection * _params.MoveSpeed;


        Vector3 velocity = new Vector3(_moveValue.x, _rb.velocity.y, _moveValue.y);

        if (_params.EnableAntiProcess) velocity = MoveCommonProcess.AntiObstacleProcess(velocity, _params.ObstacleCheckRayInfo, _params.ObstacleCheckRadius, _params.LimitSloapAngle, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });

        _rb.velocity = velocity;

        return velocity;
    }

    public void Stop()
    {
        _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
    }

    [Serializable]
    public class Params
    {
        public float MoveSpeed = 5f;
        public float LimitSloapAngle = 50f;
        /// <summary>
        /// 壁、床、坂の処理を行うか
        /// </summary>
        public bool EnableAntiProcess = true;
        /// <summary>
        /// 障害物判定に使用するRayの情報
        /// </summary>
        public RayLength ObstacleCheckRayInfo;
        /// <summary>
        /// 障害物確認 SphereCast 半径
        /// </summary>
        public float ObstacleCheckRadius;

    }
}
