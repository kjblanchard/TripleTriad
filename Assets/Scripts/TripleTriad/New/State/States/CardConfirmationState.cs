using UnityEngine;

namespace ETF.TripleTriad
{
    public class CardConfirmationState : TtState
    {
        #region Configuration

        [Header("Script References")] [SerializeField]
        TripleTriadManager ttMan;

        public enum WhichWayToScroll
        {
            IncrementingFinger,
            DecrementingFinger,
        }


        #endregion

        public override void Startup(int additionalArgs = 0)
        {
            ttMan.ttDb.InitializeCardConfirmationDbValues();
            ttMan.ttUi.InitializeCardConfirmScreen(); 
        }

        public override void Execute()
        {
            ListenForUserInputs();

            ttMan.ttUi.KeepFingerOnProperLocationInCardConfirmation();
        }

        

        #region Functions

        
        private void ListenForUserInputs()
        {
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("right")) &&
                ttMan.ttLogic.CanIScrollOnCardConfirm(WhichWayToScroll.IncrementingFinger))
            {
                CardConfirmLeftRightScroll(WhichWayToScroll.IncrementingFinger);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("left")) &&
                     ttMan.ttLogic.CanIScrollOnCardConfirm(WhichWayToScroll.DecrementingFinger))
            {
                CardConfirmLeftRightScroll(WhichWayToScroll.DecrementingFinger);
            }
            else if ((Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2")) &&
                     ttMan.ttLogic.CanIPressCancelButtonOnConfirmScreen())
            {
                ChangeToGoBackOnCancelButton();
            }
            else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) &&
                     ttMan.ttLogic.CanIGoBackToCardSelectionScreenFromConfirmScreen())
            {
                ttMan.ttUi.CloseCardConfirmAnimatorForCancel();
                ttMan.SendStateChange(ttMan.cardSelectionState,1);
            }
            else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) &&
                     ttMan.ttLogic.CanIAdvanceToChooseEnemyHandScreen())
            {
                ttMan.ttDb.UpdateMyHandTripleTriadCardsWithMySelectionList();
                ttMan.SendStateChange(ttMan.enemyHandSelectionState);
            }
        }
        
        private void CardConfirmLeftRightScroll(WhichWayToScroll directionToMove)
        {
            switch (directionToMove)
            {
                case WhichWayToScroll.IncrementingFinger:
                    ttMan.ttDb.CardConfirmLeftRightUpdateDb(directionToMove);
                    SoundManager.instance.PlaySFX(0);
                    break;
                case WhichWayToScroll.DecrementingFinger:
                    ttMan.ttDb.CardConfirmLeftRightUpdateDb(directionToMove);
                    SoundManager.instance.PlaySFX(0);
                    break;
            }

        }

        private void ChangeToGoBackOnCancelButton()
        {
            
            ttMan.ttDb.CardConfirmCancelButtonPress();
            SoundManager.instance.PlaySFX(0);
        }


        #endregion
    }
}
