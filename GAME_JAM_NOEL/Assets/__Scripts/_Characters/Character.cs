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
    protected Projectile lastProjectileTaken;


    public void Tic()
    {
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public virtual void AttackServerRpc()
    {

    }

    public virtual void TakeDamage(Projectile projectile)
    {
        if (lastProjectileTaken != projectile)
        {
            lastProjectileTaken = projectile;
            health -= projectile.DamageProjectile;
            if(health <= 0){
                Die();
            }
        }
      
    }

    protected virtual void Die()
    {
        Debug.Log("Character " + gameObject.name + " Die");
        isDead = true;
    }
}
