using UnityEngine;

/// <summary>
/// ロックオンUI
/// </summary>
public class LockOnUI : MonoBehaviour
{
    [SerializeField] private RectTransform _lockonCursorImage;
    private GameCameraComp _comp;

    private void Awake()
    {
        _comp = GameObject.FindFirstObjectByType<GameCameraComp>();
        _comp.OnLockOn.AddListener(() => _lockonCursorImage.gameObject.SetActive(true));
        _comp.OnUnlock.AddListener(() => _lockonCursorImage.gameObject.SetActive(false));
        _lockonCursorImage.gameObject.SetActive(false);
    }
}
