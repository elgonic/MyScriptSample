using System.Collections;
using UnityEngine;



/// <summary>
/// MonoBehaviour非継承クラスでもStartCoroutineが出来るように
/// </summary>
/// <remarks>
/// 参照元 : https://github.com/Unity-Technologies/EndlessRunnerSampleGame/blob/master/Assets/Scripts/CoroutineHandler.cs
/// </remarks>
public class CoroutineHandler : MonoBehaviour
{
    static protected CoroutineHandler m_Instance;
    static public CoroutineHandler instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject o = new GameObject("CoroutineHandler");
                DontDestroyOnLoad(o);
                m_Instance = o.AddComponent<CoroutineHandler>();
            }

            return m_Instance;
        }
    }

    public void OnDisable()
    {
        if (m_Instance)
            Destroy(m_Instance.gameObject);
    }

    static public Coroutine StartStaticCoroutine(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }

    static public void StopStaticCorutine(IEnumerator coroutine)
    {
        instance.StopCoroutine(coroutine);
    }
    static public void StopStaticCorutine(Coroutine coroutine)
    {
        if (coroutine != null) instance.StopCoroutine(coroutine);
    }

    static public void StopAllCorutine()
    {
        instance.StopAllCoroutines();
    }

}
