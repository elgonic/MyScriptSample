using UnityEngine;



/// <summary>
/// 壁のシェーダーにターゲットを設定する
/// </summary>
public class WallShader : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    private MeshRenderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    private static readonly int _targetPositionPropatyID = Shader.PropertyToID("_targetPosition");

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        if (!_target)
        {
            Debugger.Log($"{GetType().Name}のTargetObjectが設定されていないのでScene内の{typeof(Player).Name}を自動で設定します");
            _target = MainGameSystem.Instance.Player.gameObject;
        }
    }

    private void Update()
    {
        _materialPropertyBlock.SetVector(_targetPositionPropatyID, _target.transform.position);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
