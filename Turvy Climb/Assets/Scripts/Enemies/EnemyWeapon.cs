using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWeapon : MonoBehaviour
{
    public Collider2D hitDetector;
    protected AttackTypeSO attackData;
    protected Animator _anim;
    protected Enemy _enemy;

    public UnityEvent<float> playerDamaged;

    private Coroutine useWeaponCoroutine;
    private Coroutine atkCooldownCoroutine;

    void Awake()
    {
        GameObject staminaMngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (staminaMngObj != null)
        {
            playerDamaged.AddListener(staminaMngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
        }
    }

    void Start()
    {
        ResetWeapon();
    }

    void OnDisable()
    {
        playerDamaged.RemoveAllListeners();
    }

    public void SetupWeapon(Enemy enemy, Animator enemyAnimator, AttackTypeSO atkData)
    {
        _enemy = enemy;
        _anim = enemyAnimator;
        attackData = atkData;
    }

    public void ResetWeapon()
    {
        hitDetector.enabled = false;
        if (useWeaponCoroutine != null) StopCoroutine(useWeaponCoroutine);
        if (atkCooldownCoroutine != null) StopCoroutine(atkCooldownCoroutine);
    }

    // Prepara el arma para atacar.
    public void ReadyWeapon()
    {
        if (_enemy.state == EnemyState.STANDBY)
        {
            _enemy.state = EnemyState.WEAPON_READY;
            Debug.Log("Enemy " + _enemy.name + "\'s weapon is ready!");

            //_anim.SetTrigger("ReadyWeapon");
            hitDetector.enabled = true;
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
        _enemy.state = EnemyState.ATTACKING;
        _anim.SetBool("UsingWeapon", true);
        Debug.Log("Enemy " + _enemy.name + " is attacking!");

        AnimationClip weaponUse = _anim.runtimeAnimatorController.animationClips.ToList().Find(x => x.name == "UseWeapon");
        yield return new WaitForSeconds(weaponUse.length + attackData.finishDuration);

        _anim.SetBool("UsingWeapon", false);
        hitDetector.enabled = false;
        _enemy.state = EnemyState.STANDBY;
        Debug.Log("Enemy " + _enemy.name + " hasn't hit anything...");
    }

    // Dispara evento para hacer daño al jugador.
    protected void DamagePlayer()
    {
        _enemy.state = EnemyState.PLAYER_DAMAGED;
        Debug.Log("Enemy " + _enemy.name + " damaged Player for " + attackData.damage + " stamina!");

        playerDamaged.Invoke(attackData.damage);

        if (useWeaponCoroutine != null) StopCoroutine(useWeaponCoroutine);
        hitDetector.enabled = false;

        if (atkCooldownCoroutine != null) StopCoroutine(atkCooldownCoroutine);
        StartCoroutine(AttackCooldownCoroutine());
    }

    protected IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(attackData.cooldown);

        _anim.SetTrigger("BackToStandby");
        yield return new WaitUntil(() => _anim.IsInTransition(0));
        _enemy.state = EnemyState.STANDBY;
        Debug.Log("Enemy " + _enemy.name + "\'s attack cooldown is over.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something detected.");
        if (other.CompareTag("Player"))
        {
            if (_enemy.state == EnemyState.ATTACKING || _enemy.state == EnemyState.WEAPON_READY)
            {
                DamagePlayer();
            }
        }
    }
}
