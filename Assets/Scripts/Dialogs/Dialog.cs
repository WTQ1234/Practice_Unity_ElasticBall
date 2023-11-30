using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dialog : MonoBehaviour
{
    //public bool btnRemove = false;

    protected Text text_title;
    protected Button btn_next;
    protected CanvasGroup canvasGroup;

    public void Awake()
    {
        //if (btnRemove)
        //{
        //    EventTrigger eventTrigger = transform.Find("bg").GetComponent<EventTrigger>();
        //    EventTrigger.Entry entry = new EventTrigger.Entry();
        //    entry.eventID = EventTriggerType.PointerClick;
        //    entry.callback = new EventTrigger.TriggerEvent();
        //    entry.callback.AddListener(Hide);
        //    eventTrigger.triggers.Add(entry);
        //}
        text_title = transform.Find("Panel/text_title").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        btn_next = transform.Find("Panel/btn_next").GetComponent<Button>();
        btn_next.onClick.AddListener(Hide);
    }

    public virtual void Pop(BaseEventData pointData = null)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        DialogManager.Instance.OnCloseDialog();
    }
}
