using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// タイトルのUI管理
/// </summary>
public class TittleUI : MonoBehaviour, IUI
{

    [SerializeField] private MyButton _startButton;
    [SerializeField] private MyButton _configButton;
    [SerializeField] private MyButton _exitButton;

    [SerializeField] private RawImage _selectUI;
    [SerializeField] private float _selectWidth;

    //効果音とかに使う
    public MyButton StartButton => _startButton;
    public MyButton ExitButton => _exitButton;


    private RectTransform _selectUITransform;
    private EventSystem _eventSystem;
    private GameObject _selectObject;
    private RectTransform _selectObjectTransform;
    private Vector2 _selectObjectWidthCash;


    private void Awake()
    {
        _selectUITransform = _selectUI.GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (_eventSystem == null)
        {
            _eventSystem = EventSystem.current;
        }
        else if (_selectObject != _eventSystem.currentSelectedGameObject)
        {
            if (_eventSystem.currentSelectedGameObject == null) return;
            _selectObject = _eventSystem.currentSelectedGameObject;


            //前回選択UIを元に戻す
            if (_selectObjectTransform) _selectObjectTransform.sizeDelta = _selectObjectWidthCash;

            //選択UIの処理
            _selectObjectTransform = _selectObject.GetComponent<RectTransform>();

            _selectObjectWidthCash = _selectObjectTransform.sizeDelta;
            _selectObjectTransform.sizeDelta = new Vector2(_selectWidth, _selectObjectTransform.sizeDelta.y);

            _selectUITransform.localPosition = new Vector3(
                _selectUITransform.localPosition.x,
                _selectObject.GetComponent<RectTransform>().localPosition.y,
                _selectUITransform.localPosition.z);
        }
        else
        {
            _selectObject = _eventSystem.currentSelectedGameObject;
        }
    }
    public void Setup()
    {
        TittleSystem system = TittleSystem.Instance;
        _startButton.onClick.AddListener(system.GameStart);
        _configButton.onClick.AddListener(() =>
        {
            Close();
            system.OutGameUIController.ConfigUI.Open();
        });
        _exitButton.onClick.AddListener(system.GameExit);
    }

    public void Open()
    {

        TittleSystem.Instance.OutGameUIController.ConfigUI.Back.onClick.RemoveListener(Open);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        TittleSystem.Instance.OutGameUIController.ConfigUI.Back.onClick.AddListener(Open);
        gameObject.SetActive(false);
    }

    public void Transration()
    {

    }
}
