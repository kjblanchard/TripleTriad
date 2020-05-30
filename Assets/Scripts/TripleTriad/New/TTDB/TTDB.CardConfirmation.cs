using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTDB
    {
        [Header("Card Confirmation Section")] private int _currentFingerPositionInCardConfirmation;

        public int RetrieveCurrentFingerPositionInCardConfirmation()
        {
            return _currentFingerPositionInCardConfirmation;
        }

        public void InitializeCardConfirmationDbValues()
        {
            _currentFingerPositionInCardConfirmation = 0;
        }

        public void CardConfirmCancelButtonPress()
        {
            _currentFingerPositionInCardConfirmation = 1;
        }

        public void CardConfirmLeftRightUpdateDb(CardConfirmationState.WhichWayToScroll whichWayToScroll)
        {
            switch (whichWayToScroll)
            {
                case CardConfirmationState.WhichWayToScroll.IncrementingFinger:
                    _currentFingerPositionInCardConfirmation++;
                    break;
                case CardConfirmationState.WhichWayToScroll.DecrementingFinger:
                    _currentFingerPositionInCardConfirmation--;
                    break;
            }
        }

    }
}
