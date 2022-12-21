using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct WeaponData : INetworkSerializable
{
    public SpriteRenderer spriteRenderer;
    public Collider2D collider2D;
    public bool onGround;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref onGround);
        //serializer.SerializeNetworkSerializable(ref );
        throw new NotImplementedException();
    }
}
public class Weapon : NetworkBehaviour
{
    [Header("Projectiles")]
    [SerializeField]
    private float timerReload;
    [SerializeField] 
    private float timerReloadMax;
    [SerializeField]
    private GameObject baseProjectilePrefab;
    [SerializeField]
    private GameObject skillProjectilePrefab;
    [SerializeField] 
    private Transform firePos;

    private bool hasShoot;

    [Header("Sprite Management")] 
    private SpriteRenderer spriteRenderer;
    public Sprite weaponSpriteUI;
    public Sprite weaponSpriteGround;
    public Sprite weaponSpriteHands;

    [Header("Trigger Equip")] 
    public Collider2D colliderPlayerEquip2D;

    [Header("Ground")] 
    public bool onGround;
    private PlayerController nearbyPlayer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasShoot)
        {
            timerReload -= Time.deltaTime;
            if (timerReload <= 0)
            {
                hasShoot = !hasShoot;
                timerReload = timerReloadMax;
            }
        }

        if (onGround)
        {
            if (nearbyPlayer)
            {
                //TODO Display Grab Key
            }
        }
    }

    //TODO RPC
    public void LaunchBaseProjectile(Vector2 direction, bool isFromEnemy = false)
    {
        if (!hasShoot)
        {
            Projectile proj = Instantiate(baseProjectilePrefab,firePos.position,
                Quaternion.Euler(0,0,Mathf.Atan2(firePos.position.x, firePos.position.y) * Mathf.Rad2Deg))
                .GetComponent<Projectile>();
            proj.DirProjectile = direction;
            proj.isFromEnemy = isFromEnemy;
            proj.GetComponent<NetworkObject>().Spawn(true);
        }
        hasShoot = true;
    }

    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void LaunchSkillProjectileServerRpc(Vector2 direction)
    {
        Projectile weapon = Instantiate(skillProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
    }

    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void PickedUpServerRpc()
    {
        //Make the item disapears from the map and be equiped by the player
        GetComponent<SpriteRenderer>().sprite = weaponSpriteHands;
    }
    
    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void SpawnWeaponGroundServerRpc()
    {
        //Make the item disapears from the map and be equiped by the player
        spriteRenderer.sprite = weaponSpriteGround;
        colliderPlayerEquip2D.enabled = true;
        onGround = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (onGround)
        {
            if (col.GetType() == typeof(PlayerController))
            {
                PlayerController player = col.GetComponent<PlayerController>();
                if (player.IsOwner)
                {
                    Debug.Log("Enable Equip Weapon");
                    nearbyPlayer = player;
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.Equals(nearbyPlayer.gameObject))
        {
            nearbyPlayer = null;
        }
    }
}
