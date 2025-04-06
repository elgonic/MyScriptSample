using UnityEngine;



/// <summary>
/// 数学処理を独自に変更したクラス
/// </summary>
/// <remarks>
/// 数学処理が重かった場合は , sin , cosを軽量化したプログラムに差し替える
/// </remarks>
public static class MyMathf
{
    public static readonly float PI = Mathf.PI;

    public static float Sin(float angle)
    {
        return Mathf.Sin(angle);
    }

    public static float Cos(float angle)
    {
        return Mathf.Cos(angle);
    }

}
