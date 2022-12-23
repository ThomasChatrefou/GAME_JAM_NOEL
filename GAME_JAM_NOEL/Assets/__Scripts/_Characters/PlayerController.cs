using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Character
{
    public PlayerInputs inputs;
    
    [SerializeField]
    private Vector2 speedDirection;

    [Header("Weapon")] 
    [SerializeField] private GameObject shovelWeaponPrefab;

    private Weapon playerWeapon;
    private Weapon nearbyWeapon;

    private void Awake()
    {
        inputs = new PlayerInputs();

        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.SpecialShoot.performed += shootContext => SpecialShoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Interact.performed += interactContext => EquipWeapon(nearbyWeapon);
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Start()
    {
        if (!IsOwner) return;
        SpawnMyWeaponServerRpc();
    }

    [ServerRpc(RequireOwnership = true)]
    private void SpawnMyWeaponServerRpc()
    {
        playerWeapon = Instantiate(shovelWeaponPrefab, transform.position, Quaternion.identity, transform).GetComponent<Weapon>();
        NetworkObject weaponNO = playerWeapon.GetComponent<NetworkObject>();
        weaponNO.SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
        weaponNO.TrySetParent(gameObject);
    }

    private void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speedValue * new Vector3(speedDirection.x, speedDirection.y, 0.0f);
    }

    private void Move(Vector2 direction)
    {
        if (!IsOwner) return;
        if (!isDead)
        {
            speedDirection = direction;
        }
        else
        {
            speedDirection = Vector2.zero;
        }
    }

    private void Shoot()
    {
        if(!IsOwner || !GameManager.Instance.IsRunning)
            return;
        OnShootServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnShootServerRpc()
    {
        playerWeapon.LaunchProjectile();
    }

    private void SpecialShoot()
    {
        if (!IsOwner) return;
        SpecialShootServerRpc(transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpecialShootServerRpc(Vector2 playerPos)
    {
        playerWeapon.LaunchSpecialProjectile();

        // Re-equip Shovel Weapon
        playerWeapon.DespawnWeaponServerRpc();
        SpawnMyWeaponServerRpc();
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (!IsOwner || !weapon) return;
        playerWeapon.DespawnWeaponServerRpc();

        weapon.MoveToParentServerRpc(GetComponent<NetworkObject>().OwnerClientId);
        playerWeapon = weapon;
        //TODO Update UI Sprite
    }

    public Weapon NearbyWeapon
    {
        get => nearbyWeapon;
        set => nearbyWeapon = value;
    }

    public Weapon PlayerWeapon
    {
        get => playerWeapon;
        set => playerWeapon = value;
    }
}
