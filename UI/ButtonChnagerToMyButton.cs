using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 子要素のButtonを独自クラスのButtonに置き換える
/// </summary>
public class ButtonChnagerToMyButton : MonoBehaviour
{

    public void ButtonChangeToMyButton()
    {
#if UNITY_EDITOR
        Button[] buttons = gameObject.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            GameObject buttonObject = button.gameObject;
            DestroyImmediate(button);
            buttonObject.AddComponent<MyButton>();
        }
#endif
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ButtonChnagerToMyButton))]//拡張するクラスを指定
public class ExampleScriptEditor : Editor
{

    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();
        ButtonChnagerToMyButton buttonChnagerToMyButton = target as ButtonChnagerToMyButton;
        //ボタンを表示
        if (GUILayout.Button("DoConvert"))
        {
            buttonChnagerToMyButton.ButtonChangeToMyButton();
        }
    }

}

#endif

