using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Trigger : Target
{
    public int addNum;
    public int addSize;
    public int addCoin;

    protected override void Awake()
    {
        base.Awake();
        canHurt = false;
        //showText = false;
        randomShape = false;
    }

    public override void onSetHealth(int healthPoint, bool isMax = true)
    {
        health = 1;
        _healthMax = 1;
    }

    public override void onBallCollision(int size, int index, BallsGenerator bg = null)
    {
        if (addNum > 0)
        {
            // 随机一个发射器，增加其数量
            UIManager.Instance.onPopTips("Ball Number +" + addNum.ToString(), transform.position);
            if (bg == null)
            {
                bg = GameManager.Instance.onGetRandomGenerator();
            }
            bg.NextMaxNum += addNum;
        }
        if (addSize > 0)
        {
            if (bg == null)
            {
                bg = GameManager.Instance.onGetRandomGenerator();
            }
            bg.onAddBallData(index, addSize);
            UIManager.Instance.onPopTips("威力+" + addSize.ToString(), transform.position);
        }
        if (addCoin > 0)
        {
            GameManager.Instance.onAddCoin(addCoin);
            UIManager.Instance.onPopTips("金币+" + addCoin.ToString(), transform.position);
        }
        getDestroy();
    }
}
