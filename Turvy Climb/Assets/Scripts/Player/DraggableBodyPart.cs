using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBodyPart : MonoBehaviour
{
    private PlayerMovement player;

    // DEBUG
    public Transform anchorTransform;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnMouseDown()
    {
        player.StartMovingBodyPart(gameObject);
    }

    private void OnMouseUp()
    {
        player.StopMovingBodyPart();
    }
}
