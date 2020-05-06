using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour
{
    public static EssentialsLoader instance;

    [Header("Things to Load")]

    [SerializeField] GameObject gameManager = null;
    [SerializeField] GameObject soundManager = null;


    private void Awake()
    {
        
        int essentialsLoaderCount = FindObjectsOfType<EssentialsLoader>().Length;
        if(essentialsLoaderCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        Instantiate(gameManager);
        Instantiate(soundManager);


    }

}
