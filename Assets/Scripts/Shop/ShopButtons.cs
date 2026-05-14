using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public int Amount;
    private TextMeshProUGUI priceText;
    private TextMeshProUGUI amountText;
    private Button button;

    private void Awake()
    {
        amountText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        priceText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void OnProductFetched(Product product)
    {
        print($"Product Fetch - {Amount}");
        amountText.text = $"{Amount} <sprite name=icons_0>";
        priceText.text = product.metadata.localizedPriceString;
    }

    public void OnProductFetchFailed(ProductDefinition p,string s)
    {
        print($"Product Fetch fail - {Amount}");
        button.interactable = false;
    }

    public void OnPurchaseFetched(Order order)
    {
        print($"Purchase Fetch - {Amount}");
    }

    public void OnPurchaseFailed(FailedOrder f)
    {
        print($"Purchase Fetch failed - {Amount}");
        button.interactable = true;
    }

    public void OnOrderPending(PendingOrder p)
    {
        print($"Order pending - {Amount}");
        button.interactable = false;
    }

    public void OnOrderConfirm(ConfirmedOrder c)
    {
        print($"Order confirm - {Amount}");
        SaveStateHandler.MakePurchase(Amount);
        button.interactable = true;
    }

    public void OnOrderDeferred(DeferredOrder d)
    {
        print($"Order defer - {Amount}");
    }
}