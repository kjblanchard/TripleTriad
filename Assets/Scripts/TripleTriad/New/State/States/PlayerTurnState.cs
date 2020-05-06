using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public class PlayerTurnState : TtState
    {
        #region Configuration

        [SerializeField] private TripleTriadManager ttMan;
    

        #endregion


        public override void Startup(int additionalArgs = 0)
        {
            if (additionalArgs == 0)
            {
                ttMan.ttDb.InitializeLocationSelectionDbValues();
                ttMan.ttDb.InitializePlayerTurnDbValues();
                ttMan.ttUi.InitializePlayerTurnCanvas();
                
            }
            else
            {
                ttMan.ttUi.InitializePlayerTurnCanvasFromLocationSelectionCancel();
            }
        }

        public override void Execute()
        {
            ListenForInput();
            
            ttMan.ttUi.keepPlayerSelectionFingerOnProperLocation();
        }
        

        #region Functions

        private void ListenForInput()
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetButtonDown("up")))
            {
                if (ttMan.ttLogic.CanIScrollUpInPlayerTurn())
                {
                    ScrollUp();
                }
                else if (ttMan.ttLogic.CanILoopUpInPlayerTurn())
                {
                    LoopToBottom();
                }
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("down")))
            {
                if (ttMan.ttLogic.CanIScrollDownInPlayerTurn())
                {
                    ScrollDown();
                }
                else if (ttMan.ttLogic.CanILoopDownInPlayerTurn())
                {
                    LoopToTop();
                }
            }
            else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")))
            {
                ttMan.SendStateChange(ttMan.locationSelectionState);
            }

        }

        private void LoopToTop()
        {
            ttMan.ttUi.setOutFocusOnPlayerCardSelected();
            ttMan.ttDb.MovePlayerTurnCurrentSelectionTop();
            SoundManager.instance.PlaySFX(0);
            ttMan.ttUi.setFocusOnPlayerCardSelected();
        }

        private void ScrollDown()
        {
            ttMan.ttUi.setOutFocusOnPlayerCardSelected();
            ttMan.ttDb.MovePlayerTurnCurrentSelectionDown();
            SoundManager.instance.PlaySFX(0);
            ttMan.ttUi.setFocusOnPlayerCardSelected();
        }

        private void LoopToBottom()
        {
            ttMan.ttUi.setOutFocusOnPlayerCardSelected();
            ttMan.ttDb.MovePlayerTurnCurrentSelectionToBottom();
            SoundManager.instance.PlaySFX(0);
            ttMan.ttUi.setFocusOnPlayerCardSelected();
        }

        private void ScrollUp()
        {
            ttMan.ttUi.setOutFocusOnPlayerCardSelected();
            ttMan.ttDb.MovePlayerTurnCurrentSelectionUp();
            SoundManager.instance.PlaySFX(0);
            ttMan.ttUi.setFocusOnPlayerCardSelected();
        }

        #endregion
        
        
        
    }
}
