using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerAttackHandler : MonoBehaviour
{
    private Player _player;

    private DraggableBodyPart attackPart;
    private SpringJoint2D attackSpringJoint;
    private CircleCollider2D attackPartCollider;
    private bool _attackDetection;
    private bool _isAttackReady;

    private Vector2 _rangeCenterPos;
    private float _rangeRadius;

    private List<SpringJoint2D> _springJoints;

    void Awake()
    {
        _player = GetComponent<Player>();

        _springJoints = _player.playerTorso.GetComponents<SpringJoint2D>().ToList();
    }

    void Update()
    {
        DetectAttackReadiness();
    }

    public void StartAttackDetection(DraggableBodyPart bodyPart)
    {
        // TODO: Añadir soporte para slingshot.

        attackPart = bodyPart;
        attackPartCollider = attackPart.GetComponent<CircleCollider2D>();

        switch (attackPart)
        {
            case DraggableHand:
                _rangeRadius = _player.punchAttack.rangeForPerformingAttack;

                attackSpringJoint = _springJoints.Find(x => x.connectedBody == attackPart.GetComponent<Rigidbody2D>());
                break;
            /*
            case DraggableTorso:
                _rangeCenterPos = bodyPart.transform.position;
                _rangeRadius = _player.slingshotAttack.rangeForPerformingAttack;
                break;
            */
        }

        _attackDetection = true;
    }

    private void DetectAttackReadiness()
    {
        if (attackPart == null) return;
        
        if (attackPart.GetType() == typeof(DraggableHand))
        {
            _rangeCenterPos = _player.playerTorso.transform.position;
        }

        bool prevReadiness = _isAttackReady;  // DEBUG
        _isAttackReady = !Utilities.IsPointInsideCircle(_rangeCenterPos,
                                                        _rangeRadius,
                                                        attackPart.transform.position);
        if (!prevReadiness && _isAttackReady) Debug.Log("Attack is ready.");   // DEBUG
    }

    public void CheckAttack()
    {
        if (_isAttackReady)
        {
            if (attackPart.GetType() == typeof(DraggableHand)
                && !((DraggableHand)attackPart).isGripped)
            {
                Attack();
            }
        }
        else
        {
            StopAttackDetection();
        }
    }

    public void Attack()
    {
        // TODO: Añadir soporte para movimiento tirachinas.

        Rigidbody2D attackBody = attackPart.GetComponent<Rigidbody2D>();
        float launchForce = _player.punchAttack.launchForce;

        //attackBody.gravityScale = 0;
        Vector2 direction = _rangeCenterPos - (Vector2)attackPart.transform.position;
        Vector2 force = direction * launchForce;
        attackBody.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        // TODO: Añadir soporte para movimiento tirachinas.

        PunchHandler punchHandler = attackPart.GetComponent<PunchHandler>();

        punchHandler.attackMode = true;
        attackSpringJoint.enabled = false;
        attackPartCollider.enabled = false;
        yield return new WaitForSeconds(_player.punchAttack.attackDuration);
        attackPartCollider.enabled = true;
        attackSpringJoint.enabled = true;

        yield return new WaitForSeconds(_player.punchAttack.extraAttackHitTime);
        punchHandler.attackMode = false;

        StopAttackDetection();
    }

    public void StopAttackDetection()
    {
        _attackDetection = false;

        _rangeCenterPos = new Vector2(float.NaN, float.NaN);
        _rangeRadius = 0.0f;

        attackPart = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackDetection)
        {
            Gizmos.color = Color.yellow;

            if (_isAttackReady) Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(_rangeCenterPos, _rangeRadius);
        }
    }
}
