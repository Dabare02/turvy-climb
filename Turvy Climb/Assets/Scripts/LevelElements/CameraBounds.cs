using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class CameraBounds : MonoBehaviour
{
    // Contiene la altura y anchura de la cámara en unidades de Unity.
    private Vector2 _screenBounds;

    private PolygonCollider2D boundsCollider;

    [SerializeField] private float bottomOffset = 2f;

    void Awake()
    {
        boundsCollider = GetComponent<PolygonCollider2D>();
    }

    void Start()
    {
        float h = Camera.main.orthographicSize * 2f;
        float w = h * Camera.main.aspect;
        _screenBounds = new Vector2(w, h);
    }

    void Update()
    {
        // Calcular posición y de el lado inferior de la cámara en el sistema de coordenadas de Unity.
        float cameraBottomY = Camera.main.transform.position.y - (_screenBounds.y / 2f);

        // Comprobar si la cámara ha subido.
        if (boundsCollider.points[0].y < cameraBottomY - bottomOffset)
        {
            // Subir parte inferior del collider como corresponde.
            Vector2[] path = boundsCollider.GetPath(0);
            path[0] = new Vector2(path[0].x, cameraBottomY - bottomOffset);
            path[3] = new Vector2(path[3].x, cameraBottomY - bottomOffset);
            boundsCollider.SetPath(0, path);

            // Necesario para tener en cuenta el nuevo path definido
            // CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
            FindObjectOfType<CinemachineConfiner2D>().InvalidateCache();
        }
    }
}
