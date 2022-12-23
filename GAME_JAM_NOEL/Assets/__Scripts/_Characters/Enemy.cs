using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Enemy : Character
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] 
    private GameObject lootedWeapon;
    [SerializeField]
    private bool willLootWeapon;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    [ServerRpc(RequireOwnership = false)]
    protected override void DieServerRpc()
    {
        spriteRenderer.enabled = false;
        if (lootedWeapon && willLootWeapon)
        {
            Weapon weapon = Instantiate(lootedWeapon, transform.position, quaternion.identity).GetComponent<Weapon>();
            weapon.SpawnWeaponGround(true);
            weapon.GetComponent<NetworkObject>().Spawn(true);
        }

        DestroyEnemyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DestroyEnemyServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
