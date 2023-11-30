using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPoolsManager : SingleTon<ObjectPoolsManager>
{
    [Header("小球")]
    public Transform ball_parent;
    public Ball ball_prefab;
    public ObjectPool<Ball> PoolOfBalls = new ObjectPool<Ball>(32);
    [Header("生命Text")]
    public Transform text_health_parent;
    public ObjectPool_DefaultT text_health_prefab;
    public ObjectPool<ObjectPool_DefaultT> PoolOfText_Health = new ObjectPool<ObjectPool_DefaultT>(20);
    [Header("目标")]
    public Transform target_parent;
    //public Target target_prefab;
    public List<Target> target_prefab_list;
    private int[] targetPoolNum = new int[4] { 20, 5, 5, 5 };
    public List<ObjectPool<Target>> PoolOfTargets_List = new List<ObjectPool<Target>>();
    [Header("UITips")]
    public Transform tips_parent;
    public ObjectPool_DefaultT tipsSmall_prefab;
    public ObjectPool<ObjectPool_DefaultT> PoolOfText_SmallTips = new ObjectPool<ObjectPool_DefaultT>(2);

    // 建4个对象池Target，组成一个数组，赋值给TargetManager

    private void Awake()
    {
        PoolOfBalls.OnSetData(ball_prefab, ball_parent);
        //PoolOfTargets.OnSetData(target_prefab, target_parent);
        for (int i = 0; i < target_prefab_list.Count; i++)
        {
            ObjectPool<Target> pool = new ObjectPool<Target>(targetPoolNum[i]);
            pool.OnSetData(target_prefab_list[i], target_parent);
            PoolOfTargets_List.Add(pool);
        }
        PoolOfText_Health.OnSetData(text_health_prefab, text_health_parent);
        PoolOfText_SmallTips.OnSetData(tipsSmall_prefab, tips_parent);
    }

}
