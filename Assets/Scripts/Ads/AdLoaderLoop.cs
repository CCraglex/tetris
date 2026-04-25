
using UnityEngine;

public class AdLoaderLoop : MonoBehaviour
{
    private float last;
    private void Update()
    {
        if(Time.time - 60 > last)
        {
            last += 60;
            AdService.RefillAds();
        }       
    }
}