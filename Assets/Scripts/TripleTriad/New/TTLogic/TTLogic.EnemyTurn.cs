using System.Collections;
using System.Collections.Generic;
using ETF.TripleTriad;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTLogic
	{


		#region Configuration

		private TripleTriadCardOnBoard _potentialEmptyTripleTriadSpotToPlay;
		private int _howManyCardsCanIFlipHere;
		private TripleTriadCardOnBoard _cardToCheckAgainst;
		private int _myCurrentValue;
		private int _enemySideToCheck;
		private int _enemyCurrentValue;
		private int _fourCardMoves;
		private int _threeCardMoves;
		private int _twoCardMoves;
		private int _oneCardMoves;
		private int _randomCardMoves;
		private int _cardInListToPlay;
		private int _boardLocInListToPlay;

		private EnemyPotentialMove _moveToPlay;
		private int _boardLocationToPlaceCard;


		#endregion
	

		#region Functions

		private void SearchForPotentialMovesOnBoard(Card enemyCard, int cardNumberInInventory)
		{
			//for each board location
			for (int i = 0; i < ttDb.RetrieveBoardLocationsCount(); i++)
			{
				if (!ttDb.RetrieveTripleTriadCardInBoardSelection(i).cardInPlay)
				{
					//that is not in play
					//var potentialSpotToPlay = i;
					_potentialEmptyTripleTriadSpotToPlay = ttDb.RetrieveTripleTriadCardInBoardSelection(i);
					_howManyCardsCanIFlipHere = 1;
					for (int j = 0; j < _potentialEmptyTripleTriadSpotToPlay.locationBoardAdjacency.Length; j++)
					{
						//search adjacent sides
						if (ttDb.RetrieveTripleTriadCardInBoardSelection((_potentialEmptyTripleTriadSpotToPlay
							.locationBoardAdjacency[j])).cardInPlay && ttDb
							.RetrieveTripleTriadCardInBoardSelection((_potentialEmptyTripleTriadSpotToPlay
								.locationBoardAdjacency[j])).cardOwnedByPlayer)
						{
							//that are in play
							//var adjacentSideNumber = j;
							_cardToCheckAgainst = ttDb.RetrieveTripleTriadCardInBoardSelection(
								_potentialEmptyTripleTriadSpotToPlay
									.locationBoardAdjacency[j]);
							_myCurrentValue =
								enemyCard.cardValues[_potentialEmptyTripleTriadSpotToPlay.myValueToCheck[j]];
							_enemySideToCheck = _potentialEmptyTripleTriadSpotToPlay.adjacencyValueToCheck[j];
							_enemyCurrentValue = _cardToCheckAgainst.whatCardIAm.cardValues[_enemySideToCheck];
							if (_myCurrentValue > _enemyCurrentValue)
							{
								//check to see if my value is higher than theirs
								//print($"My Value is higher than theirs with card {enemyCard.cardName}, and I'm going to go to position {i} cause my value is {_myCurrentValue} and their value is {_enemyCurrentValue}");
								var potentialMove = new EnemyPotentialMove(i, cardNumberInInventory,enemyCard);
								ttDb.AddMoveToCardMoves(potentialMove, _howManyCardsCanIFlipHere);
								_howManyCardsCanIFlipHere++;
							}
						}
					}
				}
			}

			//ChooseCardToPlayFromListOfChoices();
		}

		private void SearchForRandomCardPlacementBeginner()
		{
			for (int i = 0; i < ttDb.RetrieveBoardLocationsCount(); i++)
			{
				if (ttDb.RetrieveTripleTriadCardInBoardSelection(i).cardInPlay == false)
				{
					ttDb.AddBoardLocationToPotentialRandomCardMoves(i);
				}
			}
		}

		private void CreateModerateRandomCardList()
		{
			var boardCards = ttDb.RetrieveAllCardsOnBoard();
			for (int i = 0; i < ttDb.currentEnemyTripleTriadCardsInHand.Count; i++)
			{
				for (int j = 0; j < boardCards.Length; j++)
				{
					if (boardCards[j].cardInPlay == false)
					{
						var potentialMove = new EnemyPotentialMove(j,i,ttDb.currentEnemyTripleTriadCardsInHand[i].whatCardIAm);
						ttDb.AddMoveToCardMoves(potentialMove,0);
					}


				}
			}
		}
		
		public void FullEnemyTurnHandPlacementChoices()
		{
			for (int i = 0; i < ttDb.currentEnemyTripleTriadCardsInHand.Count; i++)
			{
				SearchForPotentialMovesOnBoard(ttDb.currentEnemyTripleTriadCardsInHand[i].whatCardIAm,i);
					
			}
		}

		public void ChooseCardToPlayFromListOfChoices()
		{
			var enemyDifficulty = ttDb.RetrieveEnemyDifficulty();
			switch(enemyDifficulty)
			{
				case EnemyCardHand.WhatTypeOfCardPlayerAmI.Beginner:
					ChooseCardMoveAndReturnCardToPlayInHandBeginner();
					break;
				case EnemyCardHand.WhatTypeOfCardPlayerAmI.Moderate:
					ChooseCardMoveAndReturnCardToPlayInHandModerate();
					break;
				case EnemyCardHand.WhatTypeOfCardPlayerAmI.Hard:
					ChooseCardMoveAndReturnCardToPlayInHandModerate();
					break;
				case EnemyCardHand.WhatTypeOfCardPlayerAmI.ChooseAllMyCards:
					break;
			}
		}

		private void ChooseCardMoveAndReturnCardToPlayInHandBeginner()
		{
			int whichCardList;
			GatherPotentialMoveCountsFromDb();
			
			if (_fourCardMoves > 0)
			{
				whichCardList = 4;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_threeCardMoves > 0)
			{
				
				whichCardList = 3;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_twoCardMoves > 0)
			{
				whichCardList = 2;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_oneCardMoves > 0)
			{
				whichCardList = 1;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else
			{
				//print("noCardsCanBeFlipped");
				SearchForRandomCardPlacementBeginner();
				_randomCardMoves = ttDb.RetrieveNumberOfPotentialMovesRandomCards();
				_boardLocInListToPlay = Random.Range(0, _randomCardMoves);
				_boardLocationToPlaceCard =  ttDb.FindBoardLocationToPlayInRandomCardList(_boardLocInListToPlay);
				_moveToPlay = new EnemyPotentialMove(_boardLocationToPlaceCard,Random.Range(0, ttDb.currentEnemyTripleTriadCardsInHand.Count));
			}
		}
		private void ChooseCardMoveAndReturnCardToPlayInHandModerate()
		{
			int whichCardList;
			GatherPotentialMoveCountsFromDb();
			
			if (_fourCardMoves > 0)
			{
				whichCardList = 4;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_threeCardMoves > 0)
			{
				
				whichCardList = 3;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_twoCardMoves > 0)
			{
				whichCardList = 2;
				_cardInListToPlay = Random.Range(0, ttDb.RetrieveNumberOfMovesInList(whichCardList));
				_moveToPlay = ttDb.FindMoveInList(whichCardList, _cardInListToPlay);

			}
			else if (_oneCardMoves > 0)
			{
				whichCardList = 1;
				_moveToPlay = ModerateChooseBestCard(whichCardList);

			}
			else
			{
				whichCardList = 0;
				CreateModerateRandomCardList();
				_moveToPlay = ModerateChooseBestCard(whichCardList);
			}
			//print($"my moves defense is {_moveToPlay.RetrieveDefense()}");
		}

		private EnemyPotentialMove ModerateChooseBestCard(int whichCardList)
		{
			var movesList = ttDb.RetrieveMovesList(whichCardList);
			for (int i = 0; i < movesList.Count; i++)
			{
				var potentialMoveBoardLocation = ttDb.RetrieveTripleTriadCardInBoardSelection(movesList[i].RetrieveBoardLocation());
				for (int j = 0; j < potentialMoveBoardLocation.locationBoardAdjacency.Length; j++)
				{
					var locationToCheckIfInPlay = ttDb.RetrieveTripleTriadCardInBoardSelection(
						potentialMoveBoardLocation.locationBoardAdjacency[j]);
					if (locationToCheckIfInPlay.cardInPlay)
					{
						//there is a card there, so add 10 to the defense value
						//print($"adding defense of 10 and my card is {movesList[i].RetrieveCard()} cause there is a card in play at position {locationToCheckIfInPlay}");
						movesList[i].AddToDefenseValue(10);
					}
					else
					{//calculate the defense value to add
						var cardDefensevalue = movesList[i].RetrieveCard()
							.cardValues[potentialMoveBoardLocation.myValueToCheck[j]];
						 // print(
						 // 	$"I'm adding the defense value of {cardDefensevalue} and my card is {movesList[i].RetrieveCard()} cause the card isn't in play at position {locationToCheckIfInPlay}  ");

						movesList[i].AddToDefenseValue(cardDefensevalue);
					}
				}
				
			}

			for (int i = 0; i < movesList.Count; i++)
			{
				movesList[i].CalculateDefenseValue();
			}
			movesList.Sort();
			return movesList[0];
		}

		private void GatherPotentialMoveCountsFromDb()
		{
			//print("hi");
			_fourCardMoves = ttDb.RetrieveNumberOfMovesInList(4);
			_threeCardMoves = ttDb.RetrieveNumberOfMovesInList(3);
			_twoCardMoves = ttDb.RetrieveNumberOfMovesInList(2);
			_oneCardMoves = ttDb.RetrieveNumberOfMovesInList(1);

		}

		public int RetrieveBoardPositionToPlaceCard()
		{
			return _moveToPlay.RetrieveBoardLocation();
		}
		
		public int RetrieveHandPositionToPlaceCard()
		{
			return _moveToPlay.RetrieveCardLocation();
		}
		
		#endregion
	}
}
