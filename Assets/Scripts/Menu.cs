using UnityEngine;
using DG;
using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    public CanvasGroup menuScreen;
    public CanvasGroup gameScreen;
    public CanvasGroup levelScreen;
    public CanvasGroup shopScreen;
    public CanvasGroup settingsScreen;

    public CanvasGroup loadingScreen;
    public CanvasGroup swapAnimation;

    public RectTransform playButton;
    public RectTransform levelButton;
    public RectTransform shopButton;
    public RectTransform settingsButton;

    public RectTransform titleImage;
    public RectTransform logoButtton;

    public RectTransform[] menuBlocks;

    public MenuUtility menuUtility;

    public void Start()
    {
        menuUtility = new();
        menuUtility.Instance = this;
        menuUtility.PlayMenuIntro();
    }

    public async void ContinueButton()
    {
        menuScreen.interactable = false;
        loadingScreen.gameObject.SetActive(true);

        await loadingScreen.DOFade(1,1.25f)
            .AsyncWaitForCompletion();

        menuScreen.alpha = 0;
        gameScreen.alpha = 1;

        await LevelLoader.LevelLoadingTask();

        await loadingScreen.DOFade(0,1.25f)
            .AsyncWaitForCompletion();
        
        loadingScreen.gameObject.SetActive(false);
    }

    public async void LevelSelectButton()
    {
        menuScreen.interactable = false;
        swapAnimation.gameObject.SetActive(true);

        await swapAnimation.DOFade(1,0.65f)
            .AsyncWaitForCompletion();

        await Awaitable.WaitForSecondsAsync(1f);

        menuScreen.alpha = 0;
        levelScreen.alpha = 1;

        await swapAnimation.DOFade(0,0.65f)
            .AsyncWaitForCompletion();
        
        swapAnimation.gameObject.SetActive(false);
    }

    public async void ShopButton()
    {
        menuScreen.interactable = false;
        swapAnimation.gameObject.SetActive(true);

        await swapAnimation.DOFade(1,0.65f)
            .AsyncWaitForCompletion();

        await Awaitable.WaitForSecondsAsync(1f);

        menuScreen.alpha = 0;
        shopScreen.alpha = 1;

        await swapAnimation.DOFade(0,0.65f)
            .AsyncWaitForCompletion();
        
        swapAnimation.gameObject.SetActive(false);
    }

    public async void SettingsButton()
    {
        menuScreen.interactable = false;
        swapAnimation.gameObject.SetActive(true);

        await swapAnimation.DOFade(1,0.65f)
            .AsyncWaitForCompletion();

        await Awaitable.WaitForSecondsAsync(1f);

        menuScreen.alpha = 0;
        settingsScreen.alpha = 1;

        await swapAnimation.DOFade(0,0.65f)
            .AsyncWaitForCompletion();
        
        swapAnimation.gameObject.SetActive(false);       
    }
}

public class MenuUtility
{
    public Menu Instance;

    public Tween FloatRect(RectTransform rect,float strength = 30,float loopTimer = 5, Ease easing = Ease.InOutSine)
    {
        rect.DOKill(true);
        var center = rect.anchoredPosition;

        Tween t = rect.DOAnchorPosY(center.y + strength / 2,loopTimer / 2)
            .SetLoops(-1,LoopType.Yoyo)
            .SetEase(easing);
        
        return t;
    }

    public Tween MoveRect(RectTransform rect,Vector2 to,float time, Ease ease = Ease.InOutSine)
    {
        rect.DOKill(true);
        Tween t = rect.DOAnchorPos(to,time,false)
            .SetEase(ease);
        
        return t;
    }

    public Tween OnButtonHit(RectTransform rect,float time,float strength,int shakeCount)
    {
        rect.DOKill(true);
        Tween t = rect.DOShakeRotation(time,strength,shakeCount);

        return t;
    }

    public Tween RandomYoyo(RectTransform rect,float maxLoopTimer = 30,float minLoopTimer = 10,float maxStrength = 60)
    {
        rect.DOKill(false);

        float originY = rect.anchoredPosition.y;

        float strength = Random.Range(-maxStrength, maxStrength);
        float duration = Random.Range(minLoopTimer, maxLoopTimer);

        var tween = rect
            .DOAnchorPosY(originY + strength, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        float randomStartTime = Random.Range(0f, duration * 2f);
        tween.Goto(randomStartTime, true);

        return tween;
    }

    public void PlayMenuIntro()
    {
        Tween pla,sho,lev,set,tit,log;

        pla = MoveRect(Instance.playButton,Instance.playButton.anchoredPosition + new Vector2(150,0),2.45f,Ease.OutSine);
        pla.OnComplete(()=> FloatRect(Instance.playButton));

        sho = MoveRect(Instance.shopButton,Instance.shopButton.anchoredPosition + new Vector2(150,0),3f,Ease.OutSine);
        sho.OnComplete(()=> FloatRect(Instance.shopButton));

        lev = MoveRect(Instance.levelButton,Instance.levelButton.anchoredPosition + new Vector2(-150,0),2f,Ease.OutSine);
        lev.OnComplete(()=> FloatRect(Instance.levelButton));

        set = MoveRect(Instance.settingsButton,Instance.settingsButton.anchoredPosition + new Vector2(-150,0),3.4f,Ease.OutSine);
        set.OnComplete(()=> FloatRect(Instance.settingsButton));

        tit = MoveRect(Instance.titleImage,Instance.titleImage.anchoredPosition + new Vector2(0,-600),3f,Ease.OutSine);
        tit.OnComplete(()=> FloatRect(Instance.titleImage));

        log = FloatRect(Instance.logoButtton);

        foreach (RectTransform item in Instance.menuBlocks)
            RandomYoyo(item);
    }
}
