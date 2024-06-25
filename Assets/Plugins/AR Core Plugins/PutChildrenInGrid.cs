using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TriInspector;

public class PutChildrenInGrid : MonoBehaviour
{
    // How many objects in Row before a new column is started
    public int gridSize = 20;
    public float gapX = 10;
    public float gapY;
    public float gapZ;

    public Quaternion rotation;
    public float Random=0;

    // Called from Editor Script
    [Button]
    public void MoveChildrenIntoGrid()
    {
        // For each child gameobject
        for (int i = 0; i < transform.childCount; i++)
        {
            // Determine an X position based on Grid Size and Gap
            float x = (i - (Mathf.Abs(i / gridSize) * gridSize)) * gapX;
            float y = (i - (Mathf.Abs(i / gridSize) * gridSize)) * gapY;
            float z = (i - (Mathf.Abs(i / gridSize) * gridSize)) * gapZ;

            // Set the child's local position in a grid
            transform.GetChild(i).localPosition = new Vector3(x, y, z);
            transform.GetChild(i).localRotation = rotation;
        }
    }

    [Button]
    public void MoveChildrenIntoGrid2()
    {
        // For each child gameobject
        for (int i = 0; i < transform.childCount; i++)
        {
            // Determine an X position based on Grid Size and Gap
            float x = (i - (Mathf.Abs(i / gridSize) * gridSize)) * gapX + UnityEngine.Random.Range(-Random, Random);
            // Determine an Y position based on Grid Size and Gap
            float z = Mathf.Abs(i / gridSize) * gapZ+ UnityEngine.Random.Range(-Random, Random);

            float y = Mathf.Abs(i / gridSize) * gapY;

            // Set the child's local position in a grid
            transform.GetChild(i).localPosition = new Vector3(x, y, z) ;
            transform.GetChild(i).localRotation = rotation;
        }
    }

    List<Transform> x;


    [Button]
    public void SortByName()
    {
        x = new List<Transform>();
        x.Clear();
        int y = 0;

        foreach (Transform child in transform)
            x.Add(child);

        Transform[] x2 = x.ToArray();
       // x = x2.OrderBy(c => c.name.Length).ThenBy(c => c.name).ToList();
        x=  x2.OrderBy(x => x.name, new SemiNumericComparer()).ToList();
//        foreach (var thing in x2.OrderBy(x => x.name, new SemiNumericComparer()))
//        {
//            Debug.Log(thing);
//            
//        }
        
        foreach (Transform child in x)
        {
            child.SetSiblingIndex(y);
            y++;
        }
    }

    public void ActiveAllChildre ()
    {
         foreach (Transform child in transform)
                    child.gameObject.SetActive(true);
    }

//    [Button]
//    public void SortByName2()
//    {
//        string[] things = new string[] {"paul","paul1", "paul 1", "paul asd 1", "bob", "lauren", "007", "90", "101"};
//
//        foreach (var thing in things.OrderBy(x => x, new SemiNumericComparer()))
//        {
//            Debug.Log(thing);
//            
//        }
//    }

    public class SemiNumericComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            int res = Compare(s1, s2, true);
            return res;
        }

        public int Compare(string s1, string s2, bool toplevel)
        {
            int localRes = 0;
            //Purely Numeric Comparisons
            if (IsNumeric(s1) && IsNumeric(s2))
            {
                if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
            }

            string[] split1 = s1.Split(' ');
            string[] split2 = s2.Split(' ');
            //If we're dealing with words...might need to recursively call
            if (split1.Length > 1 || split2.Length > 1)
            {
                if (split1.Length == 1) //only one of these has words, and it must be split2
                {
                    localRes = Compare(s1, split2[0], false); //check s1 against the first word in s2
                    if (localRes == 0)
                    {
                        return -1;
                    } //If the single word == the first word in the other..then the single wins
                    else return localRes;
                }

                if (split2.Length == 1) //only one of these has words, and it must be split1 
                {
                    localRes = Compare(split1[0], s2, false); //check s2 against the first word in s1
                    if (localRes == 0)
                    {
                        return 1;
                    }
                    else return localRes;
                }

                //If the first word is the same..lets compare the rest
                if (split1[0] == split2[0])
                {
                    List<string> l1 = new List<string>(split1);
                    l1.RemoveAt(0);
                    List<string> l2 = new List<string>(split2);
                    l2.RemoveAt(0);

                    return Compare(String.Join(" ", l1), string.Join(" ", l2), false);
                }
            }


            if (IsNumeric(s1) && !IsNumeric(s2))
                return -1;

            if (!IsNumeric(s1) && IsNumeric(s2))
                return 1;

            return string.Compare(s1, s2, true);
        }
        
        
        public static bool IsNumeric(object value)
        {
            try
            {
                int i = Convert.ToInt32(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}