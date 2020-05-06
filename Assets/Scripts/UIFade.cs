// using System.Collections;
// using System.Collections.Generic;
// using ETF.TripleTriad;
// using UnityEngine;
// using UnityEngine.UI; // needs to access the UI
//
// public class UIFade : MonoBehaviour
// {
//     public static UIFade instance;
//
//     public Image fadeScreen; //this will add a public variable, and drag the fade image to it which is a UI element
//
//     public bool shouldFadeToBlack;
//     public bool shouldFadeFromBlack; //bool is true or false, like a checkbox
//     public float fadeSpeed = 1;
//
//     [SerializeField] private TripleTriadManager _ttMan;
//
//     // Start is called before the first frame update
//
//     private void Awake()
//     {
//         instance = this;
//     }
//     void Start()
//     {
//         //DontDestroyOnLoad(gameObject); // keeps the UIfade alive between scenes right now it is made on countryside1
//
//         //StartingFadeIn(); //adding in to start a fade in testing
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
//     public IEnumerator MenuOpenerFade()
//     {
//         if (GameManager.instance.mapTransitionTextDisplaying == true)
//         {
//
//             GameMenus.instance.mapScreenBox.SetActive(false);
//             GameManager.instance.mapTransitionTextDisplaying = false;
//         }
//         fadeSpeed = 3;
//         FadeToBlack();
//         InventoryUI.instance.UpdateGilValue();
//         GameManager.instance.TurnOnDirectionalJoystick();
//         InventoryUI.instance.UpdatePlaytimeEveryMinute();
//         PlayerController.instance.canMove = false;
//         InventoryManager.instance.initializingFields = true;
//         SoundManager.instance.PlayIntroLoop(2);
//         InventoryUI.instance.FillMateriaList();
//         yield return new WaitForSeconds(0.5f);
//         FadeFromBlack();
//         InventoryManager.instance.inventoryState = InventoryManager.InventoryStates.OPTIONS_PANEL;
//         //yield return new WaitForSeconds(0.5f);
//         
//     }
//
//     public IEnumerator MenuCloserFade()
//     {
//         SoundManager.instance.PlaySFX(1);
//         SoundManager.instance.PlayIntroLoop(SoundManager.instance.bgmToPlay);
//         //StopAllCoroutines();
//         InventoryUI.instance.StopAllCoroutines();
//         FadeToBlack();
//         yield return new WaitForSeconds(1f);
//         StartCoroutine(GameMenus.instance.ShowMapTransition());
//         GameManager.instance.shouldCloseMenu = true;
//         InventoryManager.instance.inventoryState = InventoryManager.InventoryStates.DISABLED;
//         InventoryManager.instance.currentOption = 0;
//         FadeFromBlack();
//         GameManager.instance.TurnOnRegularJoystick();
//         PlayerController.instance.canMove = true;
// //        Debug.Log("just did menu closer");
//
//     }
//
//     public IEnumerator ShopOpenerFade()
//     {
//         SoundManager.instance.PlayIntroLoop(6);
//         GameManager.instance.inShop = true;
//         DialogManager.instance.ShopCloseDialogWindow();
//         
//         if (GameManager.instance.mapTransitionTextDisplaying == true)
//         {
//
//             GameMenus.instance.mapScreenBox.SetActive(false);
//             GameManager.instance.mapTransitionTextDisplaying = false;
//         }
//
//         GameManager.instance.canMoveCursor = false;
//         //yield return new WaitForSeconds(0.5f);
//         ShopManager.instance.shopDialog.dialogText.text = ShopUI.instance.variableShopEnterText;
//         fadeSpeed = 3;
//         FadeToBlack();
//         ShopUI.instance.tempCurrentGil = Inventory.instance.currentGil;
//         PlayerController.instance.canMove = false;
//         ShopUI.instance.charHolderReference.LoadCharacters();
//         yield return new WaitForSeconds(0.5f);
//         ShopManager.instance.shopOpen = true;
//         GameManager.instance.TurnOnDirectionalJoystick();
//         DialogManager.instance.choiceBoxGameObject.SetActive(false);
//
//         FadeFromBlack();
//         ShopManager.instance.TurnOn();
//         //ShopManager.instance.shopDialog.dialogText.text = ShopUI.instance.variableShopEnterText;
//         StartCoroutine(ShopManager.instance.shopDialog.TypewriterText());
//
//     }
//
//     public IEnumerator ShopCloserFade()
//     {
//         if (ShopManager.instance.shopDialog.closingMenu)
//         {
//             yield return new WaitForSeconds(0.6f);
//         }
//         ShopManager.instance.finger.SetActive(false);
//         SoundManager.instance.PlayIntroLoop(SoundManager.instance.bgmToPlay);
//         SoundManager.instance.PlaySFX(1);
//
//         ShopManager.instance.blinkingFinger.SetActive(false);
//
//         FadeToBlack();
//         Inventory.instance.currentGil = ShopUI.instance.tempCurrentGil;
//         yield return new WaitForSeconds(1f);
//         ShopManager.instance.Turnoff();
//         DialogManager.instance.CloseDialogWindow();
//         StartCoroutine(GameMenus.instance.ShowMapTransition());
//         FadeFromBlack();
//         //GameManager.instance.TurnOnRegularJoystick();
//         PlayerController.instance.canMove = true;
//         GameManager.instance.inShop = false;
//         ShopManager.instance.shopDialog.closingMenu = false;
//     }
//     
//     public IEnumerator StartCardBattleFade(float timeToWait)
//     {
//         FadeToBlack();
//         yield return new WaitForSeconds(timeToWait);
//         //FadeFromBlack();
//         //GameManager.instance.TurnOnDirectionalJoystick();
//         TripleTriadManager.instance.gameObject.SetActive(true);
//         GameManager.instance.ttCanvas.SetActive(true);
//         PlayerController.instance.canMove = false;
//
//         //GameManager.instance.ttActive = true;
//         TripleTriadManager.instance.SendStateChange(TripleTriadManager.instance.gameStartedState);
//
//
//     }
//
//     public IEnumerator EndCardBattleFade(float timeTOWait)
//     {
//         FadeFromBlack();
//         yield return new WaitForSeconds(0.5f);
//         GameManager.instance.TurnOnRegularJoystick();
//         GameManager.instance.ttActive = false;
//         GameManager.instance.dialogActive = false;
//         //PlayerController.instance.canMove = false;
//         //yield return new WaitForSeconds(0.5f);
//         GameManager.instance.afterCardBattle = true;
//         
//         //_ttMan.gameObject.SetActive(false);
//     }
//
// }
