using System.Collections.Generic;
using UnityEngine;

/// <summary>
///　共通パラメーター
/// </summary>
public struct StaticCommonParams
{

    //ユーザーデーターの保存のベースフォルダ名
    public static readonly string UserDataBaseFolderName = "UserData";

    //Scriptable
    //static readonlyは実行時に初めて定まるので ,　属性には使用できないので const使用
    public const string BaseMenuName = "ScriptableObjects/";
    public const string BulletBaseMenuName = BaseMenuName + "BulletParam/";


    // InputSystem
    public static readonly string JUMP_INPUT = "Jump";
    public static readonly string MOVE_INPUT = "Move";
    public static readonly string DASH_INPUT = "Fire";
    public static readonly string LOCKONTOGGLE_INPUT = "LockonToggle";
    public static readonly string ATTACK_INPUT = "BounceBack";
    public static readonly string TOGGLEUI_INPUT = "ToggleUI";
    public static readonly string PAUSE_INPUT = "Pause";

    // Tag
    public static readonly string ATTACK_TAG = "Attack";
    public static readonly string WALL_TAG = "Wall";
    public static readonly string GROUND_TAG = "Ground";
    public static readonly string ENEMY_TAG = "Enemy";
    public static readonly string PLAYER_TAG = "Player";
    public static readonly string OBSTACLE_TAG = "Obstacle";
    public static readonly string UNTAGGED_TAG = "Untagged";
    public static readonly string STARTPOSITION_TAG = "StartPosition";

    //Layer
    public static readonly LayerMask GroundLayer = 1 << 6;
    public static readonly LayerMask WallLayer = 1 << 9;
    public static readonly LayerMask TargetableLayer = 1 << 7;
    public static readonly LayerMask BulletLayer = 1 << 8;
    public static readonly LayerMask Enemy = 1 << 10;
    public static readonly LayerMask Player = 1 << 11;

    //NormalizeAnimaiotn
    public static readonly AnimationCurve NormalizeLiner = AnimationCurve.Linear(0, 0, 1, 1);
    public static readonly AnimationCurve NormalizeConstant = AnimationCurve.Constant(0, 1, 1);
    public static readonly AnimationCurve NormalizeEaseInOut = AnimationCurve.EaseInOut(0, 0, 1, 1);




    /// <summary>
    /// YieldInstruction の キャッシュ
    /// ref = https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/
    /// </summary>
    public static class Yielders
    {
        private static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);
        private static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { return _endOfFrame; }
        }

        private static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { return _fixedUpdate; }
        }

        public static WaitForSeconds Get(float seconds)
        {
            if (!_timeInterval.ContainsKey(seconds))
                _timeInterval.Add(seconds, new WaitForSeconds(seconds));
            return _timeInterval[seconds];
        }
    }
}


