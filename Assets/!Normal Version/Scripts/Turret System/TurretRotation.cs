using System;
using System.Collections.Generic;
using _AR_.Extensions;
using JoostenProductions;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Turrets
{
    #region DeclareFoldoutGroup

    [DeclareBoxGroup("Base", Title = "Base")]
    [DeclareBoxGroup("References", Title = "References")]
    [DeclareBoxGroup("Debug", Title = "Debug")]
    [RequireComponent(typeof(SphereCollider))]

    #endregion

    public class TurretRotation : OverridableMonoBehaviour
    {
        #region Variables

        public enum TurretState
        {
            Idle,
            ToIdle,
            TargetAtRange,
        }

        [GroupNext("Base")]
        public LayerMask layersToDetect;

        public float turnSpeedHorizontal = 30.0f;
        public float turnSpeedVertical = 120.0f;

        [OnValueChanged("OnChangeRange")] 
        public float range = 10.0f;
        public bool limitAngleHorizontal = false;
        
        public bool lockVerticalPart = false;

        [ShowIf("limitAngleHorizontal")] 
        [Range(0.0f, 180.0f)]
        public float horizontalLimitAngle = 45f;

        [HideIf("lockVerticalPart")] 
        [Range(0.0f, 90.0f)] 
        public float upLimit = 60.0f;
        [HideIf("lockVerticalPart")] 
        [Range(0.0f, 90.0f)] 
        public float downLimit = 5.0f;

        [GroupNext("References")] public Transform partRotHorizontal;
        public Transform partRotVertical;
        public SphereCollider sphereCol;

        [GroupNext("Debug")]
        public bool showDebugRay = true;
        public float debugAngleY; 

        [ReadOnly] 
        public Transform currTarget;
        [ReadOnly] 
        public TurretState currentState;
     
        protected bool IsInIdlePosition;
        protected List<Transform> targets = new List<Transform>();
        
        
        [UnGroupNext]
        public bool IsOnIdle => currentState == TurretState.Idle;

        #endregion

        /// <summary>
        /// Call when range of turret is changed
        /// </summary>
        private void OnChangeRange()
        {
            if (sphereCol && range > 1)
                sphereCol.radius = range;
        }

        /// <summary>
        /// Fixed update inherited  
        /// </summary>
        public override void FixedUpdateMe()
        {
            if (showDebugRay)
                DrawDebugRays();

            // if we use limit we must checks angle 
            if (limitAngleHorizontal)
            {
                UpdateState();
                UpdateCurrTarget();
            }

            
            RotateTurret();
        }

        #region Rotate

        private void RotateTurret()
        {
            switch (currentState)
            {
                case TurretState.TargetAtRange:
                    RotateHorizontalPart();
                    RotateVerticalPart();
                    break;
                case TurretState.ToIdle:
                    RotateToIdle();
                    break;
                case TurretState.Idle:
                    // anim or effect?
                    break;
            }
        }

        /// <summary>
        /// Rotates the turret in the horizontal axis
        /// </summary>
        private void RotateHorizontalPart()
        {
            if (partRotHorizontal == null)
                return;

            // Note, the local conversion has to come from the parent.
            Vector3 localTargetPos = transform.InverseTransformPoint(currTarget.position);
            localTargetPos.y = 0.0f;

            // Create new rotation towards the target in local space.
            Quaternion rotationToTarget = Quaternion.LookRotation(localTargetPos);
            Quaternion newRotation = Quaternion.RotateTowards(partRotHorizontal.localRotation,
                rotationToTarget, turnSpeedHorizontal * Time.deltaTime);

            debugAngleY = newRotation.eulerAngles.y;
            if (limitAngleHorizontal )
                newRotation = newRotation.SetRotationY(ClampAngle(newRotation.eulerAngles.y, horizontalLimitAngle/2));

            partRotHorizontal.localRotation = newRotation;
        }
        

        /// <summary>
        /// Rotates the turret in the vertital axis
        /// </summary>
        private void RotateVerticalPart()
        {
            if (lockVerticalPart)
                return;

            if (partRotHorizontal != null && partRotVertical == null)
                return;

            // Note, the local conversion has to come from the parent.
            Vector3 localTargetPos = partRotHorizontal.InverseTransformPoint(currTarget.position);
            localTargetPos.x = 0.0f;

            Vector3 clampedLocalVec2Target;

            // if target is above horizontal part. To avoid look at ground
            if (localTargetPos.y >= 0.0f)
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos,
                    Mathf.Deg2Rad * upLimit, float.MaxValue);
            else
                clampedLocalVec2Target = Vector3.RotateTowards(Vector3.forward, localTargetPos,
                    Mathf.Deg2Rad * downLimit, float.MaxValue);

            // Create new rotation for vertical part
            Quaternion rotationToTarget = Quaternion.LookRotation(clampedLocalVec2Target);
            Quaternion newRotation = Quaternion.RotateTowards(partRotVertical.localRotation, rotationToTarget,
                turnSpeedVertical * Time.deltaTime);

            partRotVertical.localRotation = newRotation;
        }

        /// <summary>
        /// Rotate turret to idle state 
        /// </summary>
        private void RotateToIdle()
        {
            if (partRotHorizontal != null)
            {
                partRotHorizontal.localRotation = Quaternion.RotateTowards(partRotHorizontal.localRotation,
                    Quaternion.identity, turnSpeedHorizontal * Time.deltaTime);
            }

            if (partRotVertical != null)
            {
                if (lockVerticalPart == false)
                {
                    partRotVertical.localRotation = Quaternion.RotateTowards(partRotVertical.localRotation,
                        Quaternion.identity, turnSpeedVertical * Time.deltaTime);
                }
            }

            IsInIdlePosition = IsOnIdlePos();

            if (IsInIdlePosition)
                SetState(TurretState.Idle);
        }

        float ClampAngle(float angle, float maxAngle)
        {
            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            return Mathf.Clamp(angle, -maxAngle, maxAngle);
        }
        #endregion

        /// <summary>
        /// Updates turret status
        /// </summary>
        private void UpdateState()
        {
            if (currTarget == null)
            {
                // No targets so go to IDLE
                if (IsOnIdlePos() == false)
                    SetState(TurretState.ToIdle);
                else
                    SetState(TurretState.Idle);
            }
            else if (currTarget)
            {
                SetState(TurretState.TargetAtRange);
            }
        }

        private void UpdateCurrTarget()
        {
            // No targets
            if (targets.Count == 0)
            {
                SetCurrentTarget(null);
            }
            // Just one potential target
            else if (targets.Count == 1)
            {
                if (limitAngleHorizontal)
                    SetCurrentTarget(transform.FindNearestInFieldOfViewNonAlloc(targets, horizontalLimitAngle));
                else
                    SetCurrentTarget(targets[0]);
            }
            // More than one target. Find nearest
            else if (targets.Count > 1)
            {
                if (limitAngleHorizontal)
                    SetCurrentTarget(transform.FindNearestInFieldOfViewNonAlloc(targets, horizontalLimitAngle));
                else
                    SetCurrentTarget(transform.FindNearest(targets));
            }
        }
        
        
        #region Triggers

        private void OnTriggerEnter(Collider other)
        {
            // Add object to targets if the object acceptable
            if (layersToDetect.Contain(other.gameObject.layer))
            {
                targets.Add(other.transform);
                UpdateCurrTarget();
                UpdateState();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Remove object from targets
            if (layersToDetect.Contain(other.gameObject.layer))
            {
                targets.Remove(other.transform);
                UpdateCurrTarget();
                UpdateState();
            }
        }

        #endregion

        #region Set

        /// <summary>
        /// Set new state for turret
        /// </summary>
        /// <param name="state"></param>
        private void SetState(TurretState state)
        {
            currentState = state;
        }

        /// <summary>
        /// Set new target
        /// </summary>
        /// <param name="newTarget"></param>
        private void SetCurrentTarget(Transform newTarget)
        {
            if (newTarget)
            {
                currTarget = newTarget;
            }
            else
            {
                currTarget = null;
            }
        }
        
        /// <summary>
        /// Set new range for turret
        /// </summary>
        /// <param name="newRange"></param>
        public void SetTriggerRange(float newRange)
        {
            if (sphereCol)
                sphereCol.radius = newRange;
        }

        #endregion


        #region CheckIf

        private bool IsOnIdlePos()
        {
            if (partRotHorizontal && partRotVertical)
            {
                if (lockVerticalPart)
                    return partRotVertical.localRotation == Quaternion.identity;
                
                return partRotVertical.localRotation == Quaternion.identity &&
                       partRotHorizontal.localRotation == Quaternion.identity;
            }
            else if (partRotHorizontal)
            {
                return partRotHorizontal.localRotation == Quaternion.identity;
            }
            else if (partRotVertical)
            {
                return partRotVertical.localRotation == Quaternion.identity;
            }

            return false;
        }
        

        #endregion

        #region Debug

        private void DrawDebugRays()
        {
            if (partRotVertical != null)
                Debug.DrawRay(partRotVertical.position, partRotVertical.forward * 100.0f);
            else if (partRotHorizontal != null)
                Debug.DrawRay(partRotHorizontal.position, partRotHorizontal.forward * 100.0f);

        }

        private void OnDrawGizmos()
        {
            if (sphereCol)
                ExtensionsAR.DrawWireDisk(transform.position, sphereCol.radius, Color.red);

            if (limitAngleHorizontal && horizontalLimitAngle > 0)
            {
                GizmosExtensions.DrawWireArc(transform.position, transform.forward, horizontalLimitAngle, 10);
            }

        }
        #endregion 
     
    }

}