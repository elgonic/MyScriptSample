using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// お知らせUI
/// </summary>
public class AnnouncementUI : MonoBehaviour
{

    [SerializeField] private AnnouncementUIManager.AnnouncementType _type;
    public AnnouncementUIManager.AnnouncementType Type => _type;
    [SerializeField] private List<GameObject> _list;


    private bool _isSetuped = false;
    private int _count = 0;


    private void OnDestroy()
    {
        Reset();
    }


    /// <summary>
    /// 次の要素表示
    /// </summary>
    /// <returns>リストが末尾 or 空 : false</returns>
    public bool Next()
    {
        if (!_isSetuped)
        {
            Reset();
        }
        if (_list.Count == 0)
        {
            Debugger.LogError($"{gameObject.name} : {GetType().Name} : 要素Listが空です");
            return false;
        }

        if (_count < _list.Count)
        {
            if (_count > 0) _list[_count - 1].SetActive(false);
            _list[_count].SetActive(true);
            _count++;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 前の要素表示
    /// </summary>
    /// <returns>リストが末尾 or 空 : false</returns>

    public bool Back()
    {
        if (!_isSetuped)
        {
            Reset();
        }
        if (_list.Count == 0)
        {
            Debugger.LogError($"{gameObject.name} : {GetType().Name} : 要素Listが空です");
            return false;
        }

        if (_count < 1)
        {
            return false;
        }
        else
        {
            _count--;
            _list[_count].SetActive(true);
            return true;
        }
    }

    private void Reset()
    {
        _isSetuped = true;
        _count = 0;
        foreach (var item in _list)
        {
            item.gameObject.SetActive(false);
        }

    }
}
