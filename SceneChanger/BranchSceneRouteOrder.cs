using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 分岐シーン用のルートデータ
/// </summary>
[CreateAssetMenu(fileName = "ActionStageOrder", menuName = StaticCommonParams.BaseMenuName + "BranchSceneRouteData")]
public class BranchSceneRouteOrder : ScriptableObject
{
    public enum ResultType
    {
        Good = 0,
        Great = 1,
        Excellent = 2
    }

    [SerializeField] private List<BranchSceneData> _routeSceneList;

    public List<BranchSceneData> RouteSceneList => _routeSceneList;

    [Serializable]
    public class BranchSceneData
    {
        [SerializeField] private SceneName _sceneName;
        [Header("Great条件")]
        [SerializeField] private GoodResultConditions _conditions;

        public SceneName SceneName => _sceneName;
        public GoodResultConditions Condition => _conditions;


        public ResultType Result(Score score)
        {
            return _conditions.Result(score);
        }

    }

    [Serializable]
    public class GoodResultConditions
    {
        [SerializeField] private float _upperTime;
        [SerializeField] private float _lowerTime;
        public float UpperTime => _upperTime;
        public float LowerTime => _lowerTime;

        public ResultType Result(Score score)
        {
            if (score.Time < UpperTime) return ResultType.Excellent;
            if (score.Time > LowerTime) return ResultType.Good;
            return ResultType.Great;
        }
    }


    public class Score
    {
        public float Time { get; private set; }

        public Score(float Time)
        {
            this.Time = Time;
        }

    }
}


