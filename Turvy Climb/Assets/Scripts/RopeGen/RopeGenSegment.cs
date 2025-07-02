using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenSegment : MonoBehaviour
{
    public GameObject connectedAbove, connectedBelow;

    void Start()
    {
        connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        RopeGenSegment aboveSegment = connectedAbove.GetComponent<RopeGenSegment>();

        if (aboveSegment != null) { // Rope segment está en medio o al final de la cuerda.
            // Indicar segmento inferior del segmento superior.
            aboveSegment.connectedBelow = gameObject;

            float spriteBottom = -connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
            // Mover segmento a posición correcta.
            transform.position = new Vector3(
                connectedAbove.transform.position.x,
                connectedAbove.transform.position.y + spriteBottom,
                connectedAbove.transform.position.z);
            // Indicar donde se une al segmento superior.
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom);
        } else {    // Rope segment es el segmento superior.
            // Mover segmento a posición correcta.
            transform.position = connectedAbove.transform.position;
            // Indicar donde se une al segmento superior.
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
        }

        //GetComponent<BoxCollider2D>().enabled = true;
    }
}
