using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjManager : SingleTon<StaticObjManager>
{
    public static Transform mark_store;
    public static Transform mark_back_L;
    public static Transform mark_back_R;

    public static List<Dialog> dialogs = new List<Dialog>();

    void Awake()
    {
        mark_store = GameObject.Find("Mark_StoreBall").transform;
        mark_back_L = GameObject.Find("Mark_Back_L").transform;
        mark_back_R = GameObject.Find("Mark_Back_R").transform;

        for (int i = 0; i < (int)DialogType.tail; i++)
        {
            Dialog dialog = GameObject.Find("Dialog_" + ((DialogType)i).ToString()).GetComponent<Dialog>();
            dialog.Hide();
            dialogs.Add(dialog);
        }
    }

    void Update()
    {
        
    }
}
