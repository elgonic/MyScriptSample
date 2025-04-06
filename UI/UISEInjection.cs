using UnityEngine;


/// <summary>
/// UIにSEを設定する
/// </summary>
public class UISEInjection : MonoBehaviour
{

    private void Start()
    {
        ButtonSE(this.transform);
    }


    public static void ButtonSE(Transform UIObject)
    {
        MyButton[] buttons = UIObject.GetComponentsInChildren<MyButton>(true);
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => PlaySE(AudioManager.AudioType.UIEnter));
            button.onChose.AddListener(() => PlaySE(AudioManager.AudioType.UIChose));
        }

        void PlaySE(AudioManager.AudioType SEType)
        {
            AudioManager.Instance.CommonPooledSEPlay(SEType);
        }
    }
}
