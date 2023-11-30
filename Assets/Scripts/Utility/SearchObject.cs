/*
 * 功能描述:  查找一个物体的子物体，孙子物体
 * 示例：     
 * 更新于2020-01-23
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchObject : MonoBehaviour
{
    /// <summary>
    /// 从父物体找到第一个符合名字的，无论激活与否的子孙transform
    /// </summary>
    public static Transform FindChild(Transform FatherTrans, string childName)
    {
        Transform child = FatherTrans.Find(childName);
        if (child != null)
            return child;
        Transform go = null;

        for (int i = 0; i < FatherTrans.childCount; i++)
        {
            child = FatherTrans.GetChild(i);
            go = FindChild(child, childName);
            if (go != null)
                return go;
        }
        return null;
    }

    /// <summary>
    /// 从父物体找到所有符合名字的，无论激活与否的子孙transform（未测试!
    /// </summary>
    public static List<Transform> FindChilds(Transform FatherTrans, string childName)
    {
        List<Transform> childs = new List<Transform>();

        for (int i = 0; i < FatherTrans.childCount; i++)
        {
            Transform child = FatherTrans.GetChild(i);
            if (child.name == childName)
            {
                childs.Add(child);
            }
            List<Transform> grandChilds = FindChilds(child, childName);
            foreach (Transform a in grandChilds)
            {
                childs.Add(a);
            }
        }
        return childs;
    }

    /// <summary>
    /// Returns the first active loaded object of Type type.
    /// </summary>
    public static T FindObject<T>() where T : Object
    {
        var trans = FindObjectOfType(typeof(T));
        return trans as T;
    }

    /// <summary>
    /// Returns all the  active loaded object of Type type.
    /// </summary>
    public static T[] FindObjects<T>() where T : Object
    {
        T[] trans = FindObjectsOfType(typeof(T)) as T[];
        return trans;
    }

    /// <summary>
    /// Returns one active GameObject tagged tag
    /// </summary>
    public static GameObject FindObjectByTag(string tag)
    {
        GameObject obj = GameObject.FindWithTag(tag);
        return obj;
    }

    /// <summary>
    /// Returns all the active GameObject tagged tag
    /// </summary>
    public static GameObject[] FindObjectsByTag(string tag)
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag(tag);
        return obj;
    }
}
