using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyWeapon : MonoBehaviour
{
    public AttackTypeSO attackData;
    public Animator parentAnim
    {
        set
        {
            if (_anim != null)
            {
                _anim = value;
            }
        }
    }
    private Animator _anim;

    private bool _attacking;
    private bool _playerDamaged;

    public UnityEvent<float> playerDamaged;

    void Awake()
    {
        GameObject staminaMngObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (staminaMngObj != null)
        {
            playerDamaged.AddListener(staminaMngObj.GetComponent<StaminaManager>().DecreaseCurrentStamina);
        }
    }
    
    void OnDisable()
    {
        playerDamaged.RemoveAllListeners();
    }

    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    protected IEnumerator AttackCoroutine()
    {
        _attacking = true;
        _anim.SetBool("UsingWeapon", true);

        AnimationClip weaponUse = _anim.runtimeAnimatorController.animationClips.ToList().Find(x => x.name == "UseWeapon");
        yield return new WaitForSeconds(weaponUse.length + attackData.finishDuration);

        _anim.SetBool("UsingWeapon", false);
        _playerDamaged = false;
        _attacking = false;
    }

    protected void DamagePlayer()
    {
        playerDamaged.Invoke(attackData.damage);
        _playerDamaged = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_attacking && !_playerDamaged) DamagePlayer();
        }
    }
}
