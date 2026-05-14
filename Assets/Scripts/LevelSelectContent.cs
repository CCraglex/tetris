using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class LevelSelectContent : MonoBehaviour
{
    [SerializeField] Menu menu;

    [SerializeField] private Transform levelContainer;
    [SerializeField] private GameObject levelButton;

    private IEnumerator IAction()
    {
        AsyncOperationHandle<IList<IResourceLocation>> handle =
            Addressables.LoadResourceLocationsAsync("Level", typeof(LevelSO));
        yield return handle;

        int levelCount = handle.Result.Count;
        Addressables.Release(handle);

        var rect = levelContainer as RectTransform;
        var g = rect.GetComponent<GridLayoutGroup>();

        float totalWidth = rect.rect.width;
        float padding = g.padding.left + g.padding.right;
        float spacingX = g.spacing.x;
        int columns = g.constraintCount;

        float sqr = (totalWidth - (2 * padding) - (spacingX * 4)) / columns;
        g.cellSize = new(sqr,sqr);

        for (int i = 0; i < levelCount; i++)
        {
            GameObject ButtonGO = Instantiate(levelButton,levelContainer);
            string level = (i + 1).ToString();
            ButtonGO.GetComponentInChildren<TextMeshProUGUI>().text = level;
            var button = ButtonGO.GetComponent<Button>();
            button.onClick.AddListener(() => menu.LevelButton(level));
        }
        print($"Loaded {levelCount} levels.");
    }  

    public void Init()
        => StartCoroutine(IAction());

    public void UpdateLevels()
    {
        int currentSave = SaveStateHandler.GetMaxLevel() + 1;
        print(currentSave);

        for (int i = 0; i < levelContainer.childCount; i++)
        {
            var obj = levelContainer.GetChild(i);
            if(int.Parse(obj.GetComponentInChildren<TextMeshProUGUI>().text) <= currentSave)
                obj.GetComponent<Button>().interactable = true;
            else obj.GetComponent<Button>().interactable = false;
        }
    }
}