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
        
    }
    
    [ServerRpc(RequireOwnership = false)]
    public virtual void AttackServerRpc(Vector2 crossPosition, Vector2 playerPos)
    {

    }

    public virtual void TakeDamage(Projectile projectile)
    {
        if (lastProjectileTaken != projectile)
        {
            lastProjectileTaken = projectile;
            health -= projectile.DamageProjectile;
            if(health <= 0){    
                DieServerRpc();
            }
        }
      
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void DieServerRpc()
    {
        Debug.Log("Character " + gameObject.name + " Die");
        isDead = true;
    }
}
