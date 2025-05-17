using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private GameObject draggedPart;
    private bool _partDragging;

    void Update()
    {
        if (_partDragging && draggedPart != null)
        {
            Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedPart.transform.position = new Vector3(camPos.x, camPos.y, draggedPart.transform.position.z);
        }
    }

    public void StartMovingBodyPart(GameObject bodyPart)
    {
        draggedPart = bodyPart;
        _partDragging = true;
    }

    public void StopMovingBodyPart()
    {
        _partDragging = false;
        draggedPart = null;
    }
}
