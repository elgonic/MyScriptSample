using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 操作コントローラーに対応して操作画像変更する　
/// </summary>
public class ChangeContorollerIcon : MonoBehaviour
{
    [Serializable]
    public class DeviceImage
    {
        [SerializeField] private InputDeviceManager.InputDeviceType _deviceType;
        [SerializeField] private GameObject _image;

        public InputDeviceManager.InputDeviceType DeviceType => _deviceType;
        public GameObject Image => _image;
    }

    [SerializeField] private List<DeviceImage> _deviceImages = new List<DeviceImage>();


    private void Awake()
    {
        if (!_deviceImages.Exists(element => element.DeviceType == InputDeviceManager.InputDeviceType.Keyboard)) Debugger.Log($"{GetType().Name}:{gameObject.name}:キーボードマウス操作の画像は必須です");
    }
    private void OnEnable()
    {
        ChangeImage();
        InputDeviceManager.Instance.OnChangeDeviceType.AddListener(ChangeImage);
    }

    /// <summary>
    /// ゲーム終了時の判断フラグ
    /// </summary>
    private bool _isaplicationQuit = false;
    private void OnDisable()
    {
        //SingletonにOnDestroy, OnDisableでアクセスするとゲーム終了時に "Some objects were not cleaned up when closing the scene." と出てエラーが出るのでその対処 
        if (_isaplicationQuit == false) InputDeviceManager.Instance.OnChangeDeviceType.RemoveListener(ChangeImage);

    }

    private void OnApplicationQuit()
    {
        _isaplicationQuit = true;
    }

    private void ChangeImage()
    {
        InputDeviceManager.InputDeviceType deviceType = InputDeviceManager.Instance.CurrentDeviceType;
        if (_deviceImages.Count == 1) return;

        //操作デバイスに対応する画像が無ければ キーボードマウス画像にする
        if (!_deviceImages.Exists(element => element.DeviceType == deviceType)) deviceType = InputDeviceManager.InputDeviceType.Keyboard;
        foreach (var image in _deviceImages)
        {
            if (image.DeviceType == deviceType) image.Image.SetActive(true);
            else image.Image.SetActive(false);
        }

    }

}
