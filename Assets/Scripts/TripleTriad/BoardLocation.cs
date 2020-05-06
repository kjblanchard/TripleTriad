// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class BoardLocation : MonoBehaviour
// {
//     public Image imageReference;
//     public bool spotTaken = false;
//
//     public bool playerOwned;
//
//     public bool cardIsFlipping;
//
//     public Text currentCard = null;
//
//     public Sprite cardImageA;
//     public Sprite cardImageB;
//
//     public int topValue;
//     public int rightValue;
//     public int bottomValue;
//     public int leftValue;
//
//     //new test stuff
//     public Card cardHeld;
//     public int[] boardAdjacencies;
//     public int[] myValueToCheck;
//     public int[] adjacencyValueToCheck;
//     private Card cardReference;
//     public bool checkIfFlipped = false;
//
//     private int directionToFlip;
//     public int flippedBy;
//     public int myLocationNumber;
//
//     [Header("Card Flips")]
//     [SerializeField] int timesToRotate = 30;
//     [SerializeField] int speed = 20;
//     [SerializeField] float waitTime = 0.005f;
//     private bool displayCardBack;
//
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (spotTaken == true && cardIsFlipping == false && displayCardBack == false)
//         {
//             if (playerOwned)
//             {
//                 imageReference.sprite = cardHeld.playerImage;
//                  imageReference.color = new Color(1f, 1f, 1f, 1f);
//             }
//             else
//             {
//                 imageReference.sprite = cardHeld.enemyImage;
//                 imageReference.color = new Color(1f, 1f, 1f, 1f);
//             }
//         }
//         
//         if ( cardIsFlipping == true)
//         {
//             StartCoroutine(RotateCard(flippedBy));
//             cardIsFlipping = false;//put this here to test
//
//         }
//
//         if(checkIfFlipped == true)
//         {
//             CheckLocationTest();
//         }
//
//         
//     }
//
//     public void CheckLocationTest()
//     {
//         //TTManager.instance.cardsOnBoard++;
//         for (int i = 0; i < boardAdjacencies.Length; i++)
//         {
//
//
//
//             if (TTManager.instance.boardLocationHolder[boardAdjacencies[i]].spotTaken == true)
//
//             {
//
//                 Debug.Log(TTManager.instance.boardLocationHolder[boardAdjacencies[i]] + "This is the spot I'm checking if the enemy owns and he is " + TTManager.instance.boardLocationHolder[boardAdjacencies[i]].playerOwned);
//                
//
//                 if ( (playerOwned == true && TTManager.instance.boardLocationHolder[boardAdjacencies[i]].playerOwned == false) || (playerOwned == false && TTManager.instance.boardLocationHolder[boardAdjacencies[i]].playerOwned == true) )
//                 {
//
//                     if (cardHeld.cardValues[myValueToCheck[i]] > TTManager.instance.boardLocationHolder[boardAdjacencies[i]].cardHeld.cardValues[adjacencyValueToCheck[i]])
//                     {
//
//                         Debug.Log("Id flip that dang card at positon " + boardAdjacencies[i] + "cause my value is " + cardHeld.cardValues[myValueToCheck[i]] + "And his value is " + TTManager.instance.boardLocationHolder[boardAdjacencies[i]].cardHeld.cardValues[adjacencyValueToCheck[i]]);
//                         TTManager.instance.boardLocationHolder[boardAdjacencies[i]].playerOwned = !TTManager.instance.boardLocationHolder[boardAdjacencies[i]].playerOwned;
//                         
//                         TTManager.instance.boardLocationHolder[boardAdjacencies[i]].flippedBy = myLocationNumber;
//                         //TTManager.instance.boardLocationHolder[boardAdjacencies[i]].directionToFlip = TTManager.instance.boardLocationHolder[boardAdjacencies[i]].adjacencyValueToCheck[i];
//                         TTManager.instance.boardLocationHolder[boardAdjacencies[i]].cardIsFlipping = true;
//                         
//                     }
//                     else
//                     {
//                         Debug.Log("I wont be flippin that card at position " + boardAdjacencies[i] + "cause my value is " + cardHeld.cardValues[myValueToCheck[i]] + "And his value is " + TTManager.instance.boardLocationHolder[boardAdjacencies[i]].cardHeld.cardValues[adjacencyValueToCheck[i]]);
//                     }
//
//                 }
//             }
//         }
//         checkIfFlipped = false;
//         TTManager.instance.UpdateScore();
//         
//         
//     }
//
//
//     IEnumerator RotateCard(int flippedby)
//     {
//         displayCardBack = true;
//         yield return new WaitForSeconds(0.05f);
//         //imageReference.transform.rotation = new Quaternion(0, 0, 0, 0);
//         for (int i = 0; i < boardAdjacencies.Length; i++)
//         {
//             if(flippedby == boardAdjacencies[i])
//             {
//                  directionToFlip = adjacencyValueToCheck[i];
//             }
//         }
//         SoundManager.instance.PlaySFX(4);
//         /*
//         int timesToRotate = 15;
//         int speed = 2;
//         float waitTime = 0.005f;
//         */
//         if (directionToFlip == 1)
//         {
//             imageReference.sprite = CardInventory.instance.emptyCard.playerImage;
//             
//             for (int i = 0; i < timesToRotate; i++)
//
//             {
//                 imageReference.transform.Rotate(0, speed, 0);
//                 Debug.Log("I just did the rotation");
//                 yield return new WaitForSeconds(waitTime);
//             }
//         }
//         else if (directionToFlip == 2)
//         {
//             imageReference.sprite = CardInventory.instance.emptyCard.playerImage;
//             for (int i = 0; i < timesToRotate; i++)
//
//             {
//                 imageReference.transform.Rotate(-speed, 0, 0);
//                 Debug.Log("I just did the rotation");
//                 yield return new WaitForSeconds(waitTime);
//             }
//         }
//         else if (directionToFlip == 3)
//         {
//             imageReference.sprite = CardInventory.instance.emptyCard.playerImage;
//             for (int i = 0; i < timesToRotate; i++)
//
//             {
//                 imageReference.transform.Rotate(0, - speed, 0);
//                 Debug.Log("I just did the rotation");
//                 yield return new WaitForSeconds(waitTime);
//             }
//         }
//         else if (directionToFlip == 0)
//         {
//             imageReference.sprite = CardInventory.instance.emptyCard.playerImage;
//             for (int i = 0; i < timesToRotate; i++)
//
//             {
//                 imageReference.transform.Rotate(speed, 0, 0);
//                 Debug.Log("I just did the rotation");
//                 yield return new WaitForSeconds(waitTime);
//             }
//         }
//
//         imageReference.transform.rotation = new Quaternion(0, 0, 0, 0);
//         displayCardBack = false;
//
//     }
// }
