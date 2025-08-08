using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Contiene la altura y anchura de la cámara en unidades de Unity.
    private Vector3 _screenBounds;

    void Start()
    {
        float h = Camera.main.orthographicSize * 2f;
        float w = h * Camera.main.aspect;
        _screenBounds = new Vector3(w, h, Camera.main.transform.position.z);
    }

    void LateUpdate()
    {
        UpdatePos();
    }

    private void UpdatePos()
    {
        Vector3 newPos = transform.position;
        newPos.y = Mathf.Clamp(newPos.y, Camera.main.transform.position.y - (_screenBounds.y / 2f), Camera.main.transform.position.y + (_screenBounds.y / 2));
        transform.position = newPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<DraggableTorso>() != null)
        {
            FindObjectOfType<LevelManager>().GameOver();
        }
    }
}
