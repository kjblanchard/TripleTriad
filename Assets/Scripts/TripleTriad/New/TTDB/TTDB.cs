using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTDB : MonoBehaviour
    {

        #region configuration

        [Header("Card Battle References")] private int _numberofBattleCardsOnPage = 10;
        private int _totalNumberOfBattleCards;
        private int _totalNumberOfBattlePages;
        private int _totalNumberOfCardsOnLastBattlePage;
        public List<Card> currentBattleSelectableCards = new List<Card>();
        public List<int> currentBattleQuantityForCards = new List<int>();


        [SerializeField] TripleTriadManager ttMan;

        #endregion

        #region public Methods
        
        public void CalculateTtBattleNumbers()
        {
            _totalNumberOfBattleCards = CardInventory.instance.RetrieveTotalNumberOfBattleCards();
            var notRoundedUpBattlePageCount = ((float) _totalNumberOfBattleCards) / _numberofBattleCardsOnPage;
            _totalNumberOfBattlePages = (int) Mathf.Ceil(notRoundedUpBattlePageCount);
            _totalNumberOfCardsOnLastBattlePage = (int) _totalNumberOfBattleCards % _numberofBattleCardsOnPage;
        }

        public int RetrieveTotalNumberOfCardsOnPage()
        {
            return _numberofBattleCardsOnPage;
        }

        public int RetrieveTotalNumberOfBattleCards()
        {
            return _totalNumberOfBattleCards;
        }

        public int RetrieveTotalNumberOfBattlePages()
        {
            return _totalNumberOfBattlePages;
        }

        public int RetrieveTotalNumberOfCardsOnLastBattlePage()
        {
            return _totalNumberOfCardsOnLastBattlePage;
        }

        public void ClearBattleSelectionsList()
        {
            currentHandSelectionsList.Clear();
        }

        public void BringInUsableBattleCards()
        {
            currentBattleSelectableCards = CardInventory.instance.RetrieveBattleCardList();
            currentBattleQuantityForCards = CardInventory.instance.RetrieveBattleQuantityList();
        }

        public void InitializeDbValuesForStartingTripleTriad()
        {
            var emptyCardReference = CardInventory.instance.emptyCard;
            EmptyAllGameCards(emptyCardReference);
        }
        #endregion

        #region private Methods
        
        private void EmptyBoardCards(Card emptyCardReference)
        {
            for (int i = 0; i < _boardTripleTriadCards.Length; i++)
            {
                _boardTripleTriadCards[i].cardInPlay = false;
                _boardTripleTriadCards[i].cardOwnedByPlayer = false;
                _boardTripleTriadCards[i].whatCardIAm = emptyCardReference;
            }
        }

        private void EmptyAllGameCards(Card emptyCardReference)
        {
            EmptyPlayerCards(emptyCardReference);
            EmptyEnemyCards(emptyCardReference);
            EmptyBoardCards(emptyCardReference);
            currentEnemyTripleTriadCardsInHand.Clear();
            _myCurrentHandTripleTriadCards.Clear();
        }

        private void EmptyEnemyCards(Card emptyCardReference)
        {
            for (int i = 0; i < fullEnemyTripleTriadCards.Length; i++)
            {
                fullEnemyTripleTriadCards[i].whatCardIAm = emptyCardReference;
            }
        }

        private void EmptyPlayerCards(Card emptyCardReference)
        {
            for (int i = 0; i < _myFullHandTripleTriadCards.Length; i++)
            {
                _myFullHandTripleTriadCards[i].whatCardIAm = emptyCardReference;
            }
        }
        #endregion
    }
}