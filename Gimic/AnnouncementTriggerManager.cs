using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// お知らせUIの送り処理
/// </summary>
public class AnnouncementTriggerManager : MonoBehaviour
{
    [Serializable]
    private class AnnouncementTriggerObject
    {
        /// <summary>
        /// インスペクター確認用
        /// </summary>
        [SerializeField] private string name;

        public ColliderDelegate ColliderDelegate;
        public bool IsDone = false;
        public AnnouncementTriggerObject(ColliderDelegate colliderDelegate)
        {
            name = colliderDelegate.name;
            ColliderDelegate = colliderDelegate;
        }
    }


    [SerializeField] private AnnouncementUIManager.AnnouncementType _type;
    [ReadOnly, SerializeField] private List<AnnouncementTriggerObject> _announcementTriggerObjects = new List<AnnouncementTriggerObject>();
    public UnityEvent OnEnterPlayer;
    public AnnouncementUIManager.AnnouncementType Type => _type;

    private void Awake()
    {
        foreach (Transform chiled in transform)
        {
            var collider = chiled.GetComponent<ColliderDelegate>();
            if (collider) _announcementTriggerObjects.Add(new AnnouncementTriggerObject(collider));
        }

        foreach (var element in _announcementTriggerObjects)
        {
            element.ColliderDelegate.OnTriggerEnterAction.AddListener(Action);
        }
    }


    public void Action(Collider collider, Transform tr)
    {
        if (!collider.gameObject.CompareTag(StaticCommonParams.PLAYER_TAG)) return;

        ColliderDelegate colliderDelegate = tr.GetComponent<ColliderDelegate>();
        AnnouncementTriggerObject announcementTriggerObject = _announcementTriggerObjects.Find(element => element.ColliderDelegate == colliderDelegate);

        if (announcementTriggerObject.IsDone) return;

        announcementTriggerObject.IsDone = true;
        OnEnterPlayer?.Invoke();


    }
}
