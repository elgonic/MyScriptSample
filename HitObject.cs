using UnityEngine;


/// <summary>
/// 衝突したオブジェクト情報
/// </summary>
public class HitObject
{
    private CharacterParam _hitedCharacter;
    private GameObject _hitObject;

    public GameObject Obstacle { get; private set; }
    public AttackObjectBase Attack { get; private set; }


    public bool IsDamegeObject { get; private set; } = true;

    private CharacterParam _characterParam;
    public HitObject(CharacterParam hitedCharacterParam, GameObject hitObject)
    {
        _hitedCharacter = hitedCharacterParam;
        _hitObject = hitObject;

        Attack = hitObject.GetComponent<AttackObjectBase>();
        if (hitObject.CompareTag(StaticCommonParams.OBSTACLE_TAG)) Obstacle = hitObject;


        _characterParam = hitObject.GetComponent<CharacterParam>();
        if (_characterParam == null)
        {
            CharacterHitCollider characterHitCollider = hitObject.GetComponent<CharacterHitCollider>();
            if (characterHitCollider)
            {
                _characterParam = characterHitCollider.CharacterParam;
            }
        }


        //自傷ダメージ防止
        if (_characterParam)
        {
            //自傷ダメージ防止(キャラクターとの接触時)
            if (_hitedCharacter == _characterParam) IsDamegeObject = false;
        }
        //自傷ダメージ(攻撃と接触)
        if (Attack?.AttackData?.Attacker && Attack.AttackData.Attacker == hitedCharacterParam) IsDamegeObject = false;

    }


}
