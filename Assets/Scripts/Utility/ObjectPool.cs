// 用例：public ObjectPool<Ball> PoolOfBalls = new ObjectPool<Ball>(32);  Ball ball = ObjectPoolsManager.Instance.PoolOfBalls.New();
// this.enabled = false; 来判断是否已经存入了对象池
// 对于默认物体，如Text、粒子系统等 使用ObjectPool_DefaultT来创建prefab
// TODO 对于一个父类的多个子类，如何建立对象池？
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResetable<T> where T : MonoBehaviour, IResetable<T>, new()
{
    // 此接口的问题在于，初始化时无法传递参数
    void onPopObj(ObjectPool<T> pool);
    void onStroeObj();
}

public class ObjectPool<T> where T : MonoBehaviour, IResetable<T>, new()
{
    public Stack<T> m_objectStack;

    private T m_prefab;
    private Transform m_parent;
    private Action<T> m_onPopAction;

    public ObjectPool(int size)
    {
        m_objectStack = new Stack<T>(size);
    }

    public void OnSetData(T prefab, Transform parent, Action<T> onPopAction = null)
    {
        m_prefab = prefab;
        m_parent = parent;
        m_onPopAction = onPopAction;
    }

    public T New(Action<T> m_onNewAction = null)
    {
        T t;
        if (m_objectStack.Count > 0)
        {
            t = m_objectStack.Pop();
        }
        else
        {
            if (m_prefab && m_parent)
            {
                t = GameObject.Instantiate<T>(m_prefab, m_parent);
            }
            else
            {
                t = new T();
            }

        }
        t.gameObject.SetActive(true);
        t.enabled = true;           // 一般使用enabled来判断这个对象是否进入了对象池
        t.onPopObj(this);               // 此函数受接口限制无法传参
        m_onPopAction?.Invoke(t);   // 若对象池初始化时传入了action，则调用，用于带参数的统一初始化
        // 若New函数传入了action，则调用，用于带参数的特殊初始化
        // 但是，这种带参的action其实可以在New函数后直接执行，这种其实不是很必要并且会造成性能消耗
        m_onNewAction?.Invoke(t);   
        return t;
    }

    public void Store(T obj)
    {
        if (obj == null) return;
        if (!obj.enabled)
        {
            // 这个对象已经被禁用了，怀疑是已经进入了对象池
            Debug.LogError("obj for objectpool has been set enabled as false");
            return;
        }
        obj.onStroeObj();
        m_objectStack.Push(obj);
        obj.enabled = false;
        obj.gameObject.SetActive(false);
    }
}
