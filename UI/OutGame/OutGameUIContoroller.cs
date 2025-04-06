using UnityEngine;


/// <summary>
/// ゲーム外のUI管理
/// </summary>
/// <remarks>
/// PauseやConfigなど
/// </remarks>
public class OutGameUIContoroller : MonoBehaviour
{
    [SerializeField] private PauseUI _pauseUI;
    [SerializeField] private ConfigUI _configUI;
    [SerializeField] private InputDeviceConfig _inputDeviceConfig;


    public PauseUI PauseUI
    {
        get { return _pauseUI; }
    }

    public ConfigUI ConfigUI
    {
        get { return _configUI; }
    }

    public InputDeviceConfig InputDeviceConfig
    {
        get { return _inputDeviceConfig; }
    }

    private void Awake()
    {
        _pauseUI.Setup();
        _configUI.Setup();
        _inputDeviceConfig.gameObject.SetActive(false);

        AllUISetSE();


    }

    public void AllUIClose()
    {
        foreach (var element in gameObject.GetComponentsInChildren<Component>())
        {
            IUI ui = element as IUI;
            if (ui == null) continue;
            ui.Close();
        }

    }


    public void AllUISetSE()
    {
        foreach (var element in gameObject.GetComponentsInChildren<Component>(true))
        {
            IUI ui = element as IUI;
            if (ui == null) continue;
            UISEInjection.ButtonSE(element.transform);
        }
    }

}
