using System.Collections;
using TMPro;
using UnityEngine;

public class RewardedTest : MonoBehaviour
{
    [SerializeField] GameObject coinReward;
    [SerializeField] GameObject reviveReward;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        AdService.instance.rewardedCoin.OnRewardedAction += () => coinReward.SetActive(true);
        AdService.instance.rewardedRevive.OnRewardedAction += () => reviveReward.SetActive(true);
    }
    public void DemandCoin()
    {
        AdService.ShowRewardedCoin();
    }

    public void DemandRevive()
    {
        AdService.ShowRewardedRevive();
    }
}
