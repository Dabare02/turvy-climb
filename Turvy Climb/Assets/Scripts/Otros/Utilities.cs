using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Utilities : MonoBehaviour
{
    // Devuelve true si el punto se encuentra en el circulo definido por su centro y radio.
    public static bool IsPointInsideCircle(Vector2 circleCenter, float circleRadius, Vector2 linePoint)
    {
        return Vector2.Distance(linePoint, circleCenter) <= circleRadius;
    }

    // Encuentra el punto de intersección entre el borde de una circumferencia y una linea
    // que pasa por el centro de dicha circumferencia.
    public static Vector2 LineThroughCircleCenterIntersec(Vector2 circleCenter, float circleRadius, Vector2 linePoint)
    {
        Vector2 lineDir = linePoint - circleCenter;
        Vector2 normLineDir = lineDir.normalized;
        Vector2 res = circleCenter + normLineDir * circleRadius;

        return res;
    }

    // Devuelve true si el vector tiene valor no válido (NaN).
    public static bool IsVectorInvalid(Vector2 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y);
    }
}
