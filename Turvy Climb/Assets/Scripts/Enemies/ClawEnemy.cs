using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawEnemy : Enemy
{
    [Header("Claw parameters")]
    [SerializeField] private Transform weaponParent;
    public CircleCollider2D playerCloseDetector;
    public CircleCollider2D playerImminentDetector;

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        List<Collider2D> inColliders = new List<Collider2D>();
        ContactFilter2D cFilter = new ContactFilter2D();
        other.OverlapCollider(cFilter.NoFilter(), inColliders);

        //Debug.Log(state);
        if (inColliders.Contains(playerCloseDetector))
        {
            //Debug.Log("Claw detected something is aproaching...");
            weapon.ReadyWeapon();
        }
        if (inColliders.Contains(playerImminentDetector))
        {
            Debug.Log("Claw attacks!");
            weapon.Attack();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        List<Collider2D> inColliders = new List<Collider2D>();
        ContactFilter2D cFilter = new ContactFilter2D();
        other.OverlapCollider(cFilter.NoFilter(), inColliders);

        if (!inColliders.Contains(playerCloseDetector))
        {
            weapon.UnreadyWeapon();
        }
    }
}
