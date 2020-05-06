using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTUI
    {

        public void InitializeTurnSelection()
        {
            turnIndicatorFingerAnimator.gameObject.SetActive(true);
            UpdateScoreCount();
            _scoreDisplayCanvas.enabled = true;
            UpdateEnemyDialogBoxToTurnSelection();
        }

        public void UpdateEnemyDialogBoxToTurnSelection()
        {
            //changes the dialog box to display proper information for turn selection
            enemyDialogBoxDialogText.text = ttdb.WhatWillTheEnemySayWhenTurnSelection();
        }

        public void switchToFingerTurnSelectionAnimation()
        {
            SoundManager.instance.PlaySFX(5);
            turnIndicatorFingerAnimator.Play("TurnSelectionFinger");
        }
        
        public void LeavingTurnSelectionUiChange()
        {
            enemyHandDialogBoxAnimator.SetTrigger("default");
            _enemyDialogBoxCanvas.enabled = false;
            //enemyHandDialogBoxAnimator.gameObject.SetActive(false);
            //locationSelectionFinger.SetActive(false);
        }


    }
}
