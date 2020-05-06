using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    
    [Header("Joystick Handler")]
    public bool joystickEnabled;
    public GameObject joystickMaster;
    public GameObject[] regularJoystick;
    public GameObject directionalJoystick;
    public bool frameRateOf60;
    public bool frameRateOf120;

    [Header("FadeControl")]
    public bool shouldFade;

    [Header("Played Timer")]
    public float totalPlayedTime;


    private void Awake()
    {
        // if (frameRateOf60)
        // {testing mobile deployment, copypasta
        //     SetVsync1();
        // }
        // else if (frameRateOf120)
        // {
        //     SetVsync0_120FPS();
        // }
        // else
        // {
        //     SetVsync1();
        // }
        instance = this;

        int gameManagerCount = FindObjectsOfType<GameManager>().Length;
        if(gameManagerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void TurnOnRegularJoystick()
    {
        if (joystickEnabled)
        {
            joystickMaster.SetActive(true);
            for (int i = 0; i < regularJoystick.Length; i++)
            {
                regularJoystick[i].SetActive(true);
            }
            directionalJoystick.SetActive(false);
        }
        else
        {
            joystickMaster.SetActive(false);
        }
    }

    public void TurnOnDirectionalJoystick()
    {
        if (joystickEnabled)
        {
            joystickMaster.SetActive(true);
            for (int i = 0; i < regularJoystick.Length; i++)
            {
                regularJoystick[i].SetActive(false);
            }
            directionalJoystick.SetActive(true);
        }
        else
        {
            joystickMaster.SetActive(false);
        }


    }

    // public void SetVsync0_120FPS()
    // {testing mobile deployments, copypasta
    //     //turns of vsync and tries to get 120fps, best for stable framerate
    //     QualitySettings.vSyncCount = 0;
    //     Application.targetFrameRate = 120;
    // }
    //
    // public void SetVsync0_Default()
    // {
    //     QualitySettings.vSyncCount = 0;
    //     Application.targetFrameRate = -1;
    // }
    //
    // public void SetVsync1()
    // {
    //     QualitySettings.vSyncCount = 1;
    //     Application.targetFrameRate = -1;
    // }


}
