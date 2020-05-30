using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ETF.TripleTriad
{
    public partial class TTLogic
    {

        public bool CanIScrollOnCardSelect(CardSelectionState.WhichUpDownDirection whichDirection)
        {
            if (ttUi.isLoading)
            {
                return false;
            }

            if (whichDirection == CardSelectionState.WhichUpDownDirection.IsMovingDown)
            {
                return ttUi.listOfPages[ttDb.RetrieveCardSelectionCurrentPageNumber()].currentActiveCardsOnPage > 1;
            }

            return ttUi.listOfPages[ttDb.RetrieveCardSelectionCurrentPageNumber()].currentActiveCardsOnPage > 1;
        }

        public bool CanISwitchPages(CardSelectionState.PageScrollDirections pageDirection)
        {
            if (ttUi.isLoading)
            {
                return false;
            }

            if (pageDirection == CardSelectionState.PageScrollDirections.GoingForward)
            {
                return ttDb.RetrieveCardSelectionCurrentPageNumber() < ttUi.listOfPages.Count - 1;
            }

            return ttDb.RetrieveCardSelectionCurrentPageNumber() > 0;
        }

        public bool CanSelectCardInCardSelection(int currentSpotInCardInv)
        {
            return ttDb.currentBattleQuantityForCards[currentSpotInCardInv] > 0;
        }

        public bool CanRemoveCardFromCardSelection()
        {
            if (ttDb.currentHandSelectionsList.Count > 0)
            {
                return true;
            }

            return false;
        }

        public bool AreYouGoingToLoopInCardSelection(CardSelectionState.WhichUpDownDirection whichWayImScrolling)
        {
            switch (whichWayImScrolling)
            {
                case CardSelectionState.WhichUpDownDirection.IsMovingDown:
                    return ttDb.RetrieveCardSelectionFingerLocationOnCurrentPage() >= ttUi
                        .listOfPages[ttDb.RetrieveCardSelectionCurrentPageNumber()].currentActiveCardsOnPage - 1;
                case CardSelectionState.WhichUpDownDirection.IsMovingUp:
                    return ttDb.RetrieveCardSelectionFingerLocationOnCurrentPage() == 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(whichWayImScrolling), whichWayImScrolling, null);
            }
        }

        public bool DidYouGoPastTheCardSelectionList()
        {
            return ttDb.RetrieveCardSelectionCurrentSpotInInventory() >= ttDb.RetrieveTotalNumberOfBattleCards();
        }

    }

}