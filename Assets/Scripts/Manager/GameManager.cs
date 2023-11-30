using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class GameManager : SingleTon<GameManager>
{
    public List<BallsGenerator> ballsGenerators = new List<BallsGenerator>();
    public int Score = 0;
    public int Coin = 0;
    public int Health = 1;
    public GameObject GuideRender;

    private Camera mainCamera;
    private Vector3 inputPos;
    private Vector3 mousePos;
    private bool canTouch = true;
    private bool isPointing = false;
    private static string tag_pick = "PickArea";

    void Start()
    {
        mainCamera = Camera.main;

        // TODO 新游戏和存档功能的区别
        onNextTurn();
    }

    void Update()
    {
        if (!canTouch) return;
        if (Input.GetMouseButton(0))
        {
            inputPos = Input.mousePosition;
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(inputPos), Vector2.zero);
            if (hit && hit.collider != null && hit.collider.tag == tag_pick)
            {
                isPointing = true;
                mousePos = mainCamera.ScreenToWorldPoint(inputPos);
                mousePos.z = 0;
                onRefresh();
            }
        }
        if (Input.GetMouseButtonUp(0) && isPointing)
        {
            GuideRender.SetActive(false);
            onShoot();
            isPointing = false;
        }
    }

    private void onShoot()
    {
        canTouch = false;
        UIManager.Instance.onSetBtnClickable(false);
        StartCoroutine("ChangeTimeScale");
        for (int i = 0; i < ballsGenerators.Count; i++)
        {
            BallsGenerator bg = ballsGenerators[i];
            bg.onShoot(mousePos);
        }
    }

    private void onRefresh()
    {
        for (int i = 0; i < ballsGenerators.Count; i++)
        {
            BallsGenerator bg = ballsGenerators[i];
            bg.onRefreshLine(mousePos);
        }
    }

    public void onNextTurn()
    {
        TargetManager.Instance.onNextTurn();
        Time.timeScale = 1f;
        StopAllCoroutines();
        UIManager.Instance.onSetFast(false);
        UIManager.Instance.onSetBtnClickable(true);
        foreach(var item in ballsGenerators)
        {
            item.onNextTurn();
        }
    }

    public void onAddScore(int _score)
    {
        Score += _score;
        UIManager.Instance.OnSetScore(Score);
    }

    public void onAddCoin(int _coin)
    {
        Coin += _coin;
        UIManager.Instance.OnSetCoin(Coin);
    }

    public void onCollision()
    {
        foreach (var item in ballsGenerators)
        {
            if (!item.onDetectBallsOk())
            {
                return;
            }
        }

        // 如果有Dialog弹出
        if (DialogManager.Instance.OnNextTurn())
        {
            return;
        }
        onNextTurn();
    }

    public void onHurt()
    {
        Health -= 1;
        // TODO 是否要做一些扣血后的处理，比如全屏爆炸之类的，现在的设定是只有一滴血
        if (Health <= 0)
        {
            print("Game Over!");
        }
    }

    public void canMove()
    {
        canTouch = true;
    }

    public BallsGenerator onGetRandomGenerator()
    {
        //for (int i = 0; i < )
        int i = Random.Range(0, ballsGenerators.Count);
        return ballsGenerators[i];
    }

    IEnumerator ChangeTimeScale()
    {
        yield return new WaitForSeconds(5f);
        Time.timeScale = 2f;
        UIManager.Instance.onSetFast(true);
        yield return new WaitForSeconds(3f);
        Time.timeScale = 3f;
    }
}
