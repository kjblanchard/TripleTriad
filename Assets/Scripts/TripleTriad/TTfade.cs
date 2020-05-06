// using System.Collections;
// using System.Collections.Generic;
// using ETF.TripleTriad;
// using UnityEngine;
// using UnityEngine.UI; // needs to access the UI
//
// public class TTfade : MonoBehaviour
// {
//     public static TTfade instance;
//
//     public Image fadeScreen; //this will add a public variable, and drag the fade image to it which is a UI element
//
//     public bool shouldFadeToBlack;
//     public bool shouldFadeFromBlack; //bool is true or false, like a checkbox
//     public float fadeSpeed;
//
//     // Start is called before the first frame update
//
//     private void Awake()
//     {
//         instance = this;
//     }
//     void Start()
//     {
//         DontDestroyOnLoad(gameObject); // keeps the UIfade alive between scenes right now it is made on countryside1
//
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (shouldFadeToBlack)
//         {
//             fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
//             //this will adjust the fadescreen color to (note that there is rgba value for color, and it is NOT 255, but rather 1.)
//             //we adjust the color of ONLY the A, and since we want to do it gradually we use the math function
//             //the time.delta by fadespeed is to make it the same for all computers, deltatime is how long the last refresh took
//             //make sure to set the fadespeed public variable
//             if (fadeScreen.color.a == 1f)
//             {
//                 shouldFadeToBlack = false; //if a gets to 1, set the fadetoblack to be false
//             }
//         }
//
//         if (shouldFadeFromBlack)
//         {
//             fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
//
//             if (fadeScreen.color.a == 0f)
//             {
//                 shouldFadeFromBlack = false; //same as above but opposite
//
//             }
//         }
//
//     }
//
//     public void FadeToBlack()
//     {
//         shouldFadeToBlack = true;
//         shouldFadeFromBlack = false;
//     } //set these to that variable
//
//     public void FadeFromBlack()
//     {
//         shouldFadeToBlack = false;
//         shouldFadeFromBlack = true;
//     }
//
//     public void StartingFadeIn()
//     {
//         fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 1f);
//         shouldFadeFromBlack = true;
//     }
//
//
//     public IEnumerator StartCardBattleFade(float timeToWait)
//     {
//         yield return new WaitForSeconds(timeToWait);
//         //FadeFromBlack();
//         //GameManager.instance.TurnOnDirectionalJoystick();
//         GameManager.instance.ttCanvas.SetActive(true);
//         TripleTriadManager.instance.SendStateChange(TripleTriadManager.instance.gameStartedState);
//
//
//     }
//
//
//
//
// }
