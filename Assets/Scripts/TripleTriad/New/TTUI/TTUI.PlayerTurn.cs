using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTUI
    {
        [SerializeField] GameObject playerSelectionFinger;

        public void InitializePlayerTurnCanvas()
        {
            // enemyHandDialogBoxAnimator.gameObject.SetActive(false);
            // locationSelectionFinger.SetActive(false);
            setFocusOnPlayerCardSelected();
            _infoPanelCanvas.enabled = false;
        }

        public void InitializePlayerTurnCanvasFromLocationSelectionCancel()
        {
            locationSelectionFinger.SetActive(false);
            _infoPanelCanvas.enabled = false;
        }

        public void keepPlayerSelectionFingerOnProperLocation()
        {
            if (isLoading == false)
            {
                if (!playerSelectionFinger.gameObject.activeInHierarchy &&
                    ttMan.RetrieveCurrentState() == ttMan.playerTurnState) 
                {
                    
                    playerSelectionFinger.gameObject.SetActive(true);
                }

                playerSelectionFinger.transform.position =
                    ttdb.RetrieveTripleTriadCardInPlayerSelection().fingerPointer.transform.position;// [ttdb.RetrievePlayerTurnCurrentSelection()].transform.position;
            }
            else
            {
                playerSelectionFinger.gameObject.SetActive(false);
            }

        }

        public void setFocusOnPlayerCardSelected()
        {
            ttdb.RetrieveTripleTriadCardInPlayerSelection().cardAnimator.SetTrigger("inFocus");
            //myHandCardAnimators[ttdb.RetrievePlayerTurnCurrentSelection()].SetTrigger("inFocus");
        }

        public void setOutFocusOnPlayerCardSelected()
        {
            ttdb.RetrieveTripleTriadCardInPlayerSelection().cardAnimator.SetTrigger("outFocus");
            //myHandCardAnimators[ttdb.RetrievePlayerTurnCurrentSelection()].SetTrigger("outFocus");

        }

    }
}
