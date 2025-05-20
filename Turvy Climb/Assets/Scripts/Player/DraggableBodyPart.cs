using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBodyPart : MonoBehaviour
{
    private PlayerMovement player;

    protected Rigidbody2D _body;

    [SerializeField]
    public float dragSpeed = 10f;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        _body = GetComponent<Rigidbody2D>();
    }

    protected void OnMouseDown()
    {
        player.StartMovingBodyPart(_body);
    }

    protected void OnMouseUp()
    {
        player.StopMovingBodyPart();
    }
}
