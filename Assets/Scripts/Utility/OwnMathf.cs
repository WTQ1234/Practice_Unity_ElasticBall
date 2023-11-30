using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnMathf : MonoBehaviour
{
    /// <summary>
    /// 将一个浮点数保留小数点后指定位数，默认两位
    /// </summary>
    /// <param name="f">数值</param>
    /// <param name="n">结束点</param>
    /// <returns></returns>
    public static float GetNPointFloat(float f, int n = 2)
    {
        int multiple = 1;
        for(int i = 0; i < n; i++)
        {
            multiple *= 10;
        }
        f = (float)(Mathf.RoundToInt(f * multiple));
        f = f / (float)multiple;
        return f;
    }

    /// <summary>
    /// 获取两点之间距离一定百分比的一个点
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">结束点</param>
    /// <param name="percent">起始点到目标点距离百分比</param>
    /// <returns></returns>
    public static Vector3 GetPointBetweenPoint(Vector3 start, Vector3 end, float percent)
    {
        Vector3 normal = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        return normal * (distance * percent) + start;
    }
    public static Vector2 GetPointBetweenPoint(Vector2 start, Vector2 end, float percent)
    {
        Vector2 normal = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        return normal * (distance * percent) + start;
    }

    /// <summary>
    /// 根据哈希表产生不同随机数，用于解决同一帧下相同随机数的问题
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="count">列表数量</param>
    /// <returns></returns>
    public static List<float> GetRandomRangeByHash(float min, float max, int count, List<float> ListFloat = null)
    {
        List<float> vs = ListFloat;     // 传入一个默认为null的list，这样可以不用在循环中每次都创建新的list
        if (vs == null)
        {
            vs = new List<float>();
        }
        vs.Clear();
        Hashtable hashtable = new Hashtable();
        System.Random rm = new System.Random();

        for (int i = 0; hashtable.Count < count; i++)
        {
            float nValue = (float)rm.NextDouble() * (max - min) + min;//取一个0-1之间的随机数，映射至min和max之间
            if (!hashtable.ContainsValue(nValue))
            {
                hashtable.Add(nValue, nValue);
                vs.Add(nValue);
            }
        }

        return vs;
    }

    /// <summary>
    /// 向量卷乘：三个点依次分别相乘：如乘0,1,1.用于保留指定轴的位移坐标
    /// </summary>
    /// <param name="vector31">起始点</param>
    /// <param name="vector32">结束点</param>
    /// <returns></returns>
    public static Vector3 VectorMultipliedPointToPoint(Vector3 vector31, Vector3 vector32)
    {
        return new Vector3(vector31.x * vector32.x, vector31.y * vector32.y, vector31.z * vector32.z);
    }
    public static Vector2 VectorMultipliedPointToPoint(Vector2 vector31, Vector2 vector32)
    {
        return new Vector2(vector31.x * vector32.x, vector31.y * vector32.y);
    }
}
