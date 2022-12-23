using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [Header("Projectiles")]
    [SerializeField] private float timerReload;
    [SerializeField] private float timerReloadMax;
    [SerializeField] private GameObject baseProjectilePrefab;

    [Space]
    [SerializeField] private GameObject skillProjectilePrefab;
    [SerializeField] private Transform firePos;

    [Header("Sprite Management")]
    [SerializeField] private Sprite weaponSpriteUI;
    [SerializeField] private Sprite weaponSpriteGround;
    [SerializeField] private Sprite weaponSpriteHands;

    [Header("Trigger Equip")]
    [SerializeField] private Collider2D colliderPlayerEquip2D;
    [SerializeField] private GameObject pressKeyUI;

    [Header("Ground")]
    [SerializeField] private bool onGround;

    [Header("Ammo")]
    [SerializeField] private bool isBaseWeapon;
    [SerializeField] private int maxAmmo;
    
    private int actualAmmo;

    private SpriteRenderer spriteRenderer;
    private PlayerController nearbyPlayer;

    private NetworkVariable<Vector2> aimPosition = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );

    private bool hasShot;

    void Start()
    {
        if (pressKeyUI != null)
        {
            pressKeyUI.SetActive(false);
        }
        actualAmmo = maxAmmo;
    }

    void Update()
    {
        if (hasShot)
        {
            timerReload -= Time.deltaTime;
            if (timerReload <= 0)
            {
                hasShot = !hasShot;
                timerReload = timerReloadMax;
            }
        }

        if (IsOwner && !onGround)
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

    public Vector3 GetAimDirection()
    {
        return (GetAimPosition() - transform.position).normalized;
    }

    public Vector2 GetAimDirection2D()
    {
        return new Vector2(aimPosition.Value.x - transform.position.x, aimPosition.Value.y - transform.position.y).normalized;
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
    }

    public void LaunchProjectile(bool isFromEnemy = false)
    {
        if (actualAmmo >= 1 || isBaseWeapon)
        {
            if (!hasShot)
            {
                LaunchBaseProjectile();

                if (!isBaseWeapon)
                {
                    actualAmmo -= 1;
                }
            }
            hasShot = true;
        }
        else
        {
            LaunchSpecialProjectile(isFromEnemy);
        }
    }

    private void LaunchBaseProjectile(bool isFromEnemy = false)
    {
        GameObject projectileGO = Instantiate(baseProjectilePrefab, firePos.position, firePos.rotation);
        Projectile proj = projectileGO.GetComponent<Projectile>();

        proj.DirProjectile = GetAimDirection2D();
        proj.isFromEnemy = isFromEnemy;
        proj.ParentWeapon = this;

        hasShot = true;
    }

    private void LaunchSpecialProjectile(bool isFromEnemy = false)
    {
        Projectile proj = Instantiate(skillProjectilePrefab, firePos.position,
                Quaternion.Euler(0, 0, Mathf.Atan2(firePos.position.x, firePos.position.y) * Mathf.Rad2Deg))
            .GetComponent<Projectile>();
        proj.DirProjectile = GetAimDirection2D();
        proj.isFromEnemy = isFromEnemy;
        proj.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LaunchSkillProjectileServerRpc()
    {
        Projectile weapon = Instantiate(skillProjectilePrefab,transform.position,Quaternion.identity).GetComponent<Projectile>();
    }
    
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
        Debug.Log(GetComponent<NetworkObject>().TrySetParent(playerNetworkObj.transform));
    }

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        transform.localPosition = Vector3.zero;
        SpawnWeaponGround(false);
    }

    public void SpawnWeaponGround(bool value)
    {
        //Make the item disapear from the map and be equiped by the player
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
