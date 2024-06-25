using UnityEngine;

[System.Serializable]
public class DamageData2
{
    public float damageValue = 15;
    
    [HideInInspector] public Transform sender;
    [HideInInspector] public Transform receiver;
    [HideInInspector] public Vector3 hitPosition;
    [HideInInspector] public Vector3 force;


    public DamageData2(DamageData2 damageData)
    {
        this.damageValue = damageData.damageValue;
        this.sender = damageData.sender;
        this.receiver = damageData.receiver;
        this.hitPosition = damageData.hitPosition;
        this.force = damageData.force;
    }
}