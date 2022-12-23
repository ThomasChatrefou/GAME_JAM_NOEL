using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BT_Tree))]
public class Enemy : Character
{
    [Header("Enemy")]
    protected SpriteRenderer spriteRenderer;
    [SerializeField] 
    private GameObject lootedWeapon;
    [SerializeField]
    private bool willLootWeapon;

    private BT_Tree aiTree;
    protected Transform target;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        aiTree = GetComponent<BT_Tree>();
        aiTree.SetupTree();
        aiTree.StartBehaviour();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (target != null)
        {
            transform.position += Time.fixedDeltaTime * speedValue * (target.position - transform.position).normalized;
        }
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
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
