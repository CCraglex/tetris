using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class SoundService
{
    private static Queue<AudioSource> Channels;

    public static Transform ChannelParent;
    private static SoundCoroutineHandler SoundCoroutineHandler;

    public static void Init(Transform Parent,int StartChannelCount = 3)
    {
        ChannelParent = Parent;
        SoundCoroutineHandler = ChannelParent.gameObject.AddComponent<SoundCoroutineHandler>();
        CreateChannels(StartChannelCount);
    }

    private static void CreateChannels(int Amount)
    {
        for (int i = 0; i < Amount; i++)
            AddChannel();
    }

    public static void AddChannel()
    {
        GameObject G = Object.Instantiate(new GameObject("Sound Channel"),ChannelParent);
        Channels.Enqueue(G.AddComponent<AudioSource>());        
    }

    public static void RemoveAllChannels()
    {
        foreach (Transform t in ChannelParent)
        {
            if(t == ChannelParent)
                continue;
            
            Object.Destroy(t.gameObject);
        }

        SoundCoroutineHandler.StopAllCoroutines();
        Channels = new();
    }

    public static void PlaySound(AudioClip Clip)
    {
        static IEnumerator WaitCompletion(AudioSource res)
        {
            yield return new WaitWhile(() => res.isPlaying);
            Channels.Enqueue(res);
        }

        if (!Channels.TryDequeue(out var res))
        {
            AddChannel();
            Channels.TryDequeue(out res);
        }

        res.clip = Clip;
        res.Play();

        SoundCoroutineHandler.StartCoroutine(WaitCompletion(res));
    }
}
