using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public class TripleTriadCardInHand : TripleTriadCard
    {
        #region configuration

        private bool _hasPlayedCard;
        private int _locationToGoTo;
        private bool _isPlayerCard;

        #endregion


        
        public void SendCallToUiToStartBoardAnimation()
        {//changes the boardCardTo the card in hand, and sends him to the right location, played at the end of the isPlayed animation on hand cards
            ttMan.ttUi.PlayCardToBoardInLocationSelection(_locationToGoTo,whatCardIAm);
        }
        
        public void SetLocationToGoTo(int boardLocation)
        {//called in location selection on fire1, to set the card to the proper board location
            _locationToGoTo = boardLocation;
        }

        public void InFocus()
        {
            cardAnimator.SetTrigger("inFocus");
        }

        public void OutFocus()
        {
            cardAnimator.SetTrigger("outFocus");
        }
        
        public void CardPlayed()
        {
            cardAnimator.SetTrigger("isPlayed");
        }

        public void MoveCardUpOneSpot()
        {
            cardAnimator.SetTrigger("moveUpOneSpot");
        }
        
        
        public void ModifyMyHandCurrentCardList()
        {//removes the card from your hand when it is played
            ttMan.ttDb.modifyCurrentHandListWhenCardIsPlayed();
        }
        public void ModifyEnemyHandCurrentCardList()
        {//removes the card from your hand when it is played
            ttMan.ttDb.ModifyEnemyCurrentHandListWhenCardIsPlayed();
        }

        public void ChangeStateInCardRewards()
        {
            if (ttMan.RetrieveCurrentState() == ttMan.rewardConfirmState)
            {
                ttMan.SendStateChange(ttMan.rewardSelectionState,4);                
            }
            else
            {
                ttMan.SendStateChange(ttMan.rewardConfirmState);
                
            }
            
        }
        
        public void ChangeStateInCardRewardsEnemy(int additionArgs = 0)
        {
            if (ttMan.RetrieveCurrentState() == ttMan.rewardConfirmState)
            {
                ttMan.SendStateChange(ttMan.rewardSelectionState,4);                
            }
            else
            {
                ttMan.SendStateChange(ttMan.rewardSelectedState,additionArgs);
                
            }
            
        }
        public void TurnOnCardWonCanvas()
        {
            ttMan.ttUi.TurnOnCardWonCanvas();
        }

        public void StartUiFadeOut()
        {
            ttMan.ttUi.RewardSelectionFadeOut();
        }

        public void StartRewardSelection()
        {
            if (ttMan.ttDb.RetrieveWhoWonTheGame() == 1)
            {
                ttMan.ttUi.TurnOnCardRewardCanvas();
            }
            else
            {
                //cpu chooses reward
                var cpuCard =  ttMan.ttLogic.EnemyRewardSelection();
                ttMan.ttDb.RetrievePlayerTripleTriadCardAtLocation(cpuCard).cardAnimator.SetTrigger("cardFlip");
                //ttMan.SendStateChange(ttMan.rewardSelectedState,cpuCard);
            }


        }


        
        public void StartEndGameFadeOut()
        {
            ttMan.SendStateChange(ttMan.gameEndedState);
            //ttMan.ttUi.FinishedGameFadeOut();
        }
        


        
        
    }

}