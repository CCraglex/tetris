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
        Channels = new();
        ChannelParent = Parent;
        SoundCoroutineHandler = ChannelParent.gameObject.AddComponent<SoundCoroutineHandler>();
        CreateChannels(StartChannelCount);
    }

    public static void PauseAll()
    {
        if(Channels == null || Channels.Count == 0)
            return;
            
        foreach (var channel in Channels)
        {
            if(channel.isPlaying)
                channel.Pause();   
        }
    }

    public static void ResumeAll()
    {
        if(Channels == null || Channels.Count == 0)
            return;

        foreach (var channel in Channels)
        {
            if(channel.isPlaying)
                channel.UnPause();   
        }
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

    public static void PlaySound(AudioClip Clip,float volume = 1,float pitchLow = 0,float pitchHigh = 0)
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

        Debug.Log(volume);
        res.volume = volume;
        res.pitch = pitchLow != 0 || pitchHigh != 0 ? Random.Range(1 + pitchLow,1 + pitchHigh) : 1;
        res.clip = Clip;
        res.spatialBlend = 0f;
        res.Play();

        SoundCoroutineHandler.StartCoroutine(WaitCompletion(res));
    }
}
