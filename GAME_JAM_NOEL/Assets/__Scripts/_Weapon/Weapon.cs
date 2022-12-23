using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

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

    public PlayerController weaponOwner;

    private NetworkVariable<Vector2> aimPosition = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

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

        if (IsOwner)
        {
            UpdateAimPosition();
            HandleRotation();
        }

    }

    private void UpdateAimPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimPosition.Value = new Vector2(worldPosition.x, worldPosition.y);
    }

    public Vector3 GetAimPosition()
    {
        return new Vector3(aimPosition.Value.x, aimPosition.Value.y, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, GetAimPosition());
    }

    private void HandleRotation()
    {
        Vector3 aimDirection = (GetAimPosition() - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Debug.Log("Sprite Renderer " + spriteRenderer);
    }

    //TODO RPC
    public void LaunchBaseProjectile(Vector2 mousePosition, bool isFromEnemy = false)
    {
        if (!hasShoot)
        {
            GameObject projectileGO = Instantiate(baseProjectilePrefab, firePos.position, firePos.rotation);
            Projectile proj = projectileGO.GetComponent<Projectile>();
            proj.DirProjectile = (mousePosition - new Vector2(transform.position.x, transform.position.y)).normalized;
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

    public void SpawnWeaponGround()
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
            Debug.Log("OnGround Col " + col.name);
            if (col.GetType() == typeof(PlayerController))
            {
                Debug.Log("OnTrigger PlayerController");
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
