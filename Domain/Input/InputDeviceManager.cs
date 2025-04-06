using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;


/// <summary>
/// 入力デバイス種別を取得
/// </summary>
/// <remarks>
/// 参考先 : https://nekocha.hatenablog.com/entry/2024/03/28/212705
/// </remarks>
public class InputDeviceManager : SingletonMonobehaviour<InputDeviceManager>
{

    /// <summary> 
    /// Input Action Asset で設定されている スキーム設定
    /// </summary>
    public enum SchemeList : int
    {
        KeyboardAndMouse,
        Gamepad
    };


    /// <summary> 
    /// Input Action Asset で設定されている スキーム名と対応する文字列
    /// </summary>
    public static readonly Dictionary<SchemeList, string> SchemeDic = new Dictionary<SchemeList, string>()
    {
        {SchemeList.KeyboardAndMouse , "Keyboard&Mouse" },
        {SchemeList.Gamepad , "GamePad" }
    };


    /// <summary>
    /// 入力デバイスの種別
    /// </summary>
    public enum InputDeviceType
    {
        Keyboard,   // キーボード・マウス
        Xbox,       // Xboxコントローラー
        DualShock4, // DualShock4(PS4)
        DualSense,  // DualSense(PS5)
        Switch,     // SwitchのProコントローラー
    }

    //直近に操作されたスキーム
    public SchemeList CurrentSchemeType { get; private set; } = SchemeList.KeyboardAndMouse;
    //スキームの文字列
    public string CurrentSchemeName => SchemeDic[CurrentSchemeType];

    // 直近に操作された入力デバイスタイプ
    public InputDeviceType CurrentDeviceType { get; private set; } = InputDeviceType.Keyboard;

    // 各デバイスのすべてのキーを１つにバインドしたInputAction（キー種別検知用）
    private InputAction keyboardAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Keyboard>/AnyKey", interactions: "Press");
    private InputAction mouseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<Mouse>/*", interactions: "Press");
    private InputAction xInputAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<XInputController>/*", interactions: "Press");
    private InputAction dualShock4AnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<DualShockGamepad>/*", interactions: "Press");
    private InputAction detectDualSenseAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<DualSenseGamepadHID>/*", interactions: "Press");
    private InputAction switchProControllerAnyKey = new InputAction(type: InputActionType.PassThrough, binding: "<SwitchProControllerHID>/*", interactions: "Press");

    // 入力デバイスタイプ変更イベント
    public UnityEvent OnChangeDeviceType { get; private set; } = new();

    private void Awake()
    {

        // キー検知用アクションの有効化
        keyboardAnyKey.Enable();
        mouseAnyKey.Enable();
        xInputAnyKey.Enable();
        dualShock4AnyKey.Enable();
        detectDualSenseAnyKey.Enable();
        switchProControllerAnyKey.Enable();
    }


    private void Start()
    {
        // 初回のみ、必ず入力デバイスの種別検知を行ってコールバック発火
        StartCoroutine(InitializeDetection());
    }

    private void Update()
    {
        // 検知の更新処理
        UpdateDeviceTypesDetection();
    }


    /// <summary>
    /// 入力デバイスの種別検知を初期化する
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitializeDetection()
    {
        // 入力デバイスの種別検知を更新
        UpdateDeviceTypesDetection();
        // １フレーム待機
        yield return null;
        // イベント強制発火
        OnChangeDeviceType.Invoke();
    }


    /// <summary>
    /// 入力デバイスの種別検知を更新する
    /// </summary>
    public void UpdateDeviceTypesDetection()
    {
        var beforeDeviceType = CurrentDeviceType;




        if (xInputAnyKey.triggered)
        {
            CurrentDeviceType = InputDeviceType.Xbox;
            CurrentSchemeType = SchemeList.Gamepad;
        }

        // DualSense(PS5)は、DualShock4(PS4)としても認識される。
        // つまり、DualSenseを操作しているときは、DualSchock4とDualSenseの両方が検知される。
        // DualSenseとDualShockの両方から同時に入力検知した場合は、DualSenseとして扱うようにする。
        if (dualShock4AnyKey.triggered)
        {
            CurrentSchemeType = SchemeList.Gamepad;
            CurrentDeviceType = InputDeviceType.DualShock4;
        }
        if (detectDualSenseAnyKey.triggered)
        {
            CurrentDeviceType = InputDeviceType.DualSense;
            CurrentSchemeType = SchemeList.Gamepad;
        }

        if (switchProControllerAnyKey.triggered)
        {
            CurrentDeviceType = InputDeviceType.Switch;
            CurrentSchemeType = SchemeList.Gamepad;
        }


        // Virtual Mouceなら 無視
        if ((keyboardAnyKey.triggered || mouseAnyKey.triggered))
        {
            CurrentDeviceType = InputDeviceType.Keyboard;
            CurrentSchemeType = SchemeList.KeyboardAndMouse;
        }


        // 操作デバイスが切り替わったとき、イベント発火
        if (beforeDeviceType != CurrentDeviceType)
        {
            OnChangeDeviceType.Invoke();
        }
    }
}
