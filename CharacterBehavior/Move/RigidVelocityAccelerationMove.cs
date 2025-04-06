using System;
using UnityEngine;

/// <summary>
/// 移動開始時に設定速度に対してアニメーションカーブに沿って加速する
/// </summary>
public class RigidVelocityAccelerationMove : IMove
{
    //8方向分割だと 90度以上　
    private float _thresholdAngle = 100f;
    private Rigidbody _rb;
    private Params _params;
    private Vector2 _moveValue;
    private float _accelerationTimer = 0f;
    private Vector2 _beforeDirection = Vector2.zero;
    public RigidVelocityAccelerationMove(Params moveParams, Rigidbody rb)
    {
        _rb = rb;
        _params = moveParams;
    }

    public Vector3 Move(Vector2 moveDirection)
    {
        //現在の速度を維持するためにアーリーリターンさせている
        if (moveDirection == Vector2.zero)
        {
            _accelerationTimer = 0f;
            _beforeDirection = moveDirection;
            return Vector3.zero;
        }

        //ノーマライズしておく
        moveDirection = moveDirection.normalized;

        //指定角度以内の入力角度変更であれば速度維持
        if (Vector2.Angle(moveDirection, _beforeDirection) >= _thresholdAngle)
        {
            _accelerationTimer = 0f;
        }
        if (_accelerationTimer <= _params.AccelerationTime)
        {
            _moveValue = moveDirection * _params.MoveSpeed * _params.AccelerationCurve.Evaluate(_accelerationTimer / _params.AccelerationTime);
        }
        else
        {
            _moveValue = moveDirection * _params.MoveSpeed;
        }

        Vector3 velocity = new Vector3(_moveValue.x, _rb.velocity.y, _moveValue.y);

        if (_params.EnableAntiProcess) velocity = MoveCommonProcess.AntiObstacleProcess(velocity, _params.ObstacleCheckRayInfo, _params.ObstacleCheckRadius, _params.LimitSloapAngle, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });


        _rb.velocity = velocity;
        _beforeDirection = moveDirection;
        _accelerationTimer += Time.deltaTime;

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
        public float AccelerationTime = 0.2f;
        /// <summary>
        /// 壁、床、坂の処理を行うか
        /// </summary>
        public bool EnableAntiProcess = true;
        [NormalizedAnimationCurve] public AnimationCurve AccelerationCurve = StaticCommonParams.NormalizeLiner;

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
