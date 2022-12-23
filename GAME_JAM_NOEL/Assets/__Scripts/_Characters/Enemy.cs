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
