using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 时序：先执行Awake函数，然后执行onPopObj函数，然后执行传入的闭包,最后执行Start函数
public class Target : MonoBehaviour, IResetable<Target>
{
    #region health
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value != _health)
            {
                if (value >= health) _healthMax = value;    // 用于医疗兵影响的加分
                _health = value;
                if (overRideText == text_noOverRide && text != null)
                {
                    text.text = _health.ToString();
                }
            }
        }
    }
    [SerializeField]
    private int _health = 0;
    protected int _healthMax = 10;
    #endregion

    private Camera mainCamera;
    private Text text;
    private ObjectPool_DefaultT obj_text;
    private SpriteRenderer sprite;
    private int m_shape = 1;
    private int _typeIndex = 0;     // 好像没啥用
    private ObjectPool<Target> m_pool;
    // 种类：   1.不同的兵种：普通、医疗兵、史莱姆（分裂）、磁铁？、绞肉机（按size进行分裂弹出）、boss 、消逝（数回合后自动死亡，可被击退）、凝滞（吃掉一颗球，死的时候掉两颗球）
    //          2.不同的道具：加数量，加威力，加金币，加道具（药水？消消乐道具？  摆正：将碰撞摆正）     属于trigger
    //          3.打击后特殊事件：商店、宝箱、随机事件（商店：特殊子弹：每5轮一次激光）  属于碰撞体，会发生特殊事件的，即销毁后会弹出弹窗——传入指定的物体名字来决定弹哪个
    // 那我怎么区别不同的种类呢？用子类？那多一个子类就会多一个prefab，或者根据枚举动态添加不同的类？或者根据枚举调用不同的函数？
    // 同时，不同的兵种是有不同的图、颜色、震动表现、声音 那最好还是做成prefab
    protected bool canHurt = true;      // 是否伤害到玩家
    protected bool showText = true;     // 是否以字符显示生命值
    protected bool randomShape = true;
    public string overRideText = text_noOverRide;

    private static string tag_damage = "TargetDamage";
    private static string text_noOverRide = "";

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
    }

    // 升到顶的时候销毁
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canHurt) return;
        string tag = other.gameObject.tag;
        if (tag == tag_damage)
        {
            GameManager.Instance.onHurt();
            getDestroy();
        }
    }

    #region 接口
    public void onPopObj(ObjectPool<Target> pool)
    {
        m_pool = pool;
        if (text == null && showText)
        {
            obj_text = ObjectPoolsManager.Instance.PoolOfText_Health.New();
            text = obj_text.GetComponent<Text>();
            if (overRideText != text_noOverRide)
            {
                text.text = overRideText;
            }
        }
        if (randomShape)
        {
            m_shape = Random.Range(1, transform.childCount);
            Transform t = transform.GetChild(m_shape);
            t.gameObject.SetActive(true);
            sprite = t.GetComponent<SpriteRenderer>();
        }
    }
    public void onStroeObj()
    {
        if (randomShape)
        {
            Transform t = transform.GetChild(m_shape);
            t.gameObject.SetActive(false);
            sprite = null;
        }
        if (showText)
        {
            ObjectPoolsManager.Instance.PoolOfText_Health.Store(obj_text);
            obj_text = null;
            text = null;
        }
        TargetManager.Instance.onStoreTarget(this);
    }
    #endregion

    #region 初始化设置
    public void onSetTextPos()
    {
        if (showText)
        {
            text.transform.position = transform.position;
        }
    }

    public void onSetPos(float x, float y)
    {
        transform.localPosition = new Vector3(x, y, 0);
        onSetTextPos();
    }

    // 暂时没法分类的数据就暂时放这里
    public void onSetData(int helathMax, int typeIndex)
    {
        _health = 0;
        _healthMax = helathMax;
        _typeIndex = typeIndex;
    }

    public virtual void onSetHealth(int healthPoint, bool isMax = true)
    {
        if (isMax)
        {
            // 初始化使用
            health = _healthMax + healthPoint;
        }
        else
        {
            // 医疗兵
            health += healthPoint;
        }
    }

    public void onSetTarget(float a, float b, Color[] colors)
    {
        onSetRot((b - 0.5f) * 45);
        onSetHealth((int)((a - 0.7f) * _healthMax));
        if (sprite != null)
        {
            sprite.color = colors[Mathf.Clamp(health / 30, 0, colors.Length)];
        }
    }

    private void onSetRot(float z)
    {
        transform.Rotate(new Vector3(0, 0, z));
    }
    #endregion

    public virtual void onBallCollision(int size, int index, BallsGenerator bg = null)
    {
        getDamage(size);
    }

    public void getDamage(int damage)
    {
        health -= damage;
        if (text != null)
        {
            text.text = health.ToString();
        }
        if (health <= 0)
        {
            getDestroy();
        }
        else
        {
            ObjectShaker.Instance.SetShake(transform, transform.position, Vector2.one, 0.5f, 5, 0.3f, Vector3.one, 0.95f);
        }
    }

    protected virtual void getDestroy()
    {
        // TODO 考虑爆炸特效
        // 想办法防止被destroy两次
        GameManager.Instance.onAddScore(_healthMax);
        m_pool.Store(this);
    }
}

public enum Shape
{
    box,
}
