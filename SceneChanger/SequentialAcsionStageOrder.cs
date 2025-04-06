using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ゲームステージの進行順
/// </summary>

[CreateAssetMenu(fileName = "ActionStageOrder", menuName = StaticCommonParams.BaseMenuName + "SequentialActionStageOrder")]
public class SequentialAcsionStageOrder : ScriptableObject
{
    [SerializeField] private List<SceneName> _sceneOrder;
    public List<SceneName> SceneOrder => _sceneOrder;

}

