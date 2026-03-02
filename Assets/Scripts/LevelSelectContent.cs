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
    [SerializeField] private Transform levelContainer;
    [SerializeField] private GameObject levelButton;
    [SerializeField] private int sidePadding;
    [SerializeField] private int spacing;

    private IEnumerator Start()
    {
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(typeof(LevelSO));
        yield return handle;

        int levelCount = handle.Result.Count;
        Addressables.Release(handle);

        var rect = levelContainer as RectTransform;
        rect.GetComponent<GridLayoutGroup>().padding = new(sidePadding,sidePadding,30,30);
        float sqr = (rect.rect.width - (2 * sidePadding) - (spacing * 4)) / 5f;

        for (int i = 0; i < levelCount; i++)
        {
            GameObject ButtonGO = Instantiate(levelButton,levelContainer);
            var l = ButtonGO.GetComponentInChildren<LayoutElement>();
            l.minWidth = sqr;
            l.minHeight= sqr;
            ButtonGO.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
        }
    }
}
