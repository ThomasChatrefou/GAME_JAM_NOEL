using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Character : NetworkBehaviour
{
    [Header("Character")]
    [SerializeField]
    protected float health;
    [SerializeField]
    protected float speedValue;

    public UnityEvent OnDeath;

    
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
                DieServerRpc();
            }
        }
      
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void DieServerRpc()
    {
        Debug.Log("Character " + gameObject.name + " Die");
        isDead = true;
        OnDeath.Invoke();
    }
}
