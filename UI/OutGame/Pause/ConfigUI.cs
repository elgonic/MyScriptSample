using UnityEngine;

/// <summary>
/// ConfigUIの管理
/// </summary>
public class ConfigUI : MonoBehaviour, IUI
{
    [SerializeField] private MyButton _soundConfigButton;
    [SerializeField] private MyButton _inputConfigButton;
    [SerializeField] private MyButton _back;

    [SerializeField] private InputDeviceConfig _inputConfig;



    public MyButton SoundConfig => _soundConfigButton;
    public MyButton InputConfig => _inputConfigButton;
    public MyButton Back => _back;

    public void Setup()
    {
        _inputConfigButton.onClick.AddListener(() =>
        {
            _inputConfig.Open();
            Close();
        });
        _inputConfig.BuckButton.onClick.AddListener(() =>
        {
            Open();
        });
        _back.onClick.AddListener(Close);
        Close();
    }

    public void Open()
    {
        gameObject.gameObject.SetActive(true);
    }


    public void Close()
    {
        gameObject.gameObject.SetActive(false);


    }

    public void Transration()
    {

    }
}
