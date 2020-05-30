using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTDB
    {

        #region configuration

        [Header("EnemyHandReferences")] 
        private EnemyCardHand.WhatTypeOfCardPlayerAmI _typeOfEnemyPlayer;
        private List<Card> _currentEnemyRareCards;

        private List<Card> currentEnemyCardSelectionList = new List<Card>();
        public List<TripleTriadCardInHand> currentEnemyTripleTriadCardsInHand = new List<TripleTriadCardInHand>(); 
        public TripleTriadCardInHand[] fullEnemyTripleTriadCards;
        private string _whatToSayWhenTurnIsBeingSelected;
        private string _whatToSayWhenPlayerWins;
        private string _whatToSayWhenPlayerLoses;
        private string _whatToSayWhenPlayerTies;
        private string _whatToSayWhenChoosingCards;
        private string _npcName;
        private int _randomNumber;
        private int _chanceToUse;
        private EnemyCardHand.WhatTypeOfWinReward _typeOfWinReward;
        private bool _isEnemyHandOpen;
        private bool _isRandomCardSelection;
        private bool _isPlus;
        private bool _isSame;
        private string _opponentDescription;
        private int _minCardId;
        private int _maxCardId;

        #endregion
        
        public void BringInEnemyCardInformation(EnemyCardHand.WhatTypeOfCardPlayerAmI typeOfEnemyPlayer,
            List<Card> currentEnemyRareCards, string turnSelectionText, /*string playerWinText, string playerLoseText, string playerTieText,*/
            string choosingCardText, string npcName,bool isEnemyHandOpen,bool isRandomCardSelection,bool isPlus,bool isSame,EnemyCardHand.WhatTypeOfWinReward typeOfWinReward,string opponentDescription, int minCardId, int maxCardId)
        {
            //this is used to get information from the npc, it is called by enemy hand script, which will be on every npc..  Gets his rare cards, type of difficulty, and Information
            _typeOfEnemyPlayer = typeOfEnemyPlayer;
            _currentEnemyRareCards = currentEnemyRareCards;
            _whatToSayWhenTurnIsBeingSelected = turnSelectionText;
            _whatToSayWhenChoosingCards = choosingCardText;
            _npcName = npcName;
            _isEnemyHandOpen = isEnemyHandOpen;
            _isRandomCardSelection = isRandomCardSelection;
            _isPlus = isPlus;
            _isSame = isSame;
            _typeOfWinReward = typeOfWinReward;
            _opponentDescription = opponentDescription;
            _minCardId = minCardId;
            _maxCardId = maxCardId;
        }

        public int RetrieveMinCardLevel()
        {
            return _minCardId;
        }

        public int RetrieveMaxCardLevel()
        {
            return _maxCardId;
        }
        
        public bool RetrieveIsHandOpen()
        {
            return _isEnemyHandOpen;
        }

        public bool RetrieveIsRandomPlayerCards()
        {
            return _isRandomCardSelection;
        }
        

        public string RetrieveRuleEnemyHandOpen()
        {
            return _isEnemyHandOpen.ToString();
        }
        
        public string RetrieveRuleRandom()
        {
            return _isRandomCardSelection.ToString();
        }
        public string RetrieveRulePlus()
        {
            return _isPlus.ToString();
        }
        public string RetrieveRuleSame()
        {
            return _isSame.ToString();
        }
        public string RetrieveRuleWinReward()
        {
            return _typeOfWinReward.ToString();
        }


        public void GenerateEnemyCardHand()
        {
            currentEnemyCardSelectionList.Clear();
            ChooseCards();
        }

        private void ChooseCards()
        {
            {
                //chooses cards for the beginner ruleset
                if (_currentEnemyRareCards.Count > 0)
                {
                    DetermineIfRareCardsShouldBeAddedToHand();
                }
        
                AddCardsToHandSelectionsUntilFull();
                AddEnemySelectedHandToTheEnemyCardHandGameObjects();
                AddEnemySelectedCardsToHandList();
            }
        }
        
        private void DetermineIfRareCardsShouldBeAddedToHand()
        {
            for (var i = 0; i < _currentEnemyRareCards.Count; i++)
            {
                if (_currentEnemyRareCards[i].cardID == 999)
                {
                    return;
                    
                }
                _randomNumber = Random.Range(0, 100);
                _chanceToUse = _currentEnemyRareCards[i].percentChanceForEnemyToUseInBattle;
                if (_randomNumber <= _chanceToUse)
                {
                    currentEnemyCardSelectionList.Add(_currentEnemyRareCards[i]);
                    return;
                }
                
            }
        }

        private void AddCardsToHandSelectionsUntilFull()
        {
            var random = new System.Random();
            while (currentEnemyCardSelectionList.Count < 5)
            {
                  
                var randomNumber = random.Next(RetrieveMinCardLevel(), RetrieveMaxCardLevel() +1);  
            
                currentEnemyCardSelectionList.Add(CardInventory.instance.CardLookupById(randomNumber));
            }
        }

        private void AddEnemySelectedHandToTheEnemyCardHandGameObjects()
        {
            //uses the information in the DB to set the card gameobject so that is has a reference to the card list that was chosen
            for (int i = 0; i < fullEnemyTripleTriadCards.Length; i++)
            {
                fullEnemyTripleTriadCards[i].SetMyCurrentCard(currentEnemyCardSelectionList[i]);
            }
        }

        private void AddEnemySelectedCardsToHandList()
        {
            for (int i = 0; i < fullEnemyTripleTriadCards.Length; i++)
            {
                fullEnemyTripleTriadCards[i].cardOwnedByPlayer = false;
                currentEnemyTripleTriadCardsInHand.Add(fullEnemyTripleTriadCards[i]);
            }
        }




        public string WhatWillTheEnemySayWhenTurnSelection()
        {
            return _whatToSayWhenTurnIsBeingSelected;
        }

        public string WhatWillTheEnemySayWhenChoosingCards()
        {
            return _whatToSayWhenChoosingCards;
        }

        public string WhatIsTheEnemyNpcName()
        {
            return _npcName;
        }

        public bool CheckToSeeIfCardIsInRareList(int cardId)
        {
            for (int i = 0; i < _currentEnemyRareCards.Count; i++)
            {
                if (_currentEnemyRareCards[i].cardID == cardId)
                {
                    return true;
                }
            }

            return false;
        }
        
        public EnemyCardHand.WhatTypeOfCardPlayerAmI RetrieveEnemyDifficulty()
        {
            return _typeOfEnemyPlayer;
        }
        
    }
}
