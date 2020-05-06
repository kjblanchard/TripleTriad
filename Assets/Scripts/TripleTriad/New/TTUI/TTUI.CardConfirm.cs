 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace ETF.TripleTriad
 {
     public partial class TTUI
     {

         [Header("Card Confirm - UI References")] [SerializeField]
         Transform[] cardConfirmFingerLocations;

         [SerializeField] private Canvas _cardConfirmCanvas;

         public int ReturnCardConfirmFingerPosCount()
         {
             return cardConfirmFingerLocations.Length;
         }

         private void PlayCardConfirmAnimator()
         {
             cardConfirmAnimator.SetTrigger("displaying");

         }

         public void CloseCardConfirmAnimatorForCancel()
         {
             print("done");
             cardConfirmAnimator.SetTrigger("cancel");
         }

         public void KeepFingerOnProperLocationInCardConfirmation()
         {
             //finger is turned on and off when in update method by the isloading bool
             if (isLoading == false)
             {
                 if (!fingerPrefab.gameObject.activeInHierarchy)
                 {
                     fingerPrefab.gameObject.SetActive(true);
                 }

                 fingerPrefab.transform.position = cardConfirmFingerLocations[ttMan.ttDb
                     .RetrieveCurrentFingerPositionInCardConfirmation()].transform.position;
             }
             else
             {
                 fingerPrefab.gameObject.SetActive(false);
             }
         }

         public void InitializeCardConfirmScreen()
         {
             isLoading = true;
             _cardConfirmCanvas.enabled = true;
             PlayCardConfirmAnimator();
         }

         public void TurnOffCardConfirmCanvas()
         {
             _cardConfirmCanvas.enabled = false;
         }

     }

 }