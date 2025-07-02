using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBodyPart : MonoBehaviour
{
    protected Player _player;

    protected Rigidbody2D _body;

    [SerializeField]
    public float dragSpeed = 10f;

    protected void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _body = GetComponent<Rigidbody2D>();
    }

    protected void OnMouseDown()
    {
        _player.StartMovingBodyPart(_body);
    }

    protected void OnMouseUp()
    {
        _player.StopMovingBodyPart();
    }
}
