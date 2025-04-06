using TMPro;
using UnityEngine;

/// <summary>
/// ゲームクリア時のUI
/// </summary>
public class EndUI : MonoBehaviour
{
    [SerializeField] private MyButton _backTittleButton;
    [SerializeField] private TextMeshProUGUI _routeResult;

    public MyButton BackTittleButton => _backTittleButton;
    public TextMeshProUGUI RouteInfo => _routeResult;

    private bool _isSetRouteResult = false;
    private void Start()
    {
        _backTittleButton.onClick.AddListener(EndSystem.Instance.BackTittle);
        _routeResult.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_isSetRouteResult && EndSystem.Instance.SceneChanger)
        {
            BranchSceneChanger branchSceneChanger = EndSystem.Instance.SceneChanger as BranchSceneChanger;
            if (branchSceneChanger)
            {
                _routeResult.gameObject.SetActive(true);
                _routeResult.text = $"Route : {branchSceneChanger.ResultType.ToString()}";
            }
            _isSetRouteResult = true;
        }
    }
}
