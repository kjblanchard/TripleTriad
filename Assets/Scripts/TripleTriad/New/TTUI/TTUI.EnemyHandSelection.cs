using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public partial class TTUI
    {

        [Header("Stuff For EnemyHandSelection")]
        //[SerializeField] Animator[] enemyHandAnimator;

        [SerializeField]
        Animator enemyHandDialogBoxAnimator;

        private float _randomNumberForCardDisplaying;
        [SerializeField] Animator turnIndicatorFingerAnimator;
        [SerializeField] private Canvas _enemyDialogBoxCanvas;
        [SerializeField] private Text enemyDialogBoxNameText;
        [SerializeField] private Text enemyDialogBoxDialogText;

        private IEnumerator PlayEnemyFullHandAnimationCo()
        {
            _randomNumberForCardDisplaying = 0.1f;
            var _cachedWaitForSeconds = new WaitForSeconds(_randomNumberForCardDisplaying);
            
            for (int i = 0; i < ttdb.fullEnemyTripleTriadCards.Length; i++)
            {

                yield return _cachedWaitForSeconds;
                PlayEnemyHandAnimation(i);
            }

            StartCoroutine(PlayEnemyHandCardFlipAnimations());
        }

        private void PlayEnemyHandAnimation(int cardToDisplay)
        {
            var cachedCard = ttdb.fullEnemyTripleTriadCards[cardToDisplay];
            cachedCard.cardCanvas.enabled = true;
            cachedCard.cardAnimator.Play("MyHandSelect");
            SoundManager.instance.PlaySFX(6);
        }

        public IEnumerator InitializeEnemyHandSelectScreenUiCo()
        {
            //initializes the gameobjects and activates them, also loads in texts to be displayed in the boxes, as well as turning off the things after the animations are done playing;
            isLoading = true;
            _enemyDialogBoxCanvas.enabled = true;
            PlayCardSelectCanvasLeavingAnimation();
            PlayCardSelectCardDisplayInfoLeavingAnimation();
            PlayCardConfirmWindowLeavingAnimation();
            UpdateEnemyNameInTextBox();
            UpdateEnemyDialogBoxToChoosingCards();
            PlayEnemyHandDialogBoxAnimation();
            yield return new WaitForSeconds(0.5f);
            //TurnOffEnemyDialogBoxCanvas();
            StartCoroutine(PlayEnemyFullHandAnimationCo());
        }

        private void TurnOffEnemyDialogBoxCanvas()
        {
            _enemyDialogBoxCanvas.enabled = false;
        }
        
        private void PlayCardSelectCanvasLeavingAnimation()
        {
            cardSelectionWindowAnimator.Play("CardWindowLeaving");
        }

        private void PlayCardSelectCardDisplayInfoLeavingAnimation()
        {
            cardSelectionCardDisplayAnimator.Play("CardDisplayLeaving");
        }

        private void PlayCardConfirmWindowLeavingAnimation()
        {
            cardConfirmAnimator.Play("CardConfirmWindowLeaving");
        }

        private void PlayEnemyHandDialogBoxAnimation()
        {
            enemyHandDialogBoxAnimator.SetTrigger("play");
        }

        private IEnumerator PlayEnemyHandCardFlipAnimations()
        {
            
            if (ttdb.RetrieveIsHandOpen())
            {
                
                for (int i = 0; i < ttdb.fullEnemyTripleTriadCards.Length; i++)
                {
                    var cachedWaitForSeconds = new WaitForSeconds(0.2f);
                    ttdb.fullEnemyTripleTriadCards[i].PlayFlipCardAnimation();
                    yield return cachedWaitForSeconds;
                }


                yield return new WaitForSeconds(1f);
                //isLoading = false;
            }
            else
            {
                
                yield return new WaitForSeconds(0.25f);
            }

            ttMan.SendStateChange(ttMan.turnSelectionState);
        }


        public void UpdateEnemyNameInTextBox()
        {
            enemyDialogBoxNameText.text = ttdb.WhatIsTheEnemyNpcName() + " :";
        }

        public void UpdateEnemyDialogBoxToChoosingCards()
        {
            enemyDialogBoxDialogText.text = ttdb.WhatWillTheEnemySayWhenChoosingCards();
        }


    }
}
