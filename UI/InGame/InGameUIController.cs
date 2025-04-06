using UnityEngine;

/// <summary>
/// ゲームステージ内のUI管理
/// </summary>
public class InGameUIController : MonoBehaviour
{
    [SerializeField] private HPUI _playerUI;
    [SerializeField] private HPUI _bossUI;
    [SerializeField] private AnnouncementUIManager _announcementUIManager;
    public AnnouncementUIManager AnnouncementUIManager => _announcementUIManager;

    private void Start()
    {
        MainGameSystem.Instance.OnGameStart.AddListener(GameStart);
        MainGameSystem.Instance.OnMainStageEntry.AddListener(BossEntry);

        //Player や　Enemey が GameSystemで設定されているか不明なため
        _playerUI.SetTarget(MainGameSystem.Instance.Player?.Hp);
        _bossUI.SetTarget(MainGameSystem.Instance.Boss?.Hp);
    }

    private void GameStart()
    {
        _playerUI.gameObject.SetActive(true);
        _bossUI.gameObject.SetActive(false);
    }

    private void BossEntry()
    {
        _bossUI.gameObject.SetActive(true);
    }


}
