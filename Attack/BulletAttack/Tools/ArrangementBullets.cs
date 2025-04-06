using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾配置用処理
/// </summary>
public class ArrangementBullets<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// 円状配置
    /// </summary>
    /// <param name="angularSpacing"></param>
    /// <param name="baseList">再利用したければ引数指定する.足りなければ生成する</param>
    /// <returns></returns>
    public static List<T> CircularArrangement(float raduis, float angularSpacing, T arrangementElement, Transform parentTransform, List<T> baseList = null)
    {
        //イミュータブルを基本にしたい
        List<T> returnList = new List<T>(baseList);

        returnList = MissingRemove(returnList);

        if (360f % angularSpacing != 0) Debugger.LogWarning($"{System.Reflection.MethodBase.GetCurrentMethod().Name} , {nameof(angularSpacing)} を 360 であまり 0 にしないと等間隔にならないよ!");
        int arrangementCount = (int)(360f / angularSpacing);

        Vector3 arrangeLocalMentPosition = ((parentTransform.forward) * raduis);


        for (int i = 0; i < arrangementCount; i++)
        {
            arrangeLocalMentPosition = Quaternion.AngleAxis(angularSpacing, Vector3.up) * arrangeLocalMentPosition;

#if UNITY_EDITOR
            T element;
            if (i < returnList.Count)
            {
                element = returnList[i];
            }
            else
            {
                element = UnityEditor.PrefabUtility.InstantiatePrefab(arrangementElement) as T;
                returnList.Add(element);
            }
            element.transform.position = arrangeLocalMentPosition + parentTransform.position;
            element.transform.parent = parentTransform;
            element.gameObject.SetActive(true);
#else
            T element = Instantiate(arrangementElement, arrangeLocalMentPosition + parentTransform.position, Quaternion.identity, parentTransform);
#endif

            element.transform.rotation = Quaternion.LookRotation(element.transform.position - parentTransform.position, parentTransform.up);

        }
        return returnList;
    }

    private static List<T> MissingRemove(List<T> baseLise)
    {
        var returnList = new List<T>(baseLise);
        List<T> missingtList = new List<T>();
        foreach (var element in returnList)
        {
            if (!element) missingtList.Add(element);
        }
        foreach (var missing in missingtList)
        {
            returnList.Remove(missing);
        }

        return returnList;

    }

}



