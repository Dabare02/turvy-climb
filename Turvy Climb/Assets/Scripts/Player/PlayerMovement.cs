using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D draggedPart;
    private bool _partDragging;

    void Update()
    {
        if (_partDragging && draggedPart != null)
        {
            /*
            Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggedPart.transform.position = new Vector3(camPos.x, camPos.y, draggedPart.transform.position.z);
            */

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - draggedPart.transform.position;
            draggedPart.velocity = direction * draggedPart.GetComponent<DraggableBodyPart>().dragSpeed;
        }
    }

    public void StartMovingBodyPart(Rigidbody2D bodyPart)
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
