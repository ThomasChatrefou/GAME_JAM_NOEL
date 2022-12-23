using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

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
    [SerializeField]
    private GameObject pressKeyUI;

    [Header("Ground")] 
    [SerializeField] 
    private bool onGround;
    [SerializeField]
    private PlayerController nearbyPlayer;

    [Header("Ammo")] 
    [SerializeField] private bool isBaseWeapon;
    [SerializeField]private int maxAmmo;
    [SerializeField]private int actualAmmo;



    // Start is called before the first frame update
    void Start()
    {
        pressKeyUI.SetActive(false);
        actualAmmo = maxAmmo;
        //Set Ammo to max
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
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    public void LaunchProjectile(Vector2 direction, bool isFromEnemy = false)
    {
        if (actualAmmo >= 1 || isBaseWeapon)
        {
            if (!hasShoot) {
                Projectile proj = Instantiate(baseProjectilePrefab,firePos.position,
                        Quaternion.Euler(0,0,Mathf.Atan2(firePos.position.x, firePos.position.y) * Mathf.Rad2Deg))
                    .GetComponent<Projectile>();
                proj.DirProjectile = direction;
                proj.isFromEnemy = isFromEnemy;
                proj.GetComponent<NetworkObject>().Spawn(true); 
                
                if (!isBaseWeapon)
                {
                    actualAmmo -= 1;    
                }
            }
            hasShoot = true;
        }
        else
        {
            LaunchSpecialProjectile(direction,isFromEnemy);
        }
       
    }
    
    public void LaunchSpecialProjectile(Vector2 direction, bool isFromEnemy = false)
    {
        Projectile proj = Instantiate(skillProjectilePrefab,firePos.position,
                Quaternion.Euler(0,0,Mathf.Atan2(firePos.position.x, firePos.position.y) * Mathf.Rad2Deg))
            .GetComponent<Projectile>();
        proj.DirProjectile = direction;
        proj.isFromEnemy = isFromEnemy;
        proj.GetComponent<NetworkObject>().Spawn(true);
    }

    //TODO RPC
    [ServerRpc(RequireOwnership = false)]
    public void LaunchSkillProjectileServerRpc(Vector2 direction)
    {
        Projectile weapon = Instantiate(skillProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
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

    [ServerRpc(RequireOwnership = false)]
    public void DespawnWeaponServerRpc()
    {
        Debug.Log(GetComponent<NetworkObject>().TryRemoveParent());
        GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveToParentServerRpc(ulong idGameObject)
    {
        NetworkObject playerNetworkObj = NetworkManager.Singleton.ConnectedClients[idGameObject].PlayerObject;
        Debug.Log(GetComponent<NetworkObject>()
            .TrySetParent(playerNetworkObj.transform));
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if(parentNetworkObject == null) return;
        transform.SetParent(parentNetworkObject.transform);
        transform.localPosition = Vector3.zero;
        SpawnWeaponGround(false);
        parentNetworkObject.GetComponent<PlayerController>().PlayerWeapon = this;
    }
    
    public void SpawnWeaponGround(bool value)
    {
        //Make the item disapears from the map and be equiped by the player
        if (value)
        {
            spriteRenderer.sprite = weaponSpriteGround;    
        }
        else
        {
            spriteRenderer.sprite = weaponSpriteHands;
        }
        colliderPlayerEquip2D.enabled = value;
        onGround = value;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (onGround)
        {
            if (col.CompareTag("Player"))
            {
                PlayerController player = col.gameObject.GetComponent<PlayerController>();
                if (player.IsOwner)
                {
                    if (!nearbyPlayer)
                    {
                        pressKeyUI.SetActive(true);
                    }
                    nearbyPlayer = player;
                    nearbyPlayer.NearbyWeapon = this;
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && nearbyPlayer)
        {
            if (other.gameObject.Equals(nearbyPlayer.gameObject))
            {
                if (nearbyPlayer)
                {
                    pressKeyUI.SetActive(false);
                }
                nearbyPlayer.NearbyWeapon = null;
                nearbyPlayer = null;
            }
        }
    }
    
    
    public GameObject BaseProjectilePrefab
    {
        get => baseProjectilePrefab;
        set => baseProjectilePrefab = value;
    }

    public GameObject SkillProjectilePrefab
    {
        get => skillProjectilePrefab;
        set => skillProjectilePrefab = value;
    }
    
    public int ActualAmmo => actualAmmo;
    public bool IsBaseWeapon => isBaseWeapon;

}