using UnityEngine;


/// <summary>
/// 弾の見え方制御クラス
/// </summary>
public class BulletVisualize : MonoBehaviour
{
    [SerializeField] private Material _parryedMaterial;



    private BulletBase _bulletBase;

    private MeshRenderer _m_Renderer;


    private Material _defaultMaterial;


    private Color _defaultColor;
    private Color _defaultColorEmi;
    private void Start()
    {
        _bulletBase = GetComponent<BulletBase>();
        _bulletBase.OnParryed.AddListener(OnParryed);

        _m_Renderer = _bulletBase.GetComponentInChildren<MeshRenderer>();

        _defaultMaterial = _m_Renderer.material;


    }

    private void OnEnable()
    {
        if (_defaultMaterial) ResetMaterial();
    }

    private void Update()
    {
    }

    private void OnParryed(AttackObjectBase attackData = null)
    {
        SetMaterial(_parryedMaterial);
    }




    public void SetMaterial(Material material)
    {
        _m_Renderer.material = material;
    }

    public void ResetMaterial()
    {
        SetMaterial(_defaultMaterial);
    }

}
