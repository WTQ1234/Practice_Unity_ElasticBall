using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    [SerializeField]
    private Text text_score;
    [SerializeField]
    private Text text_coin;
    [SerializeField]
    private Image image_fast;
    [Header("按钮")]
    [SerializeField]
    private Button btn_skip;

    private void Awake()
    {
        OnSetScore(0);
        OnSetCoin(0);
    }

    public void OnSetScore(int score)
    {
        text_score.text = "Score:" + score.ToString();
    }

    public void OnSetCoin(int coin)
    {
        text_coin.text = "Coin:" + coin.ToString();
    }

    public void onSetFast(bool isfast)
    {
        image_fast.gameObject.SetActive(isfast);
    }

    public void onClickBtn_Skip()
    {
        GameManager.Instance.onAddCoin(5);
        GameManager.Instance.onAddScore(5);
        GameManager.Instance.onNextTurn();
    }

    public void onPopTips(string str, Vector2 pos)
    {
        Transform tips = ObjectPoolsManager.Instance.PoolOfText_SmallTips.New().transform;
        Text text = SearchObject.FindChild(tips, "Text").GetComponent<Text>();
        text.text = str;
        tips.transform.position = Camera.main.WorldToScreenPoint(pos);
    }

    public void onSetBtnClickable(bool canClick = true)
    {
        btn_skip.interactable = canClick;
    }
}
