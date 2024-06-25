using UnityEngine;
using Pool;
using UnityEngine.PlayerLoop;

namespace ScriptsNormal.Bullets_Projectiles
{
    public class SimpleBulletJob : DamageBase
    {
        #region Variables

        [SerializeField] 
        private Transform destroyEffect;

        #endregion

        #region Set
        
        #endregion

        public override void Init(DamageStruct damageStruct)
        {
            this.damageStruct = damageStruct; 
            base.Init(damageStruct);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            timetemp = 0;
        }
        
        public void Update()
        {
            if (inited == false)
                return;

            if (rayChecker)
                RayCheckerSingle();

            // if (lifeTime > 0)
            //     UpdateLifeTime();

            //Move
            transform.Translate(damageStruct.velocity * Time.deltaTime * Vector3.forward, Space.Self);
        }

        private void UpdateLifeTime()
        {
            timetemp += Time.deltaTime;
            
            if (timetemp >= lifeTime)
                Despawn();
        }
        
        protected override void Despawn()
        {
            gameObject.SetActive(false);
            ObjectPoolManager.Instance.ReturnObject(objectName, this);
        }

//
//        private void ExplosionDamage()
//        {
//            Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
//            for (int i = 0; i < hitColliders.Length; i++)
//            {
//                Collider hit = hitColliders[i];
//                if (!hit)
//                    continue;
//
//                if (hit.GetComponent<Rigidbody>())
//                    hit.GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f);
//
//            }
//
//            Collider[] dmhitColliders = Physics.OverlapSphere(transform.position, DamageRadius);
//
//            for (int i = 0; i < dmhitColliders.Length; i++)
//            {
//                Collider hit = dmhitColliders[i];
//
//                if (!hit)
//                    continue;
//
//                if (DoDamageCheck(hit.gameObject) && (Owner == null || (Owner != null && hit.gameObject != Owner.gameObject)))
//                {
//                    DamagePack damagePack = new DamagePack();
//                    damagePack.Damage = Damage;
//                    damagePack.Owner = Owner;
//                    hit.gameObject.SendMessage("ApplyDamage", damagePack, SendMessageOptions.DontRequireReceiver);
//                }
//            }
//
//        }
//
//        private void NormalDamage(Collision collision)
//        {
//            DamagePack damagePack = new DamagePack();
//            damagePack.Damage = Damage;
//            damagePack.Owner = Owner;
//            collision.gameObject.SendMessage("ApplyDamage", damagePack, SendMessageOptions.DontRequireReceiver);
//        }
    }
}