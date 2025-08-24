using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerAttackHandler : MonoBehaviour
{
    private Player _player;
    
    [SerializeField] private LineRenderer _trajectoryLine;

    private DraggableBodyPart attackPart;
    private SpringJoint2D attackSpringJoint;
    private CircleCollider2D attackPartCollider;
    private PlayerAttackState attackState;
    private bool _isAttackReady;

    private Vector2 _rangeCenterPos;
    private float _rangeRadius;

    private List<SpringJoint2D> _springJoints;

    private Coroutine attackCoroutine;

    void Awake()
    {
        _player = GetComponent<Player>();
        _springJoints = _player.playerTorso.GetComponents<SpringJoint2D>().ToList();
    }

    void Start()
    {
        _trajectoryLine.material.SetColor("_Color", new Color(
            _trajectoryLine.material.color.r,
            _trajectoryLine.material.color.g,
            _trajectoryLine.material.color.r, 0.5f));
        
        HideTrajectoryLine();
    }

    void Update()
    {
        if (attackState == PlayerAttackState.DETECTING_ATTACK)
        {
            DetectAttackReadiness();
        }
    }

    void OnEnable()
    {
        EventManager.PunchSucceeded += PunchInterrupt;
        EventManager.SlingshotStopped += SlingshotInterrupt;
    }

    void OnDisable()
    {
        EventManager.PunchSucceeded -= PunchInterrupt;
        EventManager.SlingshotStopped -= SlingshotInterrupt;
    }

    public void StartAttackDetection(DraggableBodyPart bodyPart)
    {
        attackPart = bodyPart;
        attackPartCollider = attackPart.GetComponent<CircleCollider2D>();

        switch (attackPart)
        {
            case DraggableHand:
                _rangeRadius = _player.punchAttack.rangeForPerformingAttack;

                attackSpringJoint = _springJoints.Find(x => x.connectedBody == attackPart.GetComponent<Rigidbody2D>());
                break;
            case DraggableTorso:
                _rangeCenterPos = bodyPart.transform.position;
                _rangeRadius = _player.slingshotAttack.rangeForPerformingAttack;
                break;
        }

        attackState = PlayerAttackState.DETECTING_ATTACK;
    }

    private void DetectAttackReadiness()
    {
        if (attackPart.GetType() == typeof(DraggableHand))
        {
            _rangeCenterPos = _player.playerTorso.transform.position;
        }

        bool prevReadiness = _isAttackReady;  // DEBUG

        _isAttackReady = !Utilities.IsPointInsideCircle(_rangeCenterPos,
                                                        _rangeRadius,
                                                        attackPart.transform.position);
        _isAttackReady &= (attackPart.GetType() == typeof(DraggableHand) && !((DraggableHand)attackPart).isGripped)
            || (attackPart.GetType() == typeof(DraggableTorso) && _player.grippedHoldsAmount == 4);
            
        if (!prevReadiness && _isAttackReady) Debug.Log("Attack is ready.");   // DEBUG

        if (_isAttackReady)
        {
            ShowTrajectoryLine();
        }
        else
        {
            HideTrajectoryLine();
        }
    }

    public void CheckAttack()
    {
        if (_isAttackReady && attackPart != null)
        {
            // Indicar que se está realizando el ataque.
            attackState = PlayerAttackState.PERFORMING_ATTACK;
            _isAttackReady = false;
            HideTrajectoryLine();

            // Comprobar el tipo de ataque.
            if (attackPart.GetType() == typeof(DraggableHand))
            {
                Punch();
            }
            else if (attackPart.GetType() == typeof(DraggableTorso))
            {   // El jugador debe estar agarrado a la pared con todas sus extremidades.
                Slingshot();
            }
        }
        else
        {
            StopAttackDetection();
        }
    }

    public void Punch()
    {
        if (attackPart != null)
        {
            // Reducir el nivel de aguante (evento).
            EventManager.OnPunchStarted(_player.punchAttack.staminaData.staminaCost);
            
            Rigidbody2D attackBody = attackPart.GetComponent<Rigidbody2D>();
            float launchForce = _player.punchAttack.launchForce;

            //attackBody.gravityScale = 0;
            Vector2 direction = (_rangeCenterPos - (Vector2)attackPart.transform.position).normalized;
            Vector2 force = direction * launchForce;
            attackBody.AddForce(force, ForceMode2D.Impulse);

            attackCoroutine = StartCoroutine(PunchCoroutine());
        }
    }

    private IEnumerator PunchCoroutine()
    {
        PunchHandler punchHandler = attackPart.GetComponent<PunchHandler>();

        punchHandler.attackMode = true;
        attackSpringJoint.enabled = false;
        // Esperar la duración del ataque antes de volver brazo a la normalidad.
        yield return new WaitForSeconds(_player.punchAttack.duration);
        //Debug.Log("Ending punch...");
        attackSpringJoint.enabled = true;

        yield return new WaitForSeconds(_player.punchAttack.extraAttackHitTime);
        //Debug.Log("Punch ended.");
        punchHandler.attackMode = false;

        StopAttackDetection();
    }

    // Función llamada por evento si el puñetazo se interrumpe prematuramente.
    public void PunchInterrupt()
    {
        //Debug.Log("Punch interrupted!");
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackPart.GetComponent<PunchHandler>().attackMode = false;

        attackPartCollider.enabled = true;
        attackSpringJoint.enabled = true;

        StopAttackDetection();
    }

    public void Slingshot()
    {
        // Reducir el nivel de aguante (evento).
        EventManager.OnSlingshotStarted(_player.slingshotAttack.staminaData.staminaCost);

        // Calcular fuerza a aplicar.
        Rigidbody2D attackBody = attackPart.GetComponent<Rigidbody2D>();
        float launchForce = _player.slingshotAttack.launchForce;
        Vector2 direction = (_rangeCenterPos - (Vector2)attackPart.transform.position).normalized;
        Vector2 force = direction * launchForce;

        // Aflojar SpringJoints y apagar gravedad.
        // _player.ChangeSpringJointsDistance(2f);
        _player.ChangeSpringJointsFrequency(0f);
        _player.ActivateGravity(false);
        _player.ActivateEnemyInmunity(true);

        // Realizar ataque.
        _player.DropAllHolds();
        _player.SetLargeHoldDetectRange();
        attackBody.AddForce(force, ForceMode2D.Impulse);

        attackCoroutine = StartCoroutine(SlingshotCoroutine());
    }

    private IEnumerator SlingshotCoroutine()
    {
        SlingshotHandler slingshotHandler = attackPart.GetComponent<SlingshotHandler>();

        slingshotHandler.attackMode = true;
        // Esperar la duración del ataque antes de volver brazo a la normalidad.
        yield return new WaitForSeconds(_player.slingshotAttack.duration);
        //Debug.Log("Slingshot ended.");

        // Resetear SpringJoints y reestablecer gravedad.
        // _player.ChangeSpringJointsDistance();
        _player.ChangeSpringJointsFrequency();
        _player.ActivateGravity(true);

        yield return new WaitForSeconds(_player.slingshotAttack.extraAttackHitTime);
        slingshotHandler.attackMode = false;
        _player.ActivateEnemyInmunity(false);

        StopAttackDetection();
    }

    public void SlingshotInterrupt()
    {
        // COndiciones para no ejecutar función.
        if (attackPart != null)
        {
            SlingshotHandler attackHandler = attackPart.GetComponent<SlingshotHandler>();
            if (attackHandler == null) return;
        }
        else return;

        //Debug.Log("Slingshot interrupted!");
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackPart.GetComponent<SlingshotHandler>().attackMode = false;

        // _player.ChangeSpringJointsDistance();
        _player.ChangeSpringJointsFrequency();
        _player.ActivateGravity(true);
        _player.ActivateEnemyInmunity(false);

        StopAttackDetection();
    }

    public void StopAttackDetection()
    {
        attackCoroutine = null;

        _rangeCenterPos = new Vector2(float.NaN, float.NaN);
        _rangeRadius = 0.0f;
        HideTrajectoryLine();

        attackPart = null;
        attackState = PlayerAttackState.STANDY;
    }

    #region Trajectory Line
    private void ShowTrajectoryLine()
    {
        if (attackPart != null)
        {
            _trajectoryLine.enabled = true;

            Vector2 lineStartPos = attackPart.transform.position;
            Vector2 lineEndPos = _rangeCenterPos;
            Vector2 lineDir = (lineEndPos - lineStartPos).normalized;
            lineEndPos = lineStartPos + lineDir * _player.trajectoryLineLength;

            DefineTrajectoryLine(new Vector3(lineStartPos.x, lineStartPos.y, -1f), new Vector3(lineEndPos.x, lineEndPos.y, -1f));
        }
    }

    private void HideTrajectoryLine()
    {
        _trajectoryLine.enabled = false;
    }

    private void DefineTrajectoryLine(Vector3 startPos, Vector3 endPos)
    {
        // Número de puntos que definen la linea.
        _trajectoryLine.positionCount = 2;
        // Posición de cada punto.
        _trajectoryLine.SetPosition(0, startPos);
        _trajectoryLine.SetPosition(1, endPos);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        if (attackState == PlayerAttackState.DETECTING_ATTACK)
        {
            Gizmos.color = Color.yellow;

            if (_isAttackReady) Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(_rangeCenterPos, _rangeRadius);
        }
    }
}
