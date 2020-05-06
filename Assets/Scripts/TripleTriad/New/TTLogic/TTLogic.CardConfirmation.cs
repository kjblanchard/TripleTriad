using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTLogic
    {

        public bool CanIScrollOnCardConfirm(CardConfirmationState.WhichWayToScroll whichWayToScroll)
        {
            switch (whichWayToScroll)
            {
                case CardConfirmationState.WhichWayToScroll.IncrementingFinger:
                    if (ttUi.isLoading)
                    {
                        return false;
                    }

                    return ttDb.RetrieveCurrentFingerPositionInCardConfirmation() <
                           ttUi.ReturnCardConfirmFingerPosCount() - 1;
                case CardConfirmationState.WhichWayToScroll.DecrementingFinger:
                    if (ttUi.isLoading)
                    {
                        return false;
                    }

                    return ttDb.RetrieveCurrentFingerPositionInCardConfirmation() > 0;
            }

            return false;
        }

        public bool CanIPressCancelButtonOnConfirmScreen()
        {
            if (ttUi.isLoading)
            {
                return false;
            }

            return ttDb.RetrieveCurrentFingerPositionInCardConfirmation() != 1;
        }

        public bool CanIGoBackToCardSelectionScreenFromConfirmScreen()
        {
            if (ttUi.isLoading)
            {
                return false;
            }

            return ttDb.RetrieveCurrentFingerPositionInCardConfirmation() == 1;
        }

        public bool CanIAdvanceToChooseEnemyHandScreen()
        {
            return ttDb.RetrieveCurrentFingerPositionInCardConfirmation() == 0 && ttUi.isLoading == false;
        }

    }
}
