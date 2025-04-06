using UnityEngine;

/// <summary>
/// カーソルの管理
/// </summary>
public class CursorManger : MonoBehaviour
{

    private enum CursorMode
    {
        UI,
        InGame
    }

    private bool _isAddEvent = false;
    private CursorMode _mode;
    public void UI()
    {
        _mode = CursorMode.UI;
        CursorProcess();
        if (!_isAddEvent)
        {
            InputDeviceManager.Instance.OnChangeDeviceType.AddListener(CursorProcess);
            _isAddEvent = true;
        }
    }
    public void InGame()
    {
        _mode = CursorMode.InGame;
        CursorProcess();
        if (!_isAddEvent)
        {
            InputDeviceManager.Instance.OnChangeDeviceType.AddListener(CursorProcess);
            _isAddEvent = true;
        }
    }
    private void CursorProcess()
    {
        switch (_mode)
        {
            case CursorMode.UI:
                if (InputDeviceManager.Instance.CurrentDeviceType == InputDeviceManager.InputDeviceType.Keyboard)
                {
                    UnityEngine.Cursor.visible = true;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    UnityEngine.Cursor.visible = false;
                }
                break;

            case CursorMode.InGame:
                if (InputDeviceManager.Instance.CurrentDeviceType == InputDeviceManager.InputDeviceType.Keyboard)
                {
                    UnityEngine.Cursor.visible = false;
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    UnityEngine.Cursor.visible = false;
                }
                break;
            default: break;
        }
    }
}
