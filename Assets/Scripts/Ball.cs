using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour, IResetable<Ball>
{
    public int size = 1;
    public int state = -1;
    public BallType ballType = BallType.normal;

    private int index = 0;
    private bool movingUp = false;
    private bool movingBack = false;
    private Vector2 originalDir;
    private Vector3 originalPos;
    private Rigidbody2D rbody2D;
    private BallsGenerator CurBg;
    private CircleCollider2D cc2D;
    private ObjectPool<Ball> m_pool;

    private static Vector3 localScale = new Vector3(1.7f, 1.7f, 1.7f);

    private Transform mark_store;
    private Transform mark_back_L;
    private Transform mark_back_R;

    private void Awake()
    {
        rbody2D = GetComponent<Rigidbody2D>();
        cc2D = GetComponent<CircleCollider2D>();
        mark_store = StaticObjManager.mark_store;
        mark_back_L = StaticObjManager.mark_back_L;
        mark_back_R = StaticObjManager.mark_back_R;
    }

    private void Update()
    {
        if (movingUp)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 13);
            if (transform.position.y > mark_store.position.y)
            {
                ObjectPoolsManager.Instance.PoolOfBalls.Store(this);
            }
            return;
        }
        if (movingBack)
        {
            // 防止直接落在底部，停住了
            transform.Translate((transform.position.x > 0 ? 1 : -1) * Vector3.right * Time.deltaTime * 0.05f);
        }
        if (transform.position.x < mark_back_L.position.x || transform.position.x > mark_back_R.position.x)
        {
            onBack();
        }
    }

    // TODO 这里的多种情况，要不要考虑用观察者模式event？
    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.gameObject.tag;
        switch(tag)
        {
            // 暂时不考虑用碰撞做返回，而是检测每帧的小球x轴位置
            //case "back":
            //    onBack();
            //    break;
            case "refall":
                // 重新掉落 这样可能会导致死循环 —— 如果state小于等于0，那么是发射出来就直接回收了，就不回收直接消失
                if (state > 0)
                {
                    state = -1;
                    movingUp = false;
                    rbody2D.gravityScale = 0f;
                    rbody2D.Sleep();
                    onShoot(originalDir, originalPos);
                }
                else
                {
                    ObjectPoolsManager.Instance.PoolOfBalls.Store(this);
                }
                break;
            case "Target":
                Target target = other.GetComponentInParent<Target>();
                target.onBallCollision(size, index);
                state += 1;
                break;
        }
        // colorPicker = Random.Range(0, 10);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.gameObject;
        string tag = obj.tag;
        switch(tag)
        {
            case "Target":
                Target target = obj.GetComponentInParent<Target>();
                // 若有爆炸等，则在此处调用自己的onBallCollision
                if (target)
                {
                    target.onBallCollision(size, index);
                    state += 1;
                }
                else
                {
                    Debug.LogWarning("can not get target component");
                    print(obj.name);
                }
                break;
            case "Down":
                //  触底，TODO 考虑播放音效，改变颜色、透明度
                rbody2D.velocity = rbody2D.velocity / 3;
                movingBack = true;
                break;
            default:
                return;
        }
        rbody2D.gravityScale = 1f;
    }

    // 正式发射
    public void onShoot(Vector2 dir, Vector3 pos, int _index = -1, BallsGenerator bg = null)
    {
        rbody2D.AddForce(dir, ForceMode2D.Impulse);
        transform.position = pos;
        originalPos = pos;
        originalDir = dir;
        if (_index >= 0)
        {
            index = _index;
        }
        if (bg != null)
        {
            CurBg = bg;
        }
    }

    public void onSetBallData(BallData ballData)
    {
        if (ballData != null)
        {
            size = ballData.size + 1;
            float scaleX = localScale.x + size * 0.1f;
            transform.localScale = new Vector3(scaleX, scaleX, scaleX);
        }
    }

    // 向上移动，回收
    private void onBack()
    {
        state = -1;         // 如果state置为-1，则表示已经被回收   这个设定暂时没用了，现在用CurNum来判断是否被回收
        cc2D.isTrigger = true;
        //rbody2D.simulated = false;
        movingUp = true;
        movingBack = false;
        rbody2D.Sleep();
        rbody2D.velocity = Vector2.zero;
        rbody2D.gravityScale = 0f;
        //rbody2D.AddForce(Vector2.up * 15, ForceMode2D.Impulse);
    }

    // 发射前的状态重置，由对象池自动调用
    public void onPopObj(ObjectPool<Ball> pool)
    {
        m_pool = pool;
        state = 0;
        cc2D.isTrigger = false;
        rbody2D.gravityScale = 0f;
        rbody2D.Sleep();
        //rbody2D.simulated = true;
    }
    // 回收的状态重置，停止其渲染与物理模拟，节省性能，由对象池自动调用
    public void onStroeObj()
    {
        state = -1;
        if (CurBg != null)
        {
            CurBg.OverNum += 1;
            CurBg = null;   // 清除引用，防止可能的内存泄漏
        }
        GameManager.Instance.onCollision();     // 检测是否可以下一回合
        movingUp = false;
        movingBack = false;
        transform.localScale = localScale;
    }
}

public enum BallType
{
    normal,
    boom,
}