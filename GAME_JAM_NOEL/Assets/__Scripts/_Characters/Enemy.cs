using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Die()
    {
        spriteRenderer.enabled = false;
        if (lootedWeapon && willLootWeapon)
        {
            //TODO Faire spawn l'arme par terre
            Weapon weapon = Instantiate(lootedWeapon, transform.position, quaternion.identity).GetComponent<Weapon>();
            weapon.GetComponent<NetworkObject>().Spawn(true);
            //weapon.SpawnWeaponGroundServerRpc();
            weapon.SpawnWeaponGround();
        }

        DestroyEnemyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DestroyEnemyServerRpc()
    {
        Debug.Log("Despawn");
        GetComponent<NetworkObject>().Despawn();
    }
}
