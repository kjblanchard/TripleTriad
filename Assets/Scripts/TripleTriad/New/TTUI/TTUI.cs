using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public partial class TTUI : MonoBehaviour
    {
        //this is the master TTUI

        #region configuration

        [Header("Script Reference")] [SerializeField]
        TTDB ttdb;

        [SerializeField] TripleTriadManager ttMan;
        [SerializeField] private Canvas fingerCanvas;
        [SerializeField] public Image _tripleTriadBoardImage;
        private static readonly int kStartGame = Animator.StringToHash("startGame");

        #endregion


        
        
        public void InitializeAndCacheAllUiElementsForGameStart(bool isGameEnded = false)
        { //start all Canvas's
            _titleScreenCanvas.enabled = true;
            _opponentSelectionCanvas.enabled = true;
            _showingRulesCanvas.enabled = true;
            _cardSelectionCanvas.enabled = true;
            _cardConfirmCanvas.enabled = true;
            var playerHand = ttdb.RetrieveAllPlayerHandCardsFull();
            for (int i = 0; i < playerHand.Length; i++)
            {
                playerHand[i].cardCanvas.enabled = true;
            }
            for (int i = 0; i < ttdb.fullEnemyTripleTriadCards.Length ; i++)
            {
                ttdb.fullEnemyTripleTriadCards[i].cardCanvas.enabled = true;
                ttdb.fullEnemyTripleTriadCards[i].SetToEmptyCardBack();
            }
            var boardCards = ttdb.RetrieveAllCardsOnBoard();
            for (int i = 0; i < boardCards.Length ; i++)
            {
                boardCards[i].cardCanvas.enabled = true;
            }
            _scoreDisplayCanvas.enabled = true;
            _enemyDialogBoxCanvas.enabled = true;
            _endGameCanvas.enabled = true;
            _cardRewardCanvas.enabled = true;
            _rewardConfirmCanvas.enabled = true;
            _cardWonCanvas.enabled = true;
            fingerCanvas.enabled = true;

            //reset all animations

            for (int i = 0; i < _opponentBoxes.Length; i++)
            {
                if (_opponentBoxes[i].canSelectOpponent)
                {
                    _opponentBoxes[i].opponentAnimator.Rebind();

                }
            }
            showingRulesAnimator.Rebind();
            cardSelectionWindowAnimator.Rebind();
            cardConfirmAnimator.Rebind();
             for (int i = 0; i < playerHand.Length; i++)
             {
                 playerHand[i].cardAnimator.Rebind();
             }
            
             for (int i = 0; i < ttdb.fullEnemyTripleTriadCards.Length; i++)
             {
                 ttdb.fullEnemyTripleTriadCards[i].cardAnimator.Rebind();
             }
            
             for (int i = 0; i < boardCards.Length; i++)
             {
                 boardCards[i].cardAnimator.Rebind();
             }
             enemyHandDialogBoxAnimator.Rebind();
             _youWinAnimator.Rebind();
             _youLoseAnimator.Rebind();
             _drawAnimator.Rebind();
             cardDisplayAnimator.Rebind();
             cardConfirmAnimator.Rebind();
             cardSelectionWindowAnimator.Rebind();
             cardSelectionCardDisplayAnimator.Rebind();
                
            //close all canvas'
            _titleScreenCanvas.enabled = false;
            _opponentSelectionCanvas.enabled = false;
            _showingRulesCanvas.enabled = false;
            _cardSelectionCanvas.enabled = false;
            _cardConfirmCanvas.enabled = false;
            for (int i = 0; i < playerHand.Length; i++)
            {
                playerHand[i].cardCanvas.enabled = false;
            }
            for (int i = 0; i < ttdb.fullEnemyTripleTriadCards.Length ; i++)
            {
                ttdb.fullEnemyTripleTriadCards[i].cardCanvas.enabled = false;
            }
            for (int i = 0; i < boardCards.Length ; i++)
            {
                boardCards[i].cardCanvas.enabled = false;
            }
            _scoreDisplayCanvas.enabled = false;
            _enemyDialogBoxCanvas.enabled = false;
            _endGameCanvas.enabled = false;
            _cardRewardCanvas.enabled = false;
            _rewardConfirmCanvas.enabled = false;
            _cardWonCanvas.enabled = false;
            _tripleTriadBoardImage.color = new Color(0.28f,0.19f,0.44f);
            
            locationSelectionFinger.SetActive(false);
            playerSelectionFinger.SetActive(false);
            turnIndicatorFingerAnimator.gameObject.SetActive(false);
        }
        
        public void StartGameFadeIn()
        {
            _tripleTriadUiFade.animator.SetTrigger(kStartGame);
        }
        
    }
}

