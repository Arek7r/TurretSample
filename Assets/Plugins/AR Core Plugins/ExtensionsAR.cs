using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LayersExt
{
    public static readonly int Default_INT = LayerMask.NameToLayer("Default");
    public static readonly int Track_INT = LayerMask.NameToLayer("Track");
    public static readonly int Bonus_INT = LayerMask.NameToLayer("Bonus");
  
    public static readonly int Default_Bit = 1 << LayerMask.NameToLayer("Default");
    public static readonly int Enemy = 1 << LayerMask.NameToLayer("Enemy");
    public static readonly int Track_Bit = 1 << LayerMask.NameToLayer("Track");
    public static readonly int SlowMotion = 1 << LayerMask.NameToLayer("SlowMotion");
    
    public const string DefaultStr = "Default";
    public const string Track = "Track";
    public const string BonusStr = "Bonus";
    public const string vPart = "vPart";
    public const string Attacker = "Attacker";
    public const string Obstacle = "Obstacle";
    public const string Scenery = "Scenery";
    public const string Vehicle = "Vehicle";
    public const string DestroPart = "DestroPart";
}


public static class Ext
{
    public static Transform GetClosest(this Transform trans, List<Transform> list)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = trans.position;
        foreach (Transform potentialTarget in list)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
    /// <summary>
    /// Looking on ALL GO on ACTIVE scenes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> Find<T>()
    {
        List<T> interfaces = new List<T>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
            foreach (var childInterface in childrenInterfaces)
            {
                interfaces.Add(childInterface);
            }
        }

        return interfaces;
    }
/// <summary>
/// Looking type only on TOP root GO on all loaded scenes
/// </summary>
/// <typeparam name="T"></typeparam>
/// <returns></returns>
    public static List<T> FindOnTopMultiScenes<T>()
    {
        List<T> types = new List<T>();
        var scenes = GetAllLoadedScenes();

        foreach (var scene in scenes)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                T[] components = rootGameObject.GetComponents<T>();
                foreach (var childInterface in components)
                {
                    types.Add(childInterface);
                }
            }
        }

        return types;
    }

    public static List<Scene> GetAllLoadedScenes()
    {
        List<Scene> scenes = new List<Scene>();
        for (int n = 0; n < SceneManager.sceneCount; ++n)
        {
            scenes.Add(SceneManager.GetSceneAt(n));
        }

        return scenes;
    }

    public static List<T> FindOnTop<T>()
    {
        List<T> interfaces = new List<T>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
            T[] components = rootGameObject.GetComponents<T>();
            foreach (var childInterface in components)
            {
                interfaces.Add(childInterface);
            }
        }

        return interfaces;
    }

    //Looking on all MonoBehaviour
    public static T FindOfType<T>() where T : class
    {
        foreach (var monoBehaviour in GameObject.FindObjectsOfType<MonoBehaviour>())
        {
            if (monoBehaviour is T)
            {
                return monoBehaviour as T;
            }
        }

        return default(T);
    }
    
    public static void SetY(this Vector3 transform, float valueY)
    {
        transform.Set(transform.x, valueY, transform.z);
    }
    public static void SetX(this Vector3 transform, float valueX)
    {
        transform.Set(valueX, transform.y, transform.z);
    }
    
    public static void SetNewPos(this Transform transform, float newX, float newY, float newZ)
    {
        transform.position = new Vector3(newX, newY, newZ);
    }
    
    public static void SetY(this Quaternion quaternion, float valueY)
    {
        quaternion.Set(quaternion.x, valueY, quaternion.z, quaternion.w);
    }
    
    public static Quaternion EulerY(this Quaternion quaternion, float valueY)
    {
        return Quaternion.Euler(quaternion.x, valueY, quaternion.z);
    }
    
    public static Quaternion SetRotationY(this Quaternion quaternion, float yAngle)
    {
        Vector3 euler = quaternion.eulerAngles;
        euler.y = yAngle;
        return Quaternion.Euler(euler);
    }
    
    public static void SetNewPosRot(this Transform transform, Transform from)
    {
        transform.position = from.position;
        transform.rotation = from.rotation;
    }
    
    public static void SetNewPosRotScale(this Transform transform, Transform from)
    {
        transform.position = from.position;
        transform.rotation = from.rotation;
        transform.localScale = from.localScale;
    }

    public static void Reset(this Rigidbody rb)
    {
        //to reset inside values
        rb.isKinematic = !rb.isKinematic;
        rb.isKinematic = !rb.isKinematic;

        //to reset inside values
        rb.useGravity = !rb.useGravity;
        rb.useGravity = !rb.useGravity;

        rb.AddForce(Vector3.zero);
        rb.drag = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //rb.inertiaTensor = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
        rb.centerOfMass = Vector3.zero;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
    }
    public static void Reset(this Rigidbody rb, bool isKinematic = true, bool useGravity = true)
    {
        rb.isKinematic = isKinematic;
        rb.useGravity = useGravity;

        rb.AddForce(Vector3.zero);
        rb.drag = 0;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //rb.inertiaTensor = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
        rb.centerOfMass = Vector3.zero;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
    }
    
    public static void Random(this ref Vector3 myVector, Vector3 min, Vector3 max)
    {
        myVector = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }
    
     public static float RoundToDecimalPlace(this float number, int decimalPlace)
    {
        float decimalDivider = 10.0f;
        for (int i = 1; i < decimalPlace; i++)
        {
            decimalDivider *= 10.0f;
        }

        return Mathf.Round(number * decimalDivider) / decimalDivider;
    }

    public static Vector3 RoundToDecimalPlace(this Vector3 vector, int decimalPlace)
    {
        float decimalDivider = 10.0f;
        for (int i = 1; i < decimalPlace; i++)
        {
            decimalDivider *= 10.0f;
        }

        var x = Mathf.Round(vector.x * decimalDivider) / decimalDivider;
        var y = Mathf.Round(vector.y * decimalDivider) / decimalDivider;
        var z = Mathf.Round(vector.z * decimalDivider) / decimalDivider;
        return new Vector3(x, y, z);
    }

    public static Vector3 ZeroY(this Vector3 vector)
    {
        return new Vector3(vector.x, 0.0f, vector.z);
    }

    public static Vector3 ZeroZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0.0f);
    }

    public static Vector3 ZeroX(this Vector3 vector)
    {
        return new Vector3(0.0f, vector.y, vector.z);
    }

    public static bool CompareLayer(this Collider collider, int layerID)
    {
        return collider.gameObject.layer == layerID;
    }

    public static bool CompareLayer(this Collision collision, int layerID)
    {
        return collision.gameObject.layer == layerID;
    }

    public static bool CompareLayer(this Transform transform, int layerID)
    {
        return transform.gameObject.layer == layerID;
    }

    public static bool CurrentStateIsTag(this Animator animator, int layer, int hash)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).tagHash == hash;
    }
    
    public static bool CurrentStateIsTag(this Animator animator, int hash)
    {
        return animator.GetCurrentAnimatorStateInfo(AnimatorLayers.BaseLayer).tagHash == hash;
    }

    public static bool NextStateIsTag(this Animator animator, int layer, int hash)
    {
        return animator.GetNextAnimatorStateInfo(layer).tagHash == hash;
    }

    public static bool CurrentOrNextStateIsTag(this Animator animator, int layer, int hash)
    {
        return CurrentStateIsTag(animator, layer, hash) || NextStateIsTag(animator, layer, hash);
    }
    
    public static void LookAtY(this Transform transform, Vector3 point)
    {
        var lookPos = point - transform.position;
        lookPos.y = 0;

        if (lookPos == Vector3.zero)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }
    
    public static bool IsObjectVisible(this UnityEngine.Camera @this, Renderer renderer)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(@this), renderer.bounds);
    }
    
    public static bool TargetIsInBack(Transform owner,  Transform target)
    {
        Vector3 targetDir = target.position - owner.transform.position;
        Vector3 forward = owner.transform.forward;
        var angle = -Vector3.SignedAngle(targetDir, forward, Vector3.up);
        return Math.Abs(angle) > 30;
    }

    public static bool TargetIsInBack2(Transform owner,  Transform target)
    {
        Vector3 directionToTarget = owner.position - target.position;
        float angel = Vector3.Angle(owner.forward, directionToTarget);
        
        if (Mathf.Abs(angel) > 30)
            return true;
        else
            return false;
    }
    
    public static bool IsBetween(float testValue, float minValue, float maxValue)
    {
        if (testValue > minValue && testValue <= maxValue)
            return true;

        return false;
    }
    private static void TargetIsFront(Transform transform, Transform target)
    {
        Vector3 toTarget = (target.position - transform.position).normalized;

        if (Vector3.Dot(toTarget, transform.forward) > 0)
        {
            Debug.Log("Target is in front of this game object.");
        }
        else
        {
            Debug.Log("Target is not in front of this game object.");
        }
    }

    private static bool IsLookingAtObject(Transform looker, Vector3 targetPos, float FOVAngle)    {
 
        Vector3 direction = targetPos - looker.position;
        float ang = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float lookerAngle = looker.eulerAngles.z;
        float checkAngle = 0f;
 
        if (ang >= 0f )
            checkAngle = ang - lookerAngle - 90f;
        else if (ang < 0f )
            checkAngle = ang - lookerAngle + 270f;
         
        if (checkAngle < -180f )
            checkAngle = checkAngle  + 360f;
 
        if (checkAngle <= FOVAngle*.5f && checkAngle >= -FOVAngle*.5f)
            return true;
        else
            return false;
    }
    
    
    public static Texture2D CreateSolidTexture2D(Color color)
    {
        var texture = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(color, 1 * 1).ToArray();
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
    {
        T item = list[index];
        for (int i = index; i > 0; i--)
            list[i] = list[i - 1];
        list[0] = item;
    }
    
    public static void MoveItemAtIndexToLast<T>(this List<T> list, int index)
    {
        T item = list[index];
        
        for (int i = index; i < list.Count; i++)
            list[i] = list[i + 1];

        list[list.Count] = item;
    }
    
    
    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        var item = list[oldIndex];

        list.RemoveAt(oldIndex);

        if (newIndex > oldIndex) newIndex--;
        // the actual index could have shifted due to the removal

        list.Insert(newIndex, item);
    }

    public static void Move<T>(this List<T> list, T item, int newIndex)
    {
        if (item != null)
        {
            var oldIndex = list.IndexOf(item);
            if (oldIndex > -1)
            {
                list.RemoveAt(oldIndex);

                if (newIndex > oldIndex) newIndex--;
                // the actual index could have shifted due to the removal

                list.Insert(newIndex, item);
            }
        }

    }
    
    public static class AnimatorLayers
    {
        public static readonly int BaseLayer = 0;
    }
    
    public static bool IsBotTag(Transform tr)
    {
        return tr.CompareTag(GlobalString.Bot);
    }
    public static bool IsBotTag(Collision col)
    {
        return col.transform.CompareTag(GlobalString.Bot);
    }
    
    public static bool IsPlayerTag(Transform tr)
    {
        return tr.CompareTag(GlobalString.Player);
    }
    
    public static bool IsPlayerTag(Collision col)
    {
        return col.transform.CompareTag(GlobalString.Player);
    }
    
    public static bool IsPlayerOrBotTag(Transform tr)
    {
        if (tr.gameObject.layer == LayerMaskID.Vehicle)
            tr = tr.root;

        return tr.CompareTag(GlobalString.Player) || tr.CompareTag(GlobalString.Bot) || tr.CompareTag(GlobalString.Attacker);
    }
    public static bool IsPlayerOrBotTag(Collision col)
    {
        return col.transform.CompareTag(GlobalString.Player) || col.transform.CompareTag(GlobalString.Bot) || col.transform.CompareTag(GlobalString.Attacker);
    }

    public static bool CompareStrings(string str1, string str2)
    {
        if (string.CompareOrdinal(str1, str2) == 0)
        {
            return true;
        }

        return false;
    }

    public static class TaskUtils
    {
        public static async Task WaitUntil(Func<bool> predicate, int sleep = 50)
        {
            while (!predicate())
            {
                await Task.Delay(sleep);
            }
        }
        
        public static async Task WaitUntilFalse(Func<bool> predicate, int sleep = 50)
        {
            while (predicate())
            {
                await Task.Delay(sleep);
            }
        }
    }
}

