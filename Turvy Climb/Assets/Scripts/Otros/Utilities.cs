using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Utilities
{
    #region Geometry
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
    #endregion

    #region Vectors
    public static Vector2 Vector2ToUnity(System.Numerics.Vector2 v) => new Vector2(v.X, v.Y);
    public static System.Numerics.Vector2 Vector2ToNumerics(Vector2 v) => new System.Numerics.Vector2(v.x, v.y);

    /// <summary>
    /// Convierte un array de System.Numeric.Vector2 a UnityEngine.Vector2.
    /// </summary>
    /// <returns></returns>
    public static Vector2[] Vector2ArrayToUnity(System.Numerics.Vector2[] v)
    {
        Vector2[] res = new Vector2[v.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = Vector2ToUnity(v[i]);
        }

        return res;
    }
    /// <summary>
    /// Convierte un array de UnityEngine.Vector2 a System.Numeric.Vector2.
    /// </summary>
    /// <returns></returns>
    public static System.Numerics.Vector2[] Vector2ArrayToNumerics(Vector2[] v)
    {
        System.Numerics.Vector2[] res = new System.Numerics.Vector2[v.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = Vector2ToNumerics(v[i]);
        }

        return res;
    }

    // Devuelve true si el vector tiene valor no válido (NaN).
    public static bool IsVectorInvalid(Vector2 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y);
    }

    /// <summary>
    /// Devuelve el punto más cercano al punto objetivo.
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="transforms"></param>
    /// <returns></returns>
    public static Transform ClosestTransformToTarget(Transform targetTransform, List<Transform> transforms)
    {
        Transform closestTransform = transforms[0];
        for (int i = 1; i < transforms.Count; i++)
        {
            if (Vector2.Distance(targetTransform.position, closestTransform.position)
                > Vector2.Distance(targetTransform.position, transforms[i].position))
            {
                closestTransform = transforms[i];
            }
        }

        return closestTransform;
    }
    #endregion

    #region Other
    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public static Tuple<int, int> ConvertToMinutes(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds - m * 60);

        return new Tuple<int, int>(m, s);
    }

    public static string ConvertToMinutesTimerFormat(float seconds)
    {
        Tuple<int, int> tuple = ConvertToMinutes(seconds);
        string timerS = string.Format("{0:00}:{1:00}", tuple.Item1, tuple.Item2);

        return timerS;
    }
    #endregion
}
