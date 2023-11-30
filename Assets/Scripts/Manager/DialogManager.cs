using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : SingleTon<DialogManager>
{
    public Stack<DialogType> stack_dialogType = new Stack<DialogType>();

    public void OnPopDialog(DialogType type)
    {
        // 将type写入栈中，等这一轮结束之后再把Dialog依次弹出
        // 只有商店是只弹一次的
        if (stack_dialogType.Contains(type) && type == DialogType.Store)
        {
            return;
        }
        stack_dialogType.Push(type);
    }

    public void OnCloseDialog()
    {
        if (stack_dialogType.Count > 0)
        {
            DialogType type = stack_dialogType.Peek();
            Dialog obj = StaticObjManager.dialogs[(int)type];
            obj.Pop();
            return;
        }
        GameManager.Instance.onNextTurn();
    }

    public bool OnNextTurn()
    {
        if (stack_dialogType.Count > 0)
        {
            DialogType type = stack_dialogType.Peek();
            Dialog obj = StaticObjManager.dialogs[(int)type];
            obj.Pop();
            return true;
        }
        return false;
    }
}

public enum DialogType
{
    Treasure,
    Store,
    Random,
    tail,
}
