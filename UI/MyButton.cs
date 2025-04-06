using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 独自クラスのボタン
/// </summary>
/// <remarks>
/// Unity標準ではOnPointerなどのイベントは実装されていないので
/// </remarks>
public class MyButton : Button
{
    public UnityEvent onChose;


    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        onChose?.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onChose?.Invoke();
    }

}
