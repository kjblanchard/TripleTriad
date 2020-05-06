using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using E7.Introloop;

public class SoundManager : MonoBehaviour
{


    public static SoundManager instance;



    public AudioSource[] SFX;
    public AudioSource[] BGM;
    //public AudioClip SFXTest;

    public IntroloopAudio[] allIntroloopAudio;

    public int bgmToPlay;



    private void Awake() //we only want one of these
    {
        instance = this;
        int soundmanagerCount = FindObjectsOfType<SoundManager>().Length;
        if(soundmanagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBgm(int musicToPlay)
    {
        StopBGM();
        BGM[musicToPlay].Play();
    } 

    public void PlaySFX(int soundToPlay)
    {
        SFX[soundToPlay].Play();
    }


    public void StopBGM()
    {

        for (int i = 0; i < BGM.Length; i++)
        {
            BGM[i].Stop();
        }
        // for ( int i = 0; i < allIntroloopAudio.Length; i++)
        // {
        //     IntroloopPlayer.Instance.Stop(i);
        // }
    }

    public void PlayIntroLoop(int musicToPlay)
    {
        IntroloopPlayer.Instance.Play(allIntroloopAudio[musicToPlay]);
    }

    public void StopIntroLoop()
    {
        IntroloopPlayer.Instance.Stop();
    }

    public void CacheNextMusic(int musicToCache)
    {
        IntroloopPlayer.Instance.Preload(allIntroloopAudio[musicToCache]);
    }


}
