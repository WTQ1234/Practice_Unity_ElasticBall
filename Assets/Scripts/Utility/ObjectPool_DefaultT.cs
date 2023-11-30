using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 如果是不需要实现接口，不需要new函数的，就使用这个默认的类，然后挂载在prefab字段上
public class ObjectPool_DefaultT : MonoBehaviour, IResetable<ObjectPool_DefaultT>
{
    ObjectPool<ObjectPool_DefaultT> m_pool;
    public void onPopObj(ObjectPool<ObjectPool_DefaultT> pool)
    {
        m_pool = pool;
    }
    public void onStroeObj()
    {
 
    }

    // 此函数可以被外在调用，用于禁用自己，重返对象池，如点击函数、动画事件等
    public void onReStore()
    {
        m_pool.Store(this);
    }
}
