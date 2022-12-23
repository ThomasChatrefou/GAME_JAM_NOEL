using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Character : NetworkBehaviour
{
    public UnityEvent OnDeath;

    [SerializeField]
    protected float health;
    [SerializeField]
    protected float speedValue;

    protected bool isDead;
    public bool IsDead => isDead;
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
                Die();
            }
        }
      
    }

    protected virtual void Die()
    {
        Debug.Log("Character " + gameObject.name + " Die");
        isDead = true;
        OnDeath.Invoke();
    }
}
