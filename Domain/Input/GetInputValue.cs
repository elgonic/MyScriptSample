using UnityEngine;
using UnityEngine.InputSystem;



/// <summary>
/// デバイスからの入力情報の取得
/// </summary>

public class GetInputValue
{
    private PlayerInput _playerInput;
    private float _inputThreshold;
    private float _inputThreshold_SqrMag;


    public GetInputValue(PlayerInput playerInput, float inputThreshold)
    {
        _playerInput = playerInput;
        _inputThreshold = inputThreshold;

        _inputThreshold_SqrMag = _inputThreshold * _inputThreshold;

    }
    public Vector2 GetInputMoveValueNomalize()
    {
        return GetInputMoveValueNomalize8Snap();
    }

    /// <summary>
    /// 入力の8方向スナップ
    /// </summary>
    /// <returns></returns>
    private Vector2 GetInputMoveValueNomalize8Snap()
    {
        //シーンのリロード時はまだ設定されていないので

        Vector2? readValue = _playerInput?.actions[StaticCommonParams.MOVE_INPUT].ReadValue<Vector2>();

        if (!readValue.HasValue) return Vector2.zero;

        if (_inputThreshold_SqrMag > readValue.Value.sqrMagnitude) return Vector2.zero;

        Vector2 nomalize = readValue.Value.normalized;
        float angle = Vector2.Angle(Vector2.up, nomalize);
        float xSignal = 1;

        if (nomalize.x < 0)
        {
            xSignal = -1;
        }
        if (angle <= 22.5f)
        {
            return new Vector2(0, 1);
        }
        if (angle <= 67.5f)
        {
            //大体√0.5
            return new Vector2(xSignal * 0.707f, 0.707f);
        }
        if (angle <= 112.5f)
        {

            return new Vector2(xSignal * 1, 0);
        }
        if (angle <= 157.5f)
        {
            return new Vector2(xSignal * 0.707f, -0.707f);
        }
        return new Vector2(0, -1);

    }



}
