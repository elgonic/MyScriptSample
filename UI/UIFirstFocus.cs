using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> 
/// コントローラーのための UI 画面で最初にボタンを選択する処理
/// </summary>
public class UIFirstFocus : MonoBehaviour
{
    private GameObject _firstFocus = null;


    private void OnEnable()
    {
        if (!_firstFocus) _firstFocus = gameObject;
    }
    private void OnDisable()
    {

        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void Update()
    {

        if (!EventSystem.current) return;
        //自身が有効かつ選択オブジェクトが無ければ自身を選択状態にする
        if (EventSystem.current?.currentSelectedGameObject) return;
        if (!gameObject.activeSelf) return;
        if (_firstFocus) EventSystem.current.SetSelectedGameObject(_firstFocus);
    }
}
