using System;


/// <summary>
/// シーン名管理クラス
/// </summary>
[Serializable]
public class SceneName
{

    public static string COMMON = "Common_Branch";
    //public static string COMMON = "Common";
    public static string TITTLE = "Tittle";
    public static string END = "END";

    public enum MainGameSceneType
    {
        Stage_BasicTutorial = -1,
        Stage_MoveTutorial = 0,
        Stage_CameraUnLockTutorial = 1,
        Stage_JumpTutorial = 2,
        Stage_DashSlopTutorial = 3,
        Stage_DashEnemyTutorial = 4,
        Stage_BulletTutorial = 5,
        Stage_BounceParryTutorial = 6,
        Stage_LaserWaveTutorial = 11,
        Stage_LaserWave = 7,
        Stage_BounceAction = 9,
        Stage_AllEnemy = 10,
        Stage_RunAWay_Good = 12,
        Stage_RunAWay_Great = 13,
        Stage_RunAWay_Ex = 14,
        Stage_突進_good = 15,
        Stage_突進_bullet_Great = 16,
        Stage_突進_bullet_Ex = 17,
        Stage_Slash_Ex = 18,
        Stage_Slash_Great = 19,
        Stage_Slash_Good = 20,
    }

    public MainGameSceneType SceneType;
    public string GetSceneName()
    {
        return SceneType.ToString();
    }

    public static string GetSceneName(MainGameSceneType type)
    {
        return type.ToString();
    }
}

