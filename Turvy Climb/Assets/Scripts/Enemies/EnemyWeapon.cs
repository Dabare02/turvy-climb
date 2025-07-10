using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWeapon : MonoBehaviour
{
    [Tooltip("Collider tipo trigger que detectará al jugador. Si no se asigna nada, se buscará algún collider en el propio GameObject que sea trigger.")]
    public Collider2D hitDetector;
    protected EnemyAttackTypeSO attackData;
    protected Animator _anim;
    protected Enemy _enemy;

    public UnityEvent<float> playerDamaged;

    private Coroutine useWeaponCoroutine;
    private Coroutine atkCooldownCoroutine;

    protected void Awake()
    {
        if (hitDetector == null)
        {
            hitDetector = GetComponents<Collider2D>().ToList().Find(x => x.isTrigger);
            if (hitDetector == null)
            {
                Debug.LogWarning("Hit detector needs to be trigger. If you assigned one, make sure to turn on that option. If not, make sure you add a trigger collider to the weapon GameObject.");
            }
        }

        GameObject staminaMngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (staminaMngObj != null)
        {
            playerDamaged.AddListener(staminaMngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
        }
    }

    protected void Start()
    {
        ResetWeapon();
    }

    protected void OnDisable()
    {
        playerDamaged.RemoveAllListeners();
    }

    public void SetupWeapon(Enemy enemy, Animator enemyAnimator, EnemyAttackTypeSO atkData)
    {
        _enemy = enemy;
        _anim = enemyAnimator;
        attackData = atkData;
    }

    public void ResetWeapon()
    {
        //hitDetector.enabled = false;
        if (useWeaponCoroutine != null)
        {
            StopCoroutine(useWeaponCoroutine);
            useWeaponCoroutine = null;
        }
        if (atkCooldownCoroutine != null)
        {
            StopCoroutine(atkCooldownCoroutine);
            atkCooldownCoroutine = null;
        }

        _enemy.state = EnemyState.STANDBY;
    }

    // Prepara el arma para atacar.
    public void ReadyWeapon()
    {
        if (_enemy.state == EnemyState.STANDBY)
        {
            _enemy.state = EnemyState.WEAPON_READY;
            //Debug.Log("Enemy " + _enemy.name + "\'s weapon is ready!");

            _anim.SetTrigger("ReadyWeapon");
            //hitDetector.enabled = true;
        }
    }

    public void UnreadyWeapon()
    {
        if (_enemy.state == EnemyState.WEAPON_READY)
        {
            _enemy.state = EnemyState.STANDBY;
            //Debug.Log("Enemy " + _enemy.name + " retreated their weapon.");

            _anim.SetTrigger("UnreadyWeapon");
            //hitDetector.enabled = false;
        }
    }

    // Realiza el ataque (en caso de no tener ataque inmediato).
    public void Attack()
    {
        if (_enemy.state == EnemyState.WEAPON_READY)
        {
            if (useWeaponCoroutine != null) StopCoroutine(useWeaponCoroutine);
            useWeaponCoroutine = StartCoroutine(UseWeaponCoroutine());
        }
    }

    protected IEnumerator UseWeaponCoroutine()
    {
        // Ataque
        _enemy.state = EnemyState.ATTACKING;
        _anim.SetBool("UsingWeapon", true);
        //Debug.Log("Enemy " + _enemy.name + " is attacking!");

        // Esperar a que termine la animacion de ataque.
        AnimationClip weaponUse = _anim.runtimeAnimatorController.animationClips.ToList().Find(x => x.name == "use_weapon");
        yield return new WaitForSeconds(weaponUse.length);

        // Parar ataque e inciar cooldown.
        _anim.SetBool("UsingWeapon", false);
        if (atkCooldownCoroutine != null) StopCoroutine(atkCooldownCoroutine);
        atkCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }

    // Dispara evento para hacer daño al jugador.
    protected void DamagePlayer()
    {
        _enemy.state = EnemyState.PLAYER_DAMAGED;

        playerDamaged.Invoke(attackData.damage);
        Debug.Log("Enemy " + _enemy.name + " attacked Player for " + attackData.damage + " stamina!");

        if (useWeaponCoroutine != null)
        {
            StopCoroutine(useWeaponCoroutine);
            useWeaponCoroutine = null;
        }

        _anim.SetBool("UsingWeapon", false);
        if (atkCooldownCoroutine != null) StopCoroutine(atkCooldownCoroutine);
        atkCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }

    // Coruitna para cooldown
    protected IEnumerator AttackCooldownCoroutine()
    {
        _enemy.state = EnemyState.COOLDOWN;

        //hitDetector.enabled = false;
        yield return new WaitForSeconds(attackData.cooldown);

        _enemy.state = EnemyState.STANDBY;
        //Debug.Log("Enemy " + _enemy.name + "\'s attack cooldown is over.");
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Enemy " + name + " detected " + other.name);
        if (!other.isTrigger && other.CompareTag("Player"))
        {
            // TEMP: En el futuro, hacer que slingshot handler y punch handler hereden de una clase que tenga este parametro attackMode.
            PunchHandler punchHandler = other.GetComponent<PunchHandler>();
            if ((_enemy.state == EnemyState.ATTACKING || _enemy.state == EnemyState.WEAPON_READY)
                && !punchHandler.attackMode)
            {
                DamagePlayer();
            }
        }
    }
}
