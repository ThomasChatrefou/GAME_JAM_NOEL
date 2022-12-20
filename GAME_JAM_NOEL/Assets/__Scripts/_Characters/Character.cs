using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Character : NetworkBehaviour
{
    [SerializeField]
    protected float health;
    [SerializeField]
    protected float speedValue;

    protected bool isDead;


    public void Tic()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    public virtual void Attack()
    {
        return;
    }

    protected virtual void Die()
    {
        Debug.Log("Character " + gameObject.name + " Die");
        isDead = true;
    }
}
