using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public class TripleTriadCardOnBoard : TripleTriadCard
    {

        #region configuration

        [SerializeField] public int[] locationBoardAdjacency;
        [SerializeField] public int[] myValueToCheck;
        [SerializeField] public int[] adjacencyValueToCheck;
        [SerializeField] public bool cardInPlay;
        //[SerializeField] public Canvas cardCanvas;


        #endregion



        #region functions

        private void SendCallToTurnControllerToChangeTurn()
        {
            ttMan.SendStateChange(ttMan.betweenTurnState);
        }


        
        

        
        public void MoveToBoard()
        {
            //cardCanvas.enabled = true;
            cardAnimator.SetTrigger("moveToBoard");
        }

        public void ChangeToCardInPlay()
        {
            cardInPlay = true;
        }

        public void CheckToSeeIfCardsFlipped()
        {
            if (ttMan.RetrieveCurrentState() == ttMan.locationSelectionState)
            {
                ttMan.ttLogic.ttBoardLogic.CheckToSeeIfCardsAreFlippingForPlayer(ttMan.ttDb.RetrieveLocationSelectionCurrentSelection());
                
            }
            else //if (ttMan.RetrieveCurrentState() == ttMan.enemyTurnState)
            {
                ttMan.ttLogic.ttBoardLogic.CheckToSeeIfCardsAreFlippingForEnemy(ttMan.ttLogic.RetrieveBoardPositionToPlaceCard());
            }
            //print($"current state is {ttMan.RetrieveCurrentState()}");
        }

        public void BoardCardFlipRight()
        {
            cardAnimator.SetTrigger("cardFlipRight");
        }
        public void BoardCardFlipUp()
        {
            cardAnimator.SetTrigger("cardFlipUp");
        }
        public void BoardCardFlipLeft()
        {
            cardAnimator.SetTrigger("cardFlipLeft");
        }
        public void BoardCardFlipDown()
        {
            cardAnimator.SetTrigger("cardFlipDown");
        }

        #endregion

        

        
        
    }

}