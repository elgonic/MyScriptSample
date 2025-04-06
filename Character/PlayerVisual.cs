using UnityEngine;

/// <summary>
/// Playerの見え方制御クラス
/// </summary>
public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Material _parryMaterial;
    [SerializeField] private Renderer _meshRenderer;
    private Material _defaultMaterial;


    private void Start()
    {
        _defaultMaterial = _meshRenderer.material;

    }

    private void Update()
    {
        if (_player.CanParryFlag)
        {
            OnEnableParryStatus();
        }
        else
        {
            ResetMaterial();
        }
    }



    private void OnEnableParryStatus()
    {
        if (_meshRenderer.material != _parryMaterial) _meshRenderer.material = _parryMaterial;
    }

    private void ResetMaterial()
    {
        if (_meshRenderer.material != _defaultMaterial) _meshRenderer.material = _defaultMaterial;
    }

}
