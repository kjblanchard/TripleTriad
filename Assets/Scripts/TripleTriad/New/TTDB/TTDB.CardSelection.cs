using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTDB
    {
        [Header("Card Selection Cursor Memory")] [SerializeField]
        int _fingerLocationOnCurrentPage;

        [SerializeField] int _currentPageNumber;
        [SerializeField] int _currentSpotInCardInventory;

        [Header("Card Selection References")]
        public List<LastItemChosen> currentHandSelectionsList = new List<LastItemChosen>();

        [SerializeField] private TripleTriadCardInHand[] _myFullHandTripleTriadCards;
       private List<TripleTriadCardInHand> _myCurrentHandTripleTriadCards = new List<TripleTriadCardInHand>();
       private List<int> _randomCardIntList = new List<int>();

        public int RetrieveCardSelectionFingerLocationOnCurrentPage()
        {
            return _fingerLocationOnCurrentPage;
        }

        public int RetrieveCardSelectionCurrentPageNumber()
        {
            return _currentPageNumber;
        }

        public int RetrieveCardSelectionCurrentSpotInInventory()
        {
            return _currentSpotInCardInventory;
        }

        public void InitializeCardSelectionValuesInDB()
        {
            _fingerLocationOnCurrentPage = 0;
            _currentPageNumber = 0;
            _currentSpotInCardInventory = 0;
            _randomCardIntList.Clear();
        }

        public void UpdateCardSelectionFingerPositionForScrollingInDb(
            CardSelectionState.WhichUpDownDirection whichUpDownDirection)
        {
            if (whichUpDownDirection == CardSelectionState.WhichUpDownDirection.IsMovingDown)
            {
                _fingerLocationOnCurrentPage++;
                _currentSpotInCardInventory++;
            }
            else if (whichUpDownDirection == CardSelectionState.WhichUpDownDirection.IsMovingUp)
            {
                _fingerLocationOnCurrentPage--;
                _currentSpotInCardInventory--;
            }
        }

        public void UpdateCardSelectionFingerPositionForPageScrollingInDb(
            CardSelectionState.PageScrollDirections whichPageScrollDirection)
        {
            if (whichPageScrollDirection == CardSelectionState.PageScrollDirections.GoingForward)
            {
                _currentPageNumber++;
                _currentSpotInCardInventory += 10;
            }
            else if (whichPageScrollDirection == CardSelectionState.PageScrollDirections.GoingBackward)
            {
                _currentPageNumber--;
                _currentSpotInCardInventory -= 10;
            }
        }

        public void UpdateCardSelectionFingerPositionForLoopingInDb(
            CardSelectionState.WhichUpDownDirection whichUpDownDirection)
        {
            switch (whichUpDownDirection)
            {
                case CardSelectionState.WhichUpDownDirection.IsMovingDown:
                    _fingerLocationOnCurrentPage = 0;
                    _currentSpotInCardInventory -= ttMan.ttUi.RetrieveNumberOfCardsActiveOnPage(_currentPageNumber) - 1;
                    break;
                case CardSelectionState.WhichUpDownDirection.IsMovingUp:
                    _fingerLocationOnCurrentPage = ttMan.ttUi.RetrieveNumberOfCardsActiveOnPage(_currentPageNumber) - 1;
                    _currentSpotInCardInventory = _currentSpotInCardInventory +
                        ttMan.ttUi.RetrieveNumberOfCardsActiveOnPage(_currentPageNumber) - 1;
                    break;
            }
        }

        public void UpdateCardSelectionInfoOnScrollingPastEndOfPage()
        {
            _fingerLocationOnCurrentPage = ttMan.ttUi.listOfPages[_currentPageNumber].currentActiveCardsOnPage - 1;
            _currentSpotInCardInventory = RetrieveTotalNumberOfBattleCards() - 1;
        }

        public void UpdateMyHandTripleTriadCardsWithMySelectionList()
        {

            for (int i = 0; i < currentHandSelectionsList.Count; i++)
            {
                _myFullHandTripleTriadCards[i].whatCardIAm =
                    currentBattleSelectableCards[currentHandSelectionsList[i].spotInCardInv];
            }

            for (int i = 0; i < _myFullHandTripleTriadCards.Length; i++)
            {
                _myCurrentHandTripleTriadCards.Add(_myFullHandTripleTriadCards[i]);
            }
            
        }
        public IEnumerator UpdateMyHandTripleTriadCardsWithRandomSelection()
        {
            _randomCardIntList.Clear();
            _myCurrentHandTripleTriadCards.Clear();
            var random = new System.Random();
            while (_randomCardIntList.Count < 5)
            {
                var potentialChoice = random.Next(0, currentBattleSelectableCards.Count);
                if (currentBattleQuantityForCards[potentialChoice] > 0)
                {
                    _randomCardIntList.Add(potentialChoice);
                    currentBattleQuantityForCards[potentialChoice]--;
                }
            }

            for (int i = 0; i < _randomCardIntList.Count; i++)
            {
                currentBattleQuantityForCards[_randomCardIntList[i]]++;
            }
            

            for (int i = 0; i < _randomCardIntList.Count; i++)
            {
                _myFullHandTripleTriadCards[i].whatCardIAm =
                    currentBattleSelectableCards[_randomCardIntList[i]];
            }

            for (int i = 0; i < _myFullHandTripleTriadCards.Length; i++)
            {
                _myCurrentHandTripleTriadCards.Add(_myFullHandTripleTriadCards[i]);
            }
            ttMan.ttUi.UpdateMyHandImagesRandom();

            for (int i = 0; i < 5; i++)
            {
                ttMan.ttUi.PlayMyHandAnimation(i);
                SoundManager.instance.PlaySFX(6);
                yield return new WaitForSeconds(0.1f);

            }
            ttMan.SendStateChange(ttMan.enemyHandSelectionState);
            
        }

        public int RetrieveCurrentTripleTriadHandLength()
        {
            return _myCurrentHandTripleTriadCards.Count;
        }
        
        public int RetrieveFullTripleTriadHandLength()
        {
            return _myFullHandTripleTriadCards.Length;
        }


    }
}

