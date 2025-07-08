using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBodyPart : MonoBehaviour
{
    protected Player _player;

    protected Rigidbody2D _body;

    [SerializeField]
    public float dragSpeed = 10f;

    protected virtual void Awake()
    {
        _player = FindObjectOfType<Player>();
        _body = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnMouseDown()
    {
        _player.StartMovingBodyPart(_body);
    }

    protected virtual void OnMouseUp()
    {
        _player.StopMovingBodyPart();
    }
}
