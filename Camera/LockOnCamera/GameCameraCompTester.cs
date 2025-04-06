using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
# endif


/// <summary>
/// GameCameraCompのテストコンポーネント
/// </summary>
public class GameCameraCompTester : MonoBehaviour
{
    [SerializeField] private GameCameraComp.LockCameraOffsetType _toCameraOffsetType;
    [SerializeField] private bool _lockCameraOffsetChange = false;


    [SerializeField] private GameCameraComp _comp;


    private void Reset()
    {
        _comp = GetComponent<GameCameraComp>();
    }
    private void OnValidate()
    {
        if (_lockCameraOffsetChange)
        {
            _lockCameraOffsetChange = false;
#if UNITY_EDITOR
            if (EditorApplication.isPlaying) _comp.ChangeLockCameraOffset(_toCameraOffsetType);
# endif
        }
    }
}
