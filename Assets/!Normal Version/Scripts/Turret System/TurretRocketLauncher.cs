using _AR_.Extensions;
using ScriptsNormal.Bullets_Projectiles;
using JoostenProductions;
using TriInspector;
using Turrets;
using Pool;
using UnityEngine;

#region DeclareFoldoutGroup
[DeclareBoxGroup("References", Title = "References")]
[DeclareBoxGroup("Base", Title = "Base")]
[DeclareBoxGroup("Rocket", Title = "Rocket data")]
[DeclareBoxGroup("Debug", Title = "Debug")]
#endregion

public class TurretRocketLauncher : OverridableMonoBehaviour
{
    #region Variables

    [GroupNext("References")]
    [SerializeField] private TurretRotation turretRotation;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SimpleRocket simpleRocket;
    
    [GroupNext("Base")]
    [SerializeField] private float cooldown = 0.1f;
    
    [GroupNext("Rocket")]
    [SerializeField] private LayerMask hitLayer = 1 << 0;
    [SerializeField] private int damage = 10;
    [SerializeField] private int flyTime = 10;
    [SerializeField] private float rocketRotSpeed = 5;

    // Cache
    private bool inited;
    private SimpleRocket currRocket;
    private DamageStruct _damageStruct;
    private float cooldownTimer;
    
    #endregion

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (inited)
            return;

        _damageStruct = new DamageStruct();
        UpdateDamageInfo();
        
        inited = true;
    }
    
    
    protected override void OnEnable()
    {
        base.OnEnable();
        cooldownTimer = cooldown;
    }
    
    public override void UpdateMe()
    {
        if (turretRotation.IsOnIdle)
            return;
        
        if (CheckIf_CanShoot())
        {
            Shoot();
        }
        
        // Cooldown
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
        
        // if (drawDebug)
        //     DrawDebugRays();
    }
    
    private bool CheckIf_CanShoot()
    {
        if (cooldownTimer > 0)
            return false;

        if (turretRotation.currTarget && turretRotation.layersToDetect.Contain(turretRotation.currTarget.gameObject.layer) == false)
        {
            currRocket = null;
            return false;
        }
        
        if (turretRotation.currTarget)
            return true;

        return false;
    }
    
    [Button]
    private void Shoot()
    {
        cooldownTimer = cooldown;
        
        currRocket = ObjectPoolManager.Instance.GetObjectAuto(simpleRocket.name, simpleRocket);
        currRocket.transform.SetNewPosRot(spawnPoint);

        UpdateDamageInfo();
        currRocket.Init(_damageStruct);
        currRocket.gameObject.SetActive(true);
    }
    
    private void UpdateDamageInfo()
    {
        _damageStruct.damage = damage;
        _damageStruct.hitLayer = hitLayer;
        _damageStruct.sender = transform;
        
        _damageStruct.currentTarget = turretRotation.currTarget;
        _damageStruct.rotSpeed = rocketRotSpeed;
        _damageStruct.flyTime = flyTime;
        _damageStruct.startPos = spawnPoint.transform.position;
    }
}
