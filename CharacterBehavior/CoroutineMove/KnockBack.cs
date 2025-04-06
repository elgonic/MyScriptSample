using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// ノックバック処理
/// </summary>
public class KnockBack : ICoroutineMove
{

    private Params _params;
    private CharacterParam _characterParam;
    private Coroutine _coroutine;
    public KnockBack(Params moveParams, CharacterParam character)
    {
        _params = moveParams;
        _characterParam = character;
    }

    public Coroutine Move(Vector3 direction)
    {
        return _coroutine = CoroutineHandler.StartStaticCoroutine(MoveCorutine(direction));
    }

    private IEnumerator MoveCorutine(Vector3 direction)
    {
        _characterParam.Rb.velocity = Vector3.zero;
        Vector3 Startposition = _characterParam.Rb.transform.position;
        float startTime = Time.time;
        float beforFlameVlaue = 0;
        float curveValue = 0;
        float moveDistance = 0;
        Vector3 nomalizeDirection = direction.normalized;
        while (Time.time - startTime < _params.MoveTime)
        {
            curveValue = _params.MoveAnimatinCureve.Evaluate((Time.time - startTime) / _params.MoveTime);
            moveDistance = (curveValue - beforFlameVlaue) * _params.MoveDistance;


            ////壁床抜け防止の処理
            //スロープは壁とみなすので 90f 
            Vector3 moveValue = MoveCommonProcess.AntiObstacleProcess(nomalizeDirection * moveDistance, _params.ObstacleCheckRayInfo, _params.ObstacleCheckRadius, 90f, new string[] { StaticCommonParams.WALL_TAG, StaticCommonParams.GROUND_TAG });
            if (_characterParam.gameObject.CompareTag(StaticCommonParams.PLAYER_TAG))
            {
                Debug.DrawLine(_characterParam.Transform.position, _characterParam.Transform.position + moveValue, Color.blue, 1f, false);
            }

            _characterParam.Rb.MovePosition(_characterParam.Transform.position + moveValue);

            beforFlameVlaue = curveValue;
            yield return StaticCommonParams.Yielders.FixedUpdate;
        }

        Vector3 finishPositon = _characterParam.Transform.position;
    }

    public void Stop()
    {
        CoroutineHandler.StopStaticCorutine(_coroutine);
    }

    [Serializable]
    public class Params
    {
        public float MoveTime = 0.2f;
        public float MoveDistance = 30f;
        [NormalizedAnimationCurve] public AnimationCurve MoveAnimatinCureve = StaticCommonParams.NormalizeLiner;
        public LayerMask ObstacleLayer = StaticCommonParams.WallLayer | StaticCommonParams.GroundLayer;


        /// <summary>
        /// 障害物判定に使用するRayの情報
        /// </summary>
        public RayLength ObstacleCheckRayInfo;
        /// <summary>
        /// 障害物確認 Spherecast 半径
        /// </summary>
        public float ObstacleCheckRadius = 0.1f;


    }

}
