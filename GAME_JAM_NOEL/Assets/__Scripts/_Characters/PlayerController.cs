using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerController : Character
{
    public PlayerInputs inputs;
    
    [SerializeField]
    private Vector2 speedDirection;
    private MouseDetection mouseDetection;

    [Header("Weapon")] 
    [SerializeField] 
    private GameObject shovelWeaponPrefab;
    [SerializeField] 
    private Weapon playerWeapon;


    [SerializeField] 
    private Vector2 crossPosition;
    private Weapon nearbyWeapon;


    private void Awake()
    {
        mouseDetection = FindObjectOfType<MouseDetection>();
        inputs = new PlayerInputs();

        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.SpecialShoot.performed += shootContext => SpecialShoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Interact.performed += interactContext => EquipWeapon(nearbyWeapon);
    }

    private void Start()
    {
        if(!IsOwner) return;
        SpawnWeaponFromPlayerServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpawnWeaponFromPlayerServerRpc()
    {
        playerWeapon = Instantiate(shovelWeaponPrefab, shovelWeaponPrefab.transform.position, Quaternion.identity,transform).GetComponent<Weapon>();
        playerWeapon.GetComponent<NetworkObject>().Spawn();
        playerWeapon.MoveToParentServerRpc(GetComponent<NetworkObject>().OwnerClientId);
    }

    private void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speedValue * new Vector3(speedDirection.x, speedDirection.y, 0.0f);
    }

    private void Update()
    {
        Tic();
        //TODO Faire viser le personnage où la souris est placée uniquement si c'est le perso du client 
        crossPosition = mouseDetection.MousePos;
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
        if(!IsOwner) return;
        AttackServerRpc(crossPosition,transform.position);
    }
    
    private void SpecialShoot()
    {
        if(!IsOwner) return;
        SpecialAttackServerRpc(crossPosition,transform.position);
    }

    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public override void AttackServerRpc(Vector2 crossPosition, Vector2 playerPos)
    {
        Vector2 fireDir = crossPosition - playerPos;
        playerWeapon.LaunchProjectile(fireDir.normalized);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SpecialAttackServerRpc(Vector2 crossPosition, Vector2 playerPos)
    {
        if (playerWeapon.SkillProjectilePrefab != null)
        {
            Vector2 fireDir = crossPosition - playerPos;
            playerWeapon.LaunchSpecialProjectile(fireDir.normalized);
            //Re-equip Shovel Weapon
            SpawnWeaponFromPlayerServerRpc();
        }
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
