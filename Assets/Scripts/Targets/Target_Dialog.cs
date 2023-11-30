using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Dialog : Target
{
    public DialogType dialog = DialogType.Treasure;

    protected override void Awake()
    {
        base.Awake();
        //canHurt = false;
        showText = false;
        randomShape = false;
    }

    protected override void getDestroy()
    {
        // 不能立刻弹出，而是做一个队列，在回合结束后，依次弹出
        DialogManager.Instance.OnPopDialog(dialog);
        base.getDestroy();
    }
}
