using UnityEngine;

/// <summary>
/// PauseUIの管理
/// </summary>
public class PauseUI : MonoBehaviour, IUI
{

    [SerializeField] private MyButton _continueButton;
    [SerializeField] private MyButton _reStartButton;
    [SerializeField] private MyButton _configButton;
    [SerializeField] private MyButton _backTittleButton;

    [SerializeField] private ConfigUI _configUI;

    public MyButton ContinueButton => _continueButton;
    public MyButton ReStartButton => _reStartButton;
    public MyButton ConfigButton => _configButton;
    public MyButton BackTittleButton => _backTittleButton;

    public void Setup()
    {
        _continueButton.onClick.AddListener(Close);
        _reStartButton.onClick.AddListener(Close);
        _configButton.onClick.AddListener(ConfigOpen);
        _backTittleButton.onClick.AddListener(Close);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        _configUI.Back.onClick.RemoveListener(Open);
    }

    private void ConfigOpen()
    {
        _configUI.Back.onClick.AddListener(Open);
        _configUI.Open();
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Transration()
    {

    }
}
