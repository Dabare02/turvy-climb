using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBodyPart : MonoBehaviour
{
    private PlayerMovement player;

    private Rigidbody2D _body;

    [SerializeField]
    public float dragSpeed = 50f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        _body = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown()
    {
        player.StartMovingBodyPart(_body);
    }

    private void OnMouseUp()
    {
        player.StopMovingBodyPart();
    }
}
