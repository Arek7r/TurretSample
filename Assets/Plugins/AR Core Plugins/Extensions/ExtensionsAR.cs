using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;

namespace _AR_.Extensions
{
    public static class ExtensionsAR
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
    
        public static Enum NextCast(this Enum input)
        {
            Array Arr = Enum.GetValues(input.GetType());
            int j = Array.IndexOf(Arr, input) + 1;
            return (Arr.Length == j) ? (Enum)Arr.GetValue(0) : (Enum)Arr.GetValue(j);
        }

        //Previous with looping
        public static Enum PrevCast(this Enum input)
        {
            Array Arr = Enum.GetValues(input.GetType());
            int j = Array.IndexOf(Arr, input) - 1;
            return (j == -1) ? (Enum)Arr.GetValue(Arr.Length -1) : (Enum)Arr.GetValue(j);
        }
    
        public static T GetOrAddComponent<T>(this GameObject child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.AddComponent<T>();
            }
            return result;
        }

        public static Gradient GetNewGradient(Color color)
        {
            var gradient = new Gradient();
            var colorKey = new GradientColorKey[2];
            colorKey[0].color = color;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.black;
            colorKey[1].time = 1.0f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;
            gradient.SetKeys(colorKey, alphaKey);
        
            return gradient;
        }
    
        public static void SetZeroY(this Vector3 transform, float valueY)
        {
            transform.Set(0, valueY, 0);
        }
        public static void LookAtY(this Transform transform, Vector3 point)
        {
            var lookPos = point - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }

        public static Transform FindNearest(this Transform from, List<Transform> targets)
        {
            Transform nearestCollider = null;
            var minSqrDistance = Mathf.Infinity;
            for (int i = 0; i < targets.Count; i++)
            {
                if (!targets[i]) continue;

                float sqrDistanceToCenter = (from.position - targets[i].transform.position).sqrMagnitude;
                if (sqrDistanceToCenter < minSqrDistance)
                {
                    minSqrDistance = sqrDistanceToCenter;
                    nearestCollider = targets[i].transform;
                }
            }

            return nearestCollider;
        }
        
        public static Transform FindNearestInFieldOfView(this Transform from, List<Transform> targets, float fieldOfViewAngle)
        {
            Transform nearestInFieldOfView = null;
            var minSqrDistance = Mathf.Infinity;

            foreach (Transform target in targets)
            {
                if (target == null)
                    continue;

                Vector3 directionToTarget = target.position - from.position;
                float angleToTarget = Vector3.Angle(from.forward, directionToTarget);

                // Checking whether an object is within a preset viewing angle
                if (angleToTarget <= fieldOfViewAngle * 0.5f)
                {
                    float sqrDistanceToCenter = directionToTarget.sqrMagnitude;
                    if (sqrDistanceToCenter < minSqrDistance)
                    {
                        minSqrDistance = sqrDistanceToCenter;
                        nearestInFieldOfView = target;
                    }
                }
            }

            return nearestInFieldOfView;
        }


        private static List<Transform> filteredTargets = new List<Transform>();

        public static Transform FindNearestInFieldOfViewNonAlloc(this Transform from, List<Transform> targets, float fieldOfViewAngle)
        {
            Transform nearestInFieldOfView = null;
            var minSqrDistance = Mathf.Infinity;

            if (targets.Count == 1)
            {
                if (from.IsInFieldOfView(targets[0], fieldOfViewAngle))
                    return targets[0];
                else
                    return null;
            }
            
            
            filteredTargets.Clear(); // Clear the list to avoid memory allocation.

            foreach (Transform target in targets)
            {
                if (target == null)
                    continue;

                Vector3 directionToTarget = target.position - from.position;
                float angleToTarget = Vector3.Angle(from.forward, directionToTarget);

                // Checking whether an object is within a preset viewing angle
                if (angleToTarget <= fieldOfViewAngle * 0.5f)
                {
                    filteredTargets.Add(target);
                }
            }

            foreach (Transform target in filteredTargets)
            {
                float sqrDistanceToCenter = (target.position - from.position).sqrMagnitude;
                if (sqrDistanceToCenter < minSqrDistance)
                {
                    minSqrDistance = sqrDistanceToCenter;
                    nearestInFieldOfView = target;
                }
            }

            return nearestInFieldOfView;
        }
        
        
        public static bool IsInFieldOfView(this Transform observer, Transform target, float fieldOfViewAngle)
        {
            // Verify that the target is in sight of the observer
            Vector3 toTarget = target.position - observer.position;
            float angle = Vector3.Angle(observer.forward, toTarget);

            if (angle <= fieldOfViewAngle * 0.5f)
            {
                return true;
            }

            // Target is out of field of view or obscured
            return false;
        }
        
        public static bool IsInFieldOfView(this Transform observer, Transform target, float fieldOfViewAngle, bool checkObstacles, LayerMask layerMask)
        {
            // Verify that the target is in sight of the observer
            Vector3 toTarget = target.position - observer.position;
            float angle = Vector3.Angle(observer.forward, toTarget);

            if (angle <= fieldOfViewAngle * 0.5f)
            {
                if (checkObstacles)
                {
                    // Calculate the distance between the observer and the target
                    float distance = toTarget.magnitude;

                    // Check for obstructions in sight
                    RaycastHit hit;
                    if (Physics.Raycast(observer.position, toTarget, out hit, distance, layerMask))
                    {
                        if (hit.transform == target)
                        {
                            // The target is in the field of view of the observer and unobstructed
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }

            // Target is out of field of view or obscured
            return false;
        }

        public static Transform[] FindChildrensByName(this Transform transform, string name) 
        { 
            return transform.GetComponentsInChildren<Transform>().Where(t => t.name == name).ToArray(); 
        } 
    
        public static Transform[] GetAllChild(this Transform transform) 
        { 
            return transform.GetComponentsInChildren<Transform>();
        } 
    
        public static List<Transform> GetAllChildByComponent<T>(this Transform transform) where T : UnityEngine.Object
        {
            List<Transform> temp2 = null;
            foreach (Transform item in transform.GetAllChild())
            {
                if (item.GetComponent<T>())
                    temp2.Add(item);
            }

            return temp2;
        }
    
        // public static Vector3 SetVectorFromAngle (float x,  float y,  float z)  
        // {
        //     var rotation = Quaternion.Euler(x, y, z);
        //     var forward = Vector3.forward * distFromObj;
        //     return forward * rotation;
        // }

        // private void GetVectorByAngle()
        // {
        //     var rotation = Quaternion.AngleAxis(90, Vector3.up);
        //     var forward = Vector3.forward;
        //     var right = forward * rotation;
        // }  
    
        public static void SetPositionAndRotation(this Transform transform,Transform newTransform)
        {
            transform.position = newTransform.position;
            transform.rotation = newTransform.rotation;
        }
  
    
        public static bool CompareQuaternion(Quaternion q1, Quaternion q2, float precision)
        {
            return Mathf.Abs(Quaternion.Dot(q1, q2)) >= 1 - precision;
        }
        public static bool CompareDirections(Vector3 first , Vector3 second, float tolerance)
        {
            return !(Vector3.Angle(second, first) > tolerance);
        }
        public static bool GetDirOfRotation(Quaternion from, Quaternion to)
        {
            float fromY = from.eulerAngles.y;
            float toY = to.eulerAngles.y;
            float clockWise = 0f;
            float counterClockWise = 0f;

            //right
            if (fromY <= toY)
            {
                clockWise = toY - fromY;
                counterClockWise = fromY + (360 - toY);
            }
            else
            {
                clockWise = (360 - fromY) + toY;
                counterClockWise = fromY - toY;
            }

            return (clockWise <= counterClockWise);
        }
    
        public static bool Contain(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    
        private const float GIZMO_DISK_THICKNESS = 0.01f;
        public static void DrawGizmoDisk(this Transform t, float radius)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
            Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
            Gizmos.DrawSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
        }
        public static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = color;
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }
        
        public static bool ArrayContains<T>(this T[] thisArray, T searchElement)
        {
            // If you want this to find "null" values, you could change the code here
            return Array.Exists<T>(thisArray, x => x.Equals(searchElement));
        }
        

    }
    

    public static class ConeCastExtension
    {
        public static RaycastHit[] ConeCastAll(this Physics physics, Vector3 origin, float maxRadius, Vector3 direction, float maxDistance, float coneAngle,LayerMask mask)
        {
            Gizmos.color = Color.blue;
            RaycastHit[] sphereCastHits = Physics.SphereCastAll(origin , maxRadius, direction, maxDistance, mask);

        
            List<RaycastHit> coneCastHitList = new List<RaycastHit>();
        
            if (sphereCastHits.Length > 0)
            {
                for (int i = 0; i < sphereCastHits.Length; i++)
                {
                    // sphereCastHits[i].collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    Vector3 hitPoint = sphereCastHits[i].point;
                    Vector3 directionToHit = hitPoint - origin;
                    float angleToHit = Vector3.Angle(direction, directionToHit);

                    if (angleToHit < coneAngle)
                    {
                        coneCastHitList.Add(sphereCastHits[i]);
                    }
                }
            }

            RaycastHit[] coneCastHits = new RaycastHit[coneCastHitList.Count];
            coneCastHits = coneCastHitList.ToArray();

            return coneCastHits;
        }
    }
}