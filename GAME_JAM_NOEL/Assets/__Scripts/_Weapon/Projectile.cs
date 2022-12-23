using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : NetworkBehaviour
{
    [SerializeField]
    private float speedProjectile;
    [SerializeField] 
    private float lifeSpan;
    [SerializeField]
    private float damageProjectile;

    //Unused
    //[SerializeField] private float knockbackProjectile;

    private Vector2 dirProjectile;
    [SerializeField]
    private bool isDestroyedOnFirstHit;

    [HideInInspector] public bool isFromEnemy;

    public UnityEvent onSpawnEvent;
    private Rigidbody2D rgbd;

    [Header("Feedback")]
    [HideInInspector] public NetworkBehaviour ParentWeapon;
    [SerializeField] private GameObject HitEffectPrefab;
    [SerializeField] private float cameraShakeIntensity;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float cameraShakeFrequency;
    [SerializeField] private Vector3 graphicsOrientation;

    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        LaunchProjectile();
    }

    private void Update()
    {
        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void LaunchProjectile()
    {
        rgbd.velocity = (dirProjectile * speedProjectile);
        var zAngle = Mathf.Atan2(dirProjectile.x, dirProjectile.y) * Mathf.Rad2Deg;

        if (onSpawnEvent.GetPersistentEventCount() > 0)
        {
            onSpawnEvent.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        character.TakeDamage(this);

        if (!isFromEnemy)
        {
            if (other.CompareTag("Enemy"))
            {
                GameObject particlesGO = Instantiate(HitEffectPrefab, character.transform.position, Quaternion.identity);
                particlesGO.transform.eulerAngles = transform.eulerAngles + graphicsOrientation;
                if (ParentWeapon == null) return;
                if (!ParentWeapon.IsOwner) return;
                CameraShaker.Instance.ShakeCamera(cameraShakeIntensity, cameraShakeDuration, cameraShakeFrequency);
            }
        }

        if (isDestroyedOnFirstHit)
        {
            Destroy(gameObject);
        }
    }

    public void RotationTreeSkill()
    {
        transform.DORotate(new Vector3(0, 0, 360), lifeSpan - 0.15f, RotateMode.FastBeyond360).SetRelative(true);
    }

    public Vector2 DirProjectile
    {
        get => dirProjectile;
        set => dirProjectile = value;
    }
    
    public float DamageProjectile => damageProjectile;

}
