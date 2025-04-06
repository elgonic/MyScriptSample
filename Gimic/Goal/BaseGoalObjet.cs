using UnityEngine;


/// <summary>
/// ゴールのオブジェクトの基底クラス
/// </summary>
public abstract class BaseGoalObjet : MonoBehaviour
{
    public abstract Coroutine Enable();
    public abstract Coroutine Disable();
}
