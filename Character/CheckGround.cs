using UnityEngine;



/// <summary>
/// 地面接触確認クラス
/// </summary>
public static class CheckGround
{
    public static bool LineCheck(Vector3 startPosition, float groundRayLength, LayerMask groundLayer, Rigidbody rb = null, float coyoteRadius = 0, bool isDebugMode = false)
    {
        bool groundCheck = false;
        //Ryacastでは開始位置のコライダは判定されないので代わりに

        if (Physics.OverlapSphere(startPosition, 0.01f, groundLayer).Length > 0)
        {
            return true;
        }

        groundCheck = Physics.Linecast(startPosition, startPosition + Vector3.down * groundRayLength, groundLayer);



        if (isDebugMode) PrintRayAndPosition(startPosition, startPosition + Vector3.down * groundRayLength, Color.yellow, "センター");
        if (groundCheck)
        {
            return true;
        }
        if (!rb) return false;
        //Coyote
        Vector3 directionOfTravelPlane = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

        //Ryacastでは開始位置のコライダは判定されないので代わりに
        if (Physics.OverlapSphere(startPosition, 0.01f, groundLayer).Length > 0)
        {
            return true;
        }

        groundCheck = Physics.Linecast(startPosition - directionOfTravelPlane * coyoteRadius, startPosition - directionOfTravelPlane * coyoteRadius + Vector3.down * groundRayLength, groundLayer);
        if (groundCheck)
        {
            if (isDebugMode) PrintRayAndPosition(startPosition - directionOfTravelPlane * coyoteRadius, startPosition - directionOfTravelPlane * coyoteRadius + Vector3.down * groundRayLength, Color.red, "コヨーテ");
            return true;
        }
        return false;
    }

    public static bool SphereCheck(Vector3 startPosition, float groundRayLength, float radius, LayerMask groundLayer, Rigidbody rb = null, float coyoteRadius = 0, bool isDebugMode = false)
    {
        bool groundCheck = false;


        //Ryacastでは開始位置のコライダは判定されないので代わりに

        if (Physics.OverlapSphere(startPosition, radius, groundLayer).Length > 0)
        {
            return true;
        }

        groundCheck = Physics.SphereCast(startPosition, radius, Vector3.down, out RaycastHit hit, groundRayLength, groundLayer);
        if (isDebugMode) PrintRayAndPosition(startPosition, startPosition + Vector3.down * groundRayLength, Color.yellow, "センター");
        if (groundCheck)
        {
            return true;
        }
        if (!rb) return false;
        //Coyote
        Vector3 directionOfTravelPlane = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;

        //Ryacastでは開始位置のコライダは判定されないので代わりに

        if (Physics.OverlapSphere(startPosition, radius, groundLayer).Length > 0)
        {
            return true;
        }

        groundCheck = Physics.SphereCast(startPosition - directionOfTravelPlane * coyoteRadius, radius, Vector3.down, out RaycastHit hit2, groundRayLength, groundLayer);

        if (groundCheck)
        {
            if (isDebugMode) PrintRayAndPosition(startPosition - directionOfTravelPlane * coyoteRadius, startPosition - directionOfTravelPlane * coyoteRadius + Vector3.down * groundRayLength, Color.red, "コヨーテ");
            return true;
        }
        return false;
    }


    private static void PrintRayAndPosition(Vector3 rayStartPoint, Vector3 rayFinishPosint, Color pointColor, string name = "")
    {
#if UNITY_EDITOR
        GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        LineRenderer line = new GameObject().AddComponent<LineRenderer>();
        point.GetComponent<Renderer>().material.color = pointColor;
        point.GetComponent<Collider>().enabled = false;
        if (string.IsNullOrEmpty(name))
        {

            point.name = "point";
            line.name = "line";
        }
        else
        {
            point.name = name + "_point";
            line.name = name + "_line";
        }
        point.transform.position = rayStartPoint;
        point.transform.localScale = Vector3.one * 0.3f;
        line.positionCount = 2;
        line.SetWidth(0.03f, 0.03f);
        line.SetPosition(0, rayStartPoint);
        line.SetPosition(1, rayFinishPosint);
#endif 
    }
}
