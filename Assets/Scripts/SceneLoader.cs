using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [Header("Scene Management")]
    public string sceneToLoad = null;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;


        int sceneloaderCount = FindObjectsOfType<SceneLoader>().Length;
        if(sceneloaderCount > 1)
        {
            Destroy(gameObject);

        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Reload()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
