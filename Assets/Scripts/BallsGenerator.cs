using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallsGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;
    private LineRenderer line;
    private BoxCollider2D box2d;
    private ObjectPool_DefaultT obj_text;
    private Text text;
    public int MaxNum
    {
        get
        {
            return _maxNum;
        }
        set
        {
            if (value != _maxNum)
            {
                _maxNum = value;
                if (text != null)
                {
                    text.text = _maxNum.ToString();
                }
            }
        }
    }
    private int _maxNum = 0;
    public int OverNum = 0;
    public int NextMaxNum = 0;
    public float force = 8.5f;
    private Vector3 dir;
    private Dictionary<int, BallData> ballDatas = new Dictionary<int, BallData>();

    //private void Awake()
    //{
    //    obj_text = ObjectPoolsManager.Instance.PoolOfText_Health.New();
    //    text = obj_text.GetComponent<Text>();
    //    line = GameObject.Find("Line").GetComponent<LineRenderer>();
    //}

    void Start()
    {
        obj_text = ObjectPoolsManager.Instance.PoolOfText_Health.New();
        text = obj_text.GetComponent<Text>();
        line = GameObject.Find("Line").GetComponent<LineRenderer>();
        //text.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        text.transform.position = transform.position;
        line.SetPosition(0, transform.position);
        MaxNum = 3;
        NextMaxNum = MaxNum;
    }

    public void onNextTurn()
    {
        if (NextMaxNum > MaxNum)
        {
            MaxNum = NextMaxNum;
        }
    }

    public void onRefreshLine(Vector3 pos)
    {
        line.enabled = true;
        line.SetPosition(1, pos);
    }

    public void onShoot(Vector3 pos)
    {
        line.enabled = false;
        dir = pos - transform.position;
        dir.Normalize();
        dir = dir * force;
        StopAllCoroutines();
        StartCoroutine("Shoot");
    }

    // 对于所有balls，刚发射时state置为0，之后不断上升，被回收了之后会变为-1
    public bool onDetectBallsOk()
    {
        return OverNum >= MaxNum;
    }

    // 后续考虑做成枚举，每个属性有点东西
    public void onAddBallData(int index, int size = 0)
    {
        if (!ballDatas.ContainsKey(index))
        {
            BallData ballData = new BallData(index, size);
            ballDatas.Add(index, ballData);
        }
        else
        {
            ballDatas[index].size += size;
        }
    }

    IEnumerator Shoot()
    {
        OverNum = 0;
        for (int i = 0; i < MaxNum; i++)
        {
            Ball ball = ObjectPoolsManager.Instance.PoolOfBalls.New();
            ball.onShoot(dir, transform.position, i, this);
            if (ballDatas.ContainsKey(i))
            {
                // 设置小球的数据，如大小威力爆炸等等
                ball.onSetBallData(ballDatas[i]);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }
}

// 存储小球数据的稀疏列表类
// 当发射器分裂的时候，会平分此列表
public class BallData
{
    public int index = 1;
    public int size = 1;

    public BallData(int _index, int _size)
    {
        index = _index;
        size = _size;
    }
}