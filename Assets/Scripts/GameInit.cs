using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private GPGSAuth gpgsAuth;
    [SerializeField] private SaveInitializer saveInitializer;
    [SerializeField] private LevelGameplay levelGameplay;
    [SerializeField] private Menu menu;
    [SerializeField] private CanvasGroup loadScreen;
    [SerializeField] private CoinHandler coinHandler;
    [SerializeField] private LevelSelectContent levelSelectContent;

    [SerializeField] Transform soundParent;
    [SerializeField] AdService adHandler;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        SoundService.Init(soundParent);
        AdService.Init(adHandler);

        loadScreen.alpha = 1;
        StartCoroutine(InitSecondary());
    }

    private IEnumerator InitSecondary()
    {
        var t = Time.time;
        float minTimer = 1;

        #if !UNITY_EDITOR
        gpgsAuth.Auth();
        yield return new WaitUntil(() => gpgsAuth.authActionCompleted == true);
        #endif

        print("Loading save...");
        yield return saveInitializer.LoadSave();
        print("Save Loaded");
        
        coinHandler.Init();
        menu.InitMenu();
        levelGameplay.Init();
        levelSelectContent.Init();

        if(Time.time - t < minTimer)
            yield return new WaitForSeconds(minTimer - t);

        yield return loadScreen.DOFade(0,0.45f)
            .OnComplete(() => loadScreen.blocksRaycasts = false);
        
        print("Done");
    }
}
