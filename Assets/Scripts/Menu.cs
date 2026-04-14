using UnityEngine;
using DG;
using DG.Tweening;
using System.Collections;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    [Header("Screens")]
    public CanvasGroup menuScreen;
    public CanvasGroup gameScreen;
    public CanvasGroup levelScreen;

    public CanvasGroup loadingScreen;
    public CanvasGroup swapAnimation;

    [Header("Buttons")]
    public RectTransform titleImage;
    public RectTransform logoButtton;

    public RectTransform[] menuBlocks;

    [Header("Data")]

    public MenuUtility menuUtility;
    private LevelUtility levelUtility;

    public AudioClip[] audios;

    [Header("Components")]
    public LevelLoadManager levelLoader;
    public Transform soundParent;


    public void Start()
    {
        Application.targetFrameRate = 120;
        SoundService.Init(soundParent);
        
        menuUtility = new();
        menuUtility.Instance = this;
        menuUtility.PlayMenuIntro();

        levelUtility = new();
        levelUtility.Instance = this;
    }

    public void ReturnToMenuFromLevelSelect()
    {
        IEnumerator IAction()
        {
            SoundService.PlaySound(audios[0]);
            yield return swapAnimation.DOFade(1, 0.65f).WaitForCompletion();
            yield return new WaitForSeconds(0.75f);
            levelScreen.alpha = 0;
            levelScreen.blocksRaycasts = false;

            menuScreen.alpha = 1;
            menuScreen.blocksRaycasts = true;
            yield return swapAnimation.DOFade(0, 0.65f).WaitForCompletion();            
        }

        StartCoroutine(IAction());
    }

    public void LevelButton(string level)
    {
        SoundService.PlaySound(audios[0]);
        levelUtility.OnLevelSelect(int.Parse(level));
    }

    public async void LevelSelectButton()
    {
        SoundService.PlaySound(audios[0]);
        swapAnimation.DOFade(1,0.25f);

        await swapAnimation.DOFade(1,0.65f)
            .AsyncWaitForCompletion();

        await Awaitable.WaitForSecondsAsync(0.75f);

        menuScreen.alpha = 0;
        menuScreen.blocksRaycasts = false;

        levelScreen.alpha = 1;
        levelScreen.blocksRaycasts = true;

        await swapAnimation.DOFade(0,0.65f)
            .AsyncWaitForCompletion();
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
        Tween tit,log;

        tit = MoveRect(Instance.titleImage,Instance.titleImage.anchoredPosition + new Vector2(0,-600),3f,Ease.OutSine);
        tit.OnComplete(()=> FloatRect(Instance.titleImage));

        log = FloatRect(Instance.logoButtton);

        foreach (RectTransform item in Instance.menuBlocks)
            RandomYoyo(item);
    }
}

public class LevelUtility
{
    public Menu Instance;

    public async void OnLevelSelect(int Level)
    {
        Instance.loadingScreen.gameObject.SetActive(true);
        await Instance.loadingScreen.DOFade(1,1.25f)
            .AsyncWaitForCompletion();

        Instance.levelScreen.alpha = 0;
        Instance.levelScreen.blocksRaycasts = false;

        Instance.gameScreen.alpha = 1;
        Instance.gameScreen.blocksRaycasts = true;

        await Instance.levelLoader.CreateLevel(Level);

        await Instance.loadingScreen.DOFade(0,1.25f)
            .AsyncWaitForCompletion();
        
        Instance.loadingScreen.gameObject.SetActive(false);
        Instance.levelLoader.ReadyLevel(Level);
    }
}