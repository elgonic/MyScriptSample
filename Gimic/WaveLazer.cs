using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 波打つレーザーを管理する
/// </summary>
public class WaveLazer : MonoBehaviour
{
    [SerializeField] private List<FollOutLaserEllipticalOrbit> _lasers;
    [SerializeField] private bool _setWaveSpeedEqualPlayerMoveSpeed;
    [SerializeField] private float _fireDelayInterval = 0;
    [SerializeField] private float _positionInterval = 0;
    private float totalDelay = 0;
    private float totalInterval = 0;
    private void OnValidate()
    {
        totalDelay = 0;
        totalInterval = 0;
        _lasers.Clear();

        if (_setWaveSpeedEqualPlayerMoveSpeed)
        {
            _setWaveSpeedEqualPlayerMoveSpeed = false;
            _fireDelayInterval = _positionInterval / MainGameSystem.Instance.Player.GetComponent<Player>().MoveSpeed;
        }

        foreach (FollOutLaserEllipticalOrbit laser in GetComponentsInChildren<FollOutLaserEllipticalOrbit>())
        {
            _lasers.Add(laser);
            laser.transform.localPosition = new Vector3(0, laser.transform.localPosition.y, totalInterval);
            laser.FireDelay = totalDelay;
            totalDelay += _fireDelayInterval;
            totalInterval += _positionInterval;
        }

    }
}
