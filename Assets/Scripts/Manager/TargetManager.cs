using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : SingleTon<TargetManager>
{
    public int TurnNum = 0;
    public int TargetHealth = 10;
    public float TargetRandom = 0.2f;   // TODO 后续如果种类多，则考虑做成每个种类的概率数组   商店、宝箱、医疗兵、加成道具
    public float TargetMovingSpeed = 2f;
    private float initTargetRandom = 0.35f;
    private bool targetMovingUp = false;
    private float movingUp = 0f;
    private float curTargetY = 0f;
    private int curTargetNum = 0;   // 当前的数量，用于即时改变概率
    [SerializeField]
    private int curContinueNormalTargetNum = 0;    // 当前的连续特殊数量
    public float[] targetSepicalRandom; // 分别为 普通、好处、随机事件、特殊兵种
    public Color[] targetColors;
    public int[] targetSpeicalLimit;
    private Transform TargetParents;
    private float[] targetX = new float[7] { -1.8f, -1.2f, -0.6f, 0f, 0.6f, 1.2f, 1.8f };
    private List<float> cache_targetRandomList = new List<float>(14);
    private List<float> cache_RandomList = new List<float>(14);  // 0为生命，1为z轴旋转值

    private List<Target> targets = new List<Target>();
    private List<Target> cache_targets = new List<Target>();
    void Start()
    {
        TargetParents = GameObject.Find("Targets").transform;
    }

    void Update()
    {
        if (targetMovingUp)
        {
            movingUp -= Time.deltaTime * TargetMovingSpeed;
            TargetParents.transform.Translate(Vector3.up * Time.deltaTime * TargetMovingSpeed);
            // 刷新生命值文本位置
            foreach(var item in targets)
            {
                item.onSetTextPos();
            }
            if (movingUp <= 0f)
            {
                // 可以操作了
                GameManager.Instance.canMove();
                targetMovingUp = false;
            }
        }
    }

    public void onNextTurn()
    {
        TurnNum += 1;
        TargetHealth += (int)(TargetHealth / 50) + 1;   // 逐渐提高难度
        // 1.用一个sin函数简单控制概率
        // 2.动态概率：若场上的target小于5个，那么概率临时提高10%
        bool moreTarget = curTargetNum <= 5;
        TargetRandom = initTargetRandom + Mathf.Sin(TurnNum * 2) / 10 + (moreTarget ? 0.1f : 0f);  
        // 生成新target:1-7描述了target出现的概率，8-14描述了具体是哪一种target
        OwnMathf.GetRandomRangeByHash(0, 1, 14, cache_targetRandomList);
        cache_targets.Clear();
        for (int i = 0; i < targetX.Length; i++)
        {
            if (cache_targetRandomList[i] <= TargetRandom)
            {
                newTarget(i, cache_targetRandomList[i + 7]);
            }
            else if(cache_targets.Count == 0)
            {
                if (i == 6)
                {
                    newTarget(i, cache_targetRandomList[i + 7]);
                }
            }
        }

        // 1表示随机的生命系数，2表示随机的旋转量
        OwnMathf.GetRandomRangeByHash(0, 1, cache_targets.Count * 2, cache_RandomList);
        for(int i = 0; i < cache_targets.Count; i++)
        {
            cache_targets[i].onSetTarget(cache_RandomList[i * 2], cache_RandomList[i * 2 + 1], targetColors);
        }
        targetMovingUp = true;
        movingUp = 0.7f;
        curTargetY -= 0.7f;
    }

    private void newTarget(int index, float randomSpecial)
    {
        int targetIndex = 0;
        // 动态特殊球：若连续10个target都没有出特殊目标，那么一定会出一个特殊目标
        bool isSpecial = curContinueNormalTargetNum > 10;
        float random = isSpecial ? targetSepicalRandom[0] : 0;
        for (int i = isSpecial ? 1 : 0; i < targetSepicalRandom.Length; i++)
        {
            if (randomSpecial < random + targetSepicalRandom[i])
            {
                // 就是这个了
                targetIndex = i;
                break;
            }
            random += targetSepicalRandom[i];
        }
        if (targetSpeicalLimit[targetIndex] > TurnNum)
        {
            targetIndex = 0;
        }
        if (targetIndex > 0)
        {
            curContinueNormalTargetNum = 0;
        }
        else
        {
            curContinueNormalTargetNum += 1;
        }
        // 根据targetIndex确定子类或者prefab
        Target target = ObjectPoolsManager.Instance.PoolOfTargets_List[targetIndex].New(
            (t) => {
                // 传入闭包，用于设置位置
                t.onSetPos(targetX[index], curTargetY);
                t.onSetData(TargetHealth, targetIndex);
            });
        targets.Add(target);
        cache_targets.Add(target);
        curTargetNum += 1;
    }

    public void onStoreTarget(Target target)
    {
        targets.Remove(target);
        curTargetNum -= 1;
    }
}
