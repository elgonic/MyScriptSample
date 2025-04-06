using TMPro;
using UnityEngine;
using UnityEngine.UI;

//***************************************************************************
/// <summary> 
/// スライダーと数値入力欄の値を連動させる
/// </summary>
//***************************************************************************
public class SyncSliderWithInputField : MonoBehaviour
{

    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_InputField _inputField;

    private float _value;
    private float _maxValue;
    private float _minValue;

    private bool _isSetUped = false;

    private void Awake()
    {
        SetUp();
    }

    private void SetUp()
    {
        if (_isSetUped) return;
        _isSetUped = true;
        _slider = GetComponentInChildren<Slider>();
        _slider.onValueChanged.AddListener(OnSlider);
        _inputField = GetComponentInChildren<TMP_InputField>();
        _inputField.onDeselect.AddListener(EndEditInputField);
    }

    /// <summary>
    /// スライダーが変更されたときの処理
    /// </summary>
    /// <param name="value"></param>
    public void OnSlider(float value)
    {
        _inputField.text = value.ToString();
        _value = value;
    }

    /// <summary>
    /// 数値入力欄が変更されたときの処理
    /// </summary>
    /// <param name="value"></param>
    public void EndEditInputField(string value)
    {
        float floatValue;
        try
        {
            floatValue = float.Parse(value);
        }
        catch
        {
            Debug.LogWarning("不正な値の入力です\n値を0にします");
            floatValue = _minValue;
            _inputField.text = floatValue.ToString();
        }
        if (floatValue < _minValue || floatValue > _maxValue)
        {
            floatValue = (floatValue < _minValue) ? _minValue : _maxValue;
            _inputField.text = floatValue.ToString();
        }
        _slider.value = floatValue;
        _value = floatValue;
    }

    public float GetValue()
    {
        return _value;
    }

    public void SetValue(float value)
    {
        SetUp();
        if (!_slider || !_inputField) return;
        _value = value;
        _inputField.text = _value.ToString();
        _slider.value = _value;
    }

    public void SetValueRange(float maxValue, float minValue)
    {
        SetUp();
        if (!_slider) return;
        _maxValue = maxValue;
        _minValue = minValue;
        _slider.maxValue = _maxValue;
        _slider.minValue = _minValue;
    }

}
