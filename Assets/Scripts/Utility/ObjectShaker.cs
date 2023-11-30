/**
 * 功能描述:  物体抖动，可在抖动时设置某个坐标轴依然可以移动，不固定
 * 示例：     ObjectShaker.Instance.SetShake(transform, transform.position, Vector2.one, 0.1f,  0.3f, Vector3.one, 0.95f);
 * 示例：     ObjectShaker.Instance.SetShake(transform, transform.position, Vector2.one, 1f, 6, 0.3f, Vector3.one, 0.95f);//四叶草振动：幅度推荐1，频率推荐6
 * 脚本依赖： 需要OwnMathf脚本协助数学运算、SingleTon脚本保证单例
 * 更新于19-12-19
*/

using System.Collections.Generic;
using UnityEngine;

public class ObjectShaker : SingleTon<ObjectShaker>
{
    List<Transform> transformListToShake = new List<Transform>();
    List<ShakeData> shakeDatas = new List<ShakeData>();
    List<Vector3> toInheritList = new List<Vector3>();

    Vector3 vector3Delta;

    //考虑到正常的设置物体抖动计算常常在Update中更新，所以在LateUpdate中更新抖动，防止冲突
    void LateUpdate()
    {
        float delta = Time.deltaTime;
        float x, y, z;
        for (int i = 0; i < transformListToShake.Count; i++)
        {
            //震动往往是受到伤害，此物体是否被摧毁？
            if (!transformListToShake[i])
            {
                transformListToShake.RemoveAt(i);
                shakeDatas.RemoveAt(i);
                toInheritList.RemoveAt(i);
                continue;
            }

            vector3Delta = shakeDatas[i].GetVector3(delta);

            x = (toInheritList[i].x == 1 ? shakeDatas[i].vector3_Center.x : transformListToShake[i].position.x) + vector3Delta.x;
            y = (toInheritList[i].y == 1 ? shakeDatas[i].vector3_Center.y : transformListToShake[i].position.y) + vector3Delta.y;
            z = (toInheritList[i].z == 1 ? shakeDatas[i].vector3_Center.z : transformListToShake[i].position.z) + vector3Delta.z;

            transformListToShake[i].position = new Vector3(x, y, z);
            if (shakeDatas[i].isOver)
            {
                SetBackToCenter(i);
                transformListToShake.RemoveAt(i);
                shakeDatas.RemoveAt(i);
                toInheritList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 指定向量进行随机震动
    /// </summary>
    /// <param name="transformToShake">将要震动的物体</param>
    /// <param name="center">物体的中心点</param>
    /// <param name="max">震动向量最大值</param>
    /// <param name="min">震动向量最小值，各项自动约束小于max</param>
    /// <param name="isLock">向量锁定，默认1,1,1.若2维震动则1,1,0 </param>
    /// <param name="time">震动的时间 </ param >
    /// <param name="isInherit">向量继承，是否在震动时锁死位置？震动时是否可以移动物体？默认1,1,1</param>
    /// <param name="weakRate">在震动时的削弱率，默认0.95</param>
    /// <returns></returns>
    public void SetShake(Transform transformToShake, Vector3 center, Vector3 max, Vector3 min, Vector3 isLock, float time, Vector3 isInherit, float weakRate = 0.95f)
    {
        //叠加时间
        if (transformListToShake.Contains(transformToShake))
        {
            int index = transformListToShake.IndexOf(transformToShake);
            shakeDatas[index].RefreshShakeData(center, max, min, isLock, time, weakRate);
            toInheritList[index] = isInherit;
        }
        //创建一个新的
        else
        {
            ShakeData shakeData = new ShakeData(center, max, min, isLock, time, weakRate);
            transformListToShake.Add(transformToShake);
            shakeDatas.Add(shakeData);
            toInheritList.Add(isInherit);
        }
    }
    /// <summary>
    /// 指定长度进行随机震动
    /// </summary>
    /// <param name="transformToShake">将要震动的物体</param>   
    /// <param name="center">物体的中心点</param>
    /// <param name="isLock">向量锁定，默认1,1,1.若2维震动则1,1,0 </param>
    /// <param name="length">震动的长度</param>
    /// <param name="time">震动的时间 </ param >
    /// <param name="isInherit">向量继承，是否在震动时锁死位置？震动时是否可以移动物体？默认1,1,1即不可以移动</param>
    /// <param name="weakRate">在震动时的削弱率，默认0.95</param>
    /// <returns></returns>
    public void SetShake(Transform transformToShake, Vector3 center, Vector3 isLock, float length, float time, Vector3 isInherit, float weakRate = 0.95f)
    {
        if (transformListToShake.Contains(transformToShake))
        {
            //叠加时间
            int index = transformListToShake.IndexOf(transformToShake);
            shakeDatas[index].RefreshShakeData(center, isLock, length, time, weakRate);
            toInheritList[index] = isInherit;
        }
        else
        {
            //创建一个新的
            ShakeData shakeData = new ShakeData(center, isLock, length, time, weakRate);
            transformListToShake.Add(transformToShake);
            shakeDatas.Add(shakeData);
            toInheritList.Add(isInherit);
        }
    }
    /// <summary>
    /// 指定长度进行四叶草震动
    /// </summary>
    /// <param name="transformToShake">将要震动的物体</param>   
    /// <param name="center">物体的中心点</param>
    /// <param name="isLock">向量锁定，默认1,1,1.若2维震动则1,1,0 </param>
    /// <param name="length">震动的长度</param>
    /// <param name="shakeFrequence">震动的频率，默认6</param>
    /// <param name="time">震动的时间 </ param >
    /// <param name="isInherit">向量继承，是否在震动时锁死位置？震动时是否可以移动物体？默认1,1,1即不可以移动</param>
    /// <param name="weakRate">在震动时的削弱率，默认0.95</param>
    /// <returns></returns>
    public void SetShake(Transform transformToShake, Vector3 center, Vector3 isLock, float length, float shakeFrequence, float time, Vector3 isInherit, float weakRate = 0.95f)
    {
        if (transformListToShake.Contains(transformToShake))
        {
            //叠加时间
            int index = transformListToShake.IndexOf(transformToShake);
            shakeDatas[index].RefreshShakeData(center, isLock, length, shakeFrequence, time, weakRate);
            toInheritList[index] = isInherit;
        }
        else
        {
            //创建一个新的
            ShakeData shakeData = new ShakeData(center, isLock, length, shakeFrequence, time, weakRate);
            transformListToShake.Add(transformToShake);
            shakeDatas.Add(shakeData);
            toInheritList.Add(isInherit);
        }
    }


    private void SetBackToCenter(int index)
    {
        transformListToShake[index].position = shakeDatas[index].vector3_Center;
    }
}

public class ShakeData
{
    public Vector3 vector3_Center;

    public bool isOver
    {
        get
        {
            return timelength <= 0;
        }
    }

    private Vector3 vector3_Min;
    private Vector3 vector3_Max;
    private Vector3 isLock;

    private float timelength;
    private float weakRate;
    private float totalLength;
    private float shakeFrequence;//振动频率

    private int mode;
    private float x, y, z;

    public ShakeData(Vector3 _vector3_Center, Vector3 _vector3_Max, Vector3 _vector3_Min, Vector3 _isLock, float _timeLength, float _weakRate)
    {
        vector3_Center = _vector3_Center;
        vector3_Max = _vector3_Max;
        vector3_Min = _vector3_Min;
        isLock = _isLock;
        timelength = _timeLength;
        weakRate = _weakRate;
        mode = 1;
        Detect();
    }
    public ShakeData(Vector3 _vector3_Center, Vector3 _isLock, float _length, float _timeLength, float _weakRate)
    {
        vector3_Center = _vector3_Center;
        isLock = _isLock;
        totalLength = _length;
        timelength = _timeLength;
        mode = 2;
        weakRate = _weakRate;
        Detect();
    }
    public ShakeData(Vector3 _vector3_Center, Vector3 _isLock, float _length, float _shakeFrequence, float _timeLength, float _weakRate)
    {
        vector3_Center = _vector3_Center;
        isLock = _isLock;
        totalLength = _length;
        shakeFrequence = _shakeFrequence;
        timelength = _timeLength;
        mode = 3;
        weakRate = _weakRate;
        Detect();
    }

    //时间叠加，其他数据替换 在震动时进行时间叠加，可能会导致中心点错位，暂时注释_vector3_Center相关代码
    public void RefreshShakeData(Vector3 _vector3_Center, Vector3 _vector3_Max, Vector3 _vector3_Min, Vector3 _isLock, float _timeLength, float _weakRate)
    {
        //vector3_Center = _vector3_Center;
        vector3_Max = _vector3_Max;
        vector3_Min = _vector3_Min;
        isLock = _isLock;
        timelength += _timeLength;

        mode = 1;
        weakRate = _weakRate;
        Detect();
    }
    public void RefreshShakeData(Vector3 _vector3_Center, Vector3 _isLock, float _length, float _timeLength, float _weakRate)
    {
        //vector3_Center = _vector3_Center;
        isLock = _isLock;
        totalLength = _length;
        timelength += _timeLength;
        mode = 2;
        weakRate = _weakRate;
        Detect();
    }
    public void RefreshShakeData(Vector3 _vector3_Center, Vector3 _isLock, float _length, float _shakeFrequence, float _timeLength, float _weakRate)
    {
        //vector3_Center = _vector3_Center;
        isLock = _isLock;
        totalLength = _length;
        shakeFrequence = _shakeFrequence;
        timelength += _timeLength;
        mode = 3;
        weakRate = _weakRate;
        Detect();
    }

    //防止min的值大于max的值而引发bug
    private void Detect()
    {
        switch (mode)
        {
            case 1:
                if (vector3_Min.x > vector3_Max.x)
                {
                    vector3_Min.x = vector3_Max.x;
                }
                if (vector3_Min.x > vector3_Max.x)
                {
                    vector3_Min.y = vector3_Max.y;
                }
                if (vector3_Min.y > vector3_Max.y)
                {
                    vector3_Min.y = vector3_Max.y;
                }
                break;
            case 2:
            case 3:
                if (totalLength < 0)
                {
                    totalLength = 0;
                }
                break;
            default:
                Debug.LogWarning("【震动模式出错】");
                break;
        }
        if (timelength < 0)
        {
            timelength = 0;
        }
        weakRate = Mathf.Clamp(weakRate, 0, 1);
    }

    public Vector3 GetVector3(float deltaTime)
    {
        Vector3 vector3_Result = Vector3.zero;

        switch (mode)
        {
            #region 根据两个向量进行随机振动
            case 1:
                x = Random.Range(vector3_Min.x, vector3_Max.x) * isLock.x;
                y = Random.Range(vector3_Min.y, vector3_Max.y) * isLock.y;
                z = Random.Range(vector3_Min.z, vector3_Max.z) * isLock.z;
                vector3_Result = new Vector3(x, y, z);

                vector3_Min *= weakRate;
                vector3_Max *= weakRate;
                break;
            #endregion
            #region 根据长度进行随机振动
            case 2:
                List<float> xyz = OwnMathf.GetRandomRangeByHash(0, 1, 3);
                vector3_Result = new Vector3(
                    xyz[0] * isLock.x,
                    xyz[1] * isLock.y,
                    xyz[2] * isLock.z);
                vector3_Result = vector3_Result.normalized * totalLength;

                totalLength *= weakRate;
                break;
            #endregion
            #region 根据长度与幅度进行四叶草振动
            case 3:
                vector3_Result.x = 
                    Mathf.Sin(timelength * shakeFrequence * Mathf.PI) * 
                    Mathf.Cos(timelength * shakeFrequence * Mathf.PI) * 
                    Mathf.Cos(timelength * shakeFrequence * Mathf.PI) * 
                    isLock.x;
                vector3_Result.y = 
                    Mathf.Sin(timelength * shakeFrequence * Mathf.PI) * 
                    Mathf.Sin(timelength * shakeFrequence * Mathf.PI) * 
                    Mathf.Cos(timelength * shakeFrequence * Mathf.PI) * 
                    isLock.y;
                vector3_Result.z = 0 * isLock.z;
                vector3_Result *= totalLength * Mathf.Atan(timelength);

                totalLength *= weakRate;
                break;
            #endregion
            default:
                Debug.LogWarning("【震动模式出错】");
                break;
        }

        timelength -= deltaTime;

        return vector3_Result;
    }
}
