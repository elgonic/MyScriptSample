using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//***************************************************************************
/// <summary> 
/// 入力デバイスの設定用クラス
/// </summary>
//***************************************************************************
public class InputDeviceConfig : MonoBehaviour, IUI
{
    //***************************************************************************
    /// <summary> 
    /// キーボード、マウス使用時の設定データクラス
    /// </summary>
    //***************************************************************************
    [Serializable]
    public class MouceConfig
    {
        public string schemeName = InputDeviceManager.SchemeDic[InputDeviceManager.SchemeList.KeyboardAndMouse];
        public string actionMapName;
        public string actionName;
        public bool invertX;
        public bool invertY;
        public int scaleX;
        public int scaleY;

        public MouceConfig(MouceConfig mouceConfig)
        {
            actionMapName = mouceConfig.actionMapName;
            actionName = mouceConfig.actionName;
            invertX = mouceConfig.invertX;
            invertY = mouceConfig.invertY;
            scaleX = mouceConfig.scaleX;
            scaleY = mouceConfig.scaleY;
        }

        public MouceConfig(string actionMapName, string actionName, bool invertX, bool invertY, int scaleX, int scaleY)
        {
            this.actionMapName = actionMapName;
            this.actionName = actionName;
            this.invertX = invertX;
            this.invertY = invertY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }

        public void Set(string actionMapName, string actionName, bool invertX, bool invertY, int scaleX, int scaleY)
        {
            this.actionMapName = actionMapName;
            this.actionName = actionName;
            this.invertX = invertX;
            this.invertY = invertY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
        }

        public string SchemeName()
        {
            return schemeName;
        }
    }


    //***************************************************************************
    /// <summary> 
    /// コントローラー使用時の設定データクラス
    /// </summary>
    //***************************************************************************
    [Serializable]

    public class GamepadConfig
    {

        public string schemeName = InputDeviceManager.SchemeDic[InputDeviceManager.SchemeList.Gamepad];

        public string actionMapName;
        public string actionName;
        public bool invertX;
        public bool invertY;

        public GamepadConfig(GamepadConfig gamepadConfig)
        {
            actionMapName = gamepadConfig.actionMapName;
            actionName = gamepadConfig.actionName;
            invertX = gamepadConfig.invertX;
            invertY = gamepadConfig.invertY;
        }

        public GamepadConfig(string actionMapName, string actionName, bool invertX, bool invertY)
        {
            this.actionMapName = actionMapName;
            this.actionName = actionName;
            this.invertX = invertX;
            this.invertY = invertY;
        }
        public void Set(string actionMapName, string actionName, bool invertX, bool invertY)
        {
            this.actionMapName = actionMapName;
            this.actionName = actionName;
            this.invertX = invertX;
            this.invertY = invertY;
        }
        public string SchemeName()
        {
            return schemeName;
        }
    }



    [SerializeField] private InputActionAsset _inputActionAsset;
    private InputAction _targetAction = null;


    private static readonly string _configFolderName = "InputConfig";
    /// <summary> 設定を適用するボタン</summary>
    [SerializeField] private MyButton _applyButton;
    /// <summary> 設定を初期に戻すボタン</summary>
    [SerializeField] private MyButton _resetAllButton;

    /// <summary> 戻るボタン</summary>
    [SerializeField] private MyButton _backButton;
    public MyButton BuckButton { get { return _backButton; } }

    [SerializeField] private float _sensitivityMaxValue = 100;
    [SerializeField] private float _sensitivityMinValue = 0;


    [Header("Mouce Config")]
    [SerializeField] private string _mouceConfigFileName = "MouceConfig";
    [SerializeField] private Toggle _mouceVertialInvertCheckbox;
    [SerializeField] private Toggle _mouceHorizontalInvertCheckbox;
    [SerializeField] private SyncSliderWithInputField _mouceVerticalSensitivity;
    [SerializeField] private SyncSliderWithInputField _mouceHorizontalSensitivity;

    [SerializeField] private string _mouceActionMapName;
    [SerializeField] private string _mouceActionName;
    [SerializeField] private bool _mouceInvertX;
    [SerializeField] private bool _mouceInvertY;
    [SerializeField] private int _mouceScaleX;
    [SerializeField] private int _mouceScaleY;


    private MouceConfig _mouceConfig;
    private MouceConfig _defoultMouceConfig;

    [Header("Gamepad Config")]
    [SerializeField] private string _gamepadConfigFileName = "GamepadConfig";
    [SerializeField] private Toggle _gamepadVertialInvertCheckbox;
    [SerializeField] private Toggle _gamepadHorizontalInvertCheckbox;

    [SerializeField] private string _gamepadActionMapName;
    [SerializeField] private string _gamepadActionName;
    [SerializeField] private bool _gamepadInvertX;
    [SerializeField] private bool _gamepadInvertY;

    private GamepadConfig _gamepadConfig;
    private GamepadConfig _defoultGamepadConfig;

    [Header("GamepadとMouceでUiの変更を行うために使用")]
    [SerializeField] private GameObject _gamepadConfigUIComp;
    [SerializeField] private GameObject _mouceConfigUIComp;
    [SerializeField] private GameObject _otherConfigUIComp;
    private Vector2 _gamepadConfigUICompDefPosition;
    private Vector2 _otherConfigUICompDefpotision;
    [SerializeField] private ScrollRect _scrollRect;

    private void Start()
    {
        _mouceVerticalSensitivity.SetValueRange(_sensitivityMaxValue, _sensitivityMinValue);
        _mouceHorizontalSensitivity.SetValueRange(_sensitivityMaxValue, _sensitivityMinValue);

        //ボタン処理を登録
        _applyButton.onClick.AddListener(ApplyConfig);
        _resetAllButton.onClick.AddListener(ResetAllConfig);
        _backButton.onClick.AddListener(Close);
    }
    public void Open()
    {
        gameObject.SetActive(true);

        _gamepadConfigUICompDefPosition = _gamepadConfigUIComp.GetComponent<RectTransform>().anchoredPosition;
        _otherConfigUICompDefpotision = _otherConfigUIComp.GetComponent<RectTransform>().anchoredPosition;
        switch (InputDeviceManager.Instance.CurrentSchemeType)
        {
            case InputDeviceManager.SchemeList.Gamepad:

                _mouceConfigUIComp.SetActive(false);
                _gamepadConfigUIComp.SetActive(true);
                _otherConfigUIComp.SetActive(true);

                float locatedPosition = _mouceConfigUIComp.GetComponent<RectTransform>().anchoredPosition.y;
                _gamepadConfigUIComp.GetComponent<RectTransform>().anchoredPosition = new Vector2(_gamepadConfigUICompDefPosition.x, locatedPosition);
                _otherConfigUIComp.GetComponent<RectTransform>().anchoredPosition = new Vector2(_otherConfigUICompDefpotision.x, locatedPosition + (_otherConfigUICompDefpotision.y - _gamepadConfigUICompDefPosition.y));

                //スクロールバーを毎回リセットしないとGamePadの時offsetの値がおかしくなる
                _scrollRect.verticalNormalizedPosition = 1;
                _scrollRect.vertical = false;
                break;
            case InputDeviceManager.SchemeList.KeyboardAndMouse:
                _mouceConfigUIComp.SetActive(true);
                _gamepadConfigUIComp.SetActive(true);
                _otherConfigUIComp.SetActive(true);
                _scrollRect.verticalNormalizedPosition = 1;
                _scrollRect.vertical = true;
                break;

        }
        SetUpConfigUI();
    }
    public void Close()
    {
        _gamepadConfigUIComp.GetComponent<RectTransform>().anchoredPosition = _gamepadConfigUICompDefPosition;
        _otherConfigUIComp.GetComponent<RectTransform>().anchoredPosition = _otherConfigUICompDefpotision;
        gameObject.SetActive(false);
    }

    public void Transration()
    {

    }


    /// <summary> 
    /// マウスの設定をActionMapに適応させる
    /// </summary>
    private void OverrideTargetAction(MouceConfig mouceConfig)
    {
        _targetAction = _inputActionAsset.FindActionMap(mouceConfig.actionMapName).FindAction(mouceConfig.actionName);
        if (_targetAction == null)
        {
            Debug.Log($"{mouceConfig.actionName} is not Found \n You shoud to Check ActionMap Name or Action Name or Action Asset");
            return;
        }
        Debug.Log(" BindigOverride RebindKeyboardANdMouce");
        _targetAction.ApplyBindingOverride(new InputBinding { groups = mouceConfig.SchemeName(), overrideProcessors = $"scaleVector2(x={_mouceConfig.scaleX * 0.001},y={_mouceConfig.scaleY * 0.001}),invertVector2(invertY={_mouceConfig.invertY},invertX={_mouceConfig.invertX})" });
        string mouceConfigJson = JsonUtility.ToJson(_mouceConfig);
        SaveConfig(_mouceConfigFileName, mouceConfigJson);

    }

    /// <summary> 
    /// コントローラーの設定をActionMapに適応させる
    /// </summary>
    private void OverrideTargetAction(GamepadConfig gamepadConfig)
    {
        _targetAction = _inputActionAsset.FindActionMap(gamepadConfig.actionMapName).FindAction(gamepadConfig.actionName);
        if (_targetAction == null)
        {
            Debug.Log($"{gamepadConfig.actionName} is not Found \n You shoud to Check ActionMap Name or Action Name or Action Asset");
            return;
        }
        Debug.Log(" BindigOverride RebindGamepad");
        _targetAction.ApplyBindingOverride(new InputBinding { groups = gamepadConfig.SchemeName(), overrideProcessors = $"invertVector2(invertY={gamepadConfig.invertY},invertX={gamepadConfig.invertX})" });
        string configJson = JsonUtility.ToJson(gamepadConfig);
        SaveConfig(_gamepadConfigFileName, configJson);

    }



    /// <summary> 
    /// UIで設定された情報を、各入力デバイスの設定用クラスの格納する
    /// </summary>
    private void InportConfigFromUI()
    {
        _mouceConfig = new MouceConfig(_mouceConfig.actionMapName, _mouceConfig.actionName, _mouceHorizontalInvertCheckbox.isOn, _mouceVertialInvertCheckbox.isOn, (int)_mouceHorizontalSensitivity.GetValue(), (int)_mouceVerticalSensitivity.GetValue());
        _gamepadConfig = new GamepadConfig(_gamepadConfig.actionMapName, _gamepadConfig.actionName, _gamepadHorizontalInvertCheckbox.isOn, _gamepadVertialInvertCheckbox.isOn);

    }

    public void ApplyConfig()
    {
        InportConfigFromUI();
        OverrideTargetAction(_mouceConfig);
        OverrideTargetAction(_gamepadConfig);
    }
    public void LoadConfig()
    {
        if (_defoultMouceConfig == null || _defoultGamepadConfig == null)
        {
            SaveDefoultConfigValue();
        }
        LoadMouceConfig(_mouceConfigFileName);
        LoadGamepadConfig(_gamepadConfigFileName);
    }
    private void LoadMouceConfig(string fileName)
    {
        string filePath = Path.Combine(CreateFolderPath(_configFolderName), fileName);
        if (System.IO.File.Exists(filePath))
        {
            string readConfigData = File.ReadAllText(filePath);
            _mouceConfig = JsonUtility.FromJson<MouceConfig>(readConfigData);
        }
        else
        {
            //ファイルがないと_mouceconfigがnull
            _mouceConfig = new MouceConfig(_defoultMouceConfig);
        }
        OverrideTargetAction(_mouceConfig);
    }
    private void LoadGamepadConfig(string fileName)
    {

        string filePath = Path.Combine(CreateFolderPath(_configFolderName), fileName);
        if (System.IO.File.Exists(filePath))
        {
            string readConfigData = File.ReadAllText(filePath);
            _gamepadConfig = JsonUtility.FromJson<GamepadConfig>(readConfigData);
        }
        else
        {
            //設定ファイルが無いと _gamepadConfig が null 
            _gamepadConfig = new GamepadConfig(_defoultGamepadConfig);
        }
        OverrideTargetAction(_gamepadConfig);
    }


    private void SaveConfig(string fileName, string configJson)
    {
        string folderPath = CreateFolderPath(_configFolderName);
        string filePath = Path.Combine(folderPath, fileName);
        if (System.IO.Directory.Exists(folderPath) == false)
        {
            Debug.Log($"Create Folder :  {folderPath}");
            Directory.CreateDirectory(folderPath);
        }

        using (StreamWriter file = new StreamWriter(filePath, false))
        {
            file.Write(configJson);
        }

    }

    private string CreateFolderPath(string folderName)
    {
        return Path.Combine(StaticCommonParams.UserDataBaseFolderName, folderName);
    }

    private void SetUpConfigUI()
    {
        if (_defoultGamepadConfig == null)
        {
            SaveDefoultConfigValue();
            ResetAllConfig();
        }


        SetUpConfigUI(_mouceConfig);
        SetUpConfigUI(_gamepadConfig);
    }
    private void SetUpConfigUI(MouceConfig mouceConfig)
    {
        _mouceVertialInvertCheckbox.isOn = mouceConfig.invertY;
        _mouceHorizontalInvertCheckbox.isOn = mouceConfig.invertX;
        _mouceVerticalSensitivity.SetValue(mouceConfig.scaleY);
        _mouceHorizontalSensitivity.SetValue(mouceConfig.scaleX);
    }
    private void SetUpConfigUI(GamepadConfig gamepadConfig)
    {
        _gamepadVertialInvertCheckbox.isOn = gamepadConfig.invertY;
        _gamepadHorizontalInvertCheckbox.isOn = gamepadConfig.invertX;
    }

    public void SaveDefoultConfigValue()
    {
        //初期のデフォルト値保存
        _defoultMouceConfig = new MouceConfig(_mouceActionMapName, _mouceActionName, _mouceInvertX, _mouceInvertY, _mouceScaleX, _mouceScaleY);
        _defoultGamepadConfig = new GamepadConfig(_gamepadActionMapName, _gamepadActionName, _gamepadInvertX, _gamepadInvertY);
    }
    public void ResetAllConfig()
    {
        _mouceConfig = new MouceConfig(_defoultMouceConfig);
        _gamepadConfig = new GamepadConfig(_defoultGamepadConfig);
        OverrideTargetAction(_mouceConfig);
        OverrideTargetAction(_gamepadConfig);
        SetUpConfigUI();
    }
}




