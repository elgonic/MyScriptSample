using UnityEditor;
using UnityEngine;

/// <summary>
/// 敵のいる位置にゴールを作成
/// </summary>
public class CreateGoal : MonoBehaviour
{
    [SerializeField] private bool _create;
    [SerializeField] private Goal _goal;

    private void OnValidate()
    {
        if (_create)
        {
            _create = false;
            Create();

        }
    }

    private void Create()
    {
#if UNITY_EDITOR
        Enemy enemy = GameObject.FindFirstObjectByType<Enemy>();
        if (!enemy)
        {
            Debugger.Log($"Scene内に{typeof(Enemy)}がありません");
            return;
        }
        Goal instatiate = PrefabUtility.InstantiatePrefab(_goal) as Goal;
        instatiate.gameObject.transform.position = enemy.transform.position;
#endif
    }
}
