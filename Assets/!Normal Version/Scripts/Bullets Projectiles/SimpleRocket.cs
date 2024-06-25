using System;
using ScriptsNormal.Bullets_Projectiles;
using TriInspector;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleRocket : DamageBase
{
    #region Variables
 
    [Space]
    public float height=3;
    public float flyTime=2;
    public float rotSpeed;
    
    [Space]
    public AnimationCurve smoothCurve; // Kind like turn speed
    private float time;
    private float _smooth; // Kind like turn speed
    private Vector3 targetPos;
    public Vector3 startPos;

    [Header(GlobalString.Debug)]
    [ReadOnly] public Transform target;

    #endregion
    
    public override void Init(DamageStruct damageStruct)
    {
        rotSpeed = damageStruct.rotSpeed;
        flyTime = damageStruct.flyTime;
        startPos = damageStruct.startPos;
        this.damageStruct = damageStruct; 

        SetTarget(damageStruct.currentTarget);
        base.Init(damageStruct);
    }
    
    public void SetDamage(float value)
    {
        damageStruct.damage = value;
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        targetPos = target.position;
    }


    protected override void OnEnable()
    {
        time = 0;
        base.OnEnable();
    }

    public void Update()
    {
        if (target == null || inited == false)
            return;
        
        if (rayChecker)
            RayCheckerSingle();

        _smooth = smoothCurve.Evaluate(time) * rotSpeed;

        SmoothLookAt ();
        Move();
        
        time += Time.deltaTime;
    }

    
    private void Move()
    {
        transform.position = MathParabola.Parabola(startPos, targetPos, height, time / flyTime);
        //transform.Translate(_speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    private void SmoothLookAt()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _smooth);
    }

    protected override void Despawn()
    {
        ObjectPoolManager.Instance.ReturnObject(name, this);
    }

}


public class MathParabola
{
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
}