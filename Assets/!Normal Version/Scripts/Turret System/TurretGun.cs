using _AR_.Extensions;
using ScriptsNormal.Bullets_Projectiles;    
using JoostenProductions;
using TriInspector;
using Turrets;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;

#region DeclareFoldoutGroup

[DeclareBoxGroup("References", Title = "References")]
[DeclareBoxGroup("Base", Title = "Base")]
[DeclareBoxGroup("Bullet", Title = "Bullet data")]
[DeclareBoxGroup("Debug", Title = "Debug")]
#endregion

public class TurretGun : OverridableMonoBehaviour
{
    #region Variables

    [GroupNext("References")]
    //[Header("References")]
    [SerializeField] private TurretRotation turretRotation;
    [SerializeField] private SimpleBullet simpleBulletPrefab;
    [SerializeField] private Transform spawnPoint;
    
    [GroupNext("Base")]
    public float cooldown = 0.1f;
    
    [GroupNext("Bullet")]
    [SerializeField] private LayerMask bulletHitLayer = 1 << 0;
    [SerializeField] private float velocity = 380;
    [SerializeField] private int damage = 10;

    [GroupNext("Debug")]
    [SerializeField] private bool drawDebug;
    
    // Cache
    private bool inited;
    private SimpleBullet currBullet;
    private DamageStruct _damageInfo;
    private float cooldownTimer;

    #endregion

    private void Awake()
    {
        Init();
        
        if (simpleBulletPrefab == null)
        {
            Debug.LogError($"AR: No bullet prefab at {transform.name}" );
            enabled = false;
        }
        
        if (spawnPoint == null)
        {
            Debug.LogError($"AR: No spawnPoint at {transform.name}" );
            enabled = false;
        }
    }

    private void Init()
    {
        if (inited)
            return;

        _damageInfo = new DamageStruct();
        UpdateDamageInfo();
        
        inited = true;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        cooldownTimer = cooldown;
    }

    /// <summary>
    /// Update inherited from UpdateManager
    /// </summary>
    public override void UpdateMe()
    {
        if (turretRotation.IsOnIdle)
            return;

        if (CanShoot())
        {
            Shoot();
        }

        // Cooldown
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (drawDebug)
            DrawDebugRays();
    }

    private bool CanShoot()
    {
        if (cooldownTimer > 0)
            return false;

        if (RaycastExtAR.SafeRaycast(spawnPoint.transform.position, spawnPoint.forward, turretRotation.sphereCol.radius, turretRotation.layersToDetect))
            return true;
        else
            return false;
    }

    private void Shoot()
    {
        cooldownTimer = cooldown;
        
        currBullet = ObjectPoolManager.Instance.GetObjectAuto(simpleBulletPrefab.name, simpleBulletPrefab);
        currBullet.transform.SetNewPosRot(spawnPoint);
        
        currBullet.Init(_damageInfo);
        currBullet.gameObject.SetActive(true);
    }

    
    private void UpdateDamageInfo()
    {
        _damageInfo.damage = damage;
        _damageInfo.velocity = velocity;
        _damageInfo.hitLayer = bulletHitLayer;
        _damageInfo.sender = transform;
    }
    
    private void DrawDebugRays()
    {
        if (spawnPoint != null)
            Debug.DrawRay(spawnPoint.position, spawnPoint.forward * 100.0f);
    }
}