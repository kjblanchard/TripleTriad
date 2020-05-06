using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration

		// private List<int> _positionToPlaceCardOnBoardOneCardList = new List<int>();
		// private List<int> _positionInHandForCardPlacementOneCardList = new List<int>();
		//
		// private  List<int> _positionToPlaceCardOnBoardTwoCardsList = new List<int>();
		// private List<int> _positionInHandForCardPlacementTwoCardsList = new List<int>();
		//
		// private  List<int> _positionToPlaceCardOnBoardThreeCardsList = new List<int>();
		// private List<int> _positionInHandForCardPlacementThreeCardsList = new List<int>();
		//
		// private  List<int> _positionToPlaceCardOnBoardFourCardsList = new List<int>();
		// private List<int> _positionInHandForCardPlacementFourCardsList = new List<int>();
		//
		private List<int> _positionToPlaceCardOnBoardRandomCardList = new List<int>();
		private List<int> _positionInHandForCardPlacementRandomCardList = new List<int>();
		
		private List<EnemyPotentialMove> _zeroCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _oneCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _twoCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _threeCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _fourCardMoveList = new List<EnemyPotentialMove>();

		private int  _enemySelectedBoardLocation;
		private int _enemySelectedHandPosition;

		//public bool isEnemyTurn;
		
		#endregion
	

		#region Functions

		public void ClearAllEnemyCardPlacementLists()
		{
			_positionToPlaceCardOnBoardRandomCardList.Clear();
			_zeroCardMoveList.Clear();
			_oneCardMoveList.Clear();
			_twoCardMoveList.Clear();
			_threeCardMoveList.Clear();
			_fourCardMoveList.Clear();

		}
		

		public void AddMoveToCardMoves(EnemyPotentialMove potentialMove, int whichListToAddTo)
		{
			switch (whichListToAddTo)
			{
				case 0:
					_zeroCardMoveList.Add(potentialMove);
					break;
				case 1:
					_oneCardMoveList.Add(potentialMove);
					break;
				case 2:
					_twoCardMoveList.Add(potentialMove);
					break;
				case 3:
					_threeCardMoveList.Add(potentialMove);
					break;
				case 4:
					_fourCardMoveList.Add(potentialMove);
					break;
			}
		}


		// public void AddBoardLocationToPotentialOneCardMoves(int boardLocationToAdd)
		// {
		// 	_positionToPlaceCardOnBoardOneCardList.Add(boardLocationToAdd);
		// }
		//
		// public void AddCardNumberToPotentialOneCardMoves(int cardNumToAdd)
		// {
		// 	_positionInHandForCardPlacementOneCardList.Add(cardNumToAdd);
		// }
		//
		// public void AddBoardLocationToPotentialTwoCardMoves(int boardLocationToAdd)
		// {
		// 	_positionToPlaceCardOnBoardTwoCardsList.Add(boardLocationToAdd);
		// }
		//
		// public void AddCardNumberToPotentialTwoCardMoves(int cardNumToAdd)
		// {
		// 	_positionInHandForCardPlacementTwoCardsList.Add(cardNumToAdd);
		// }
		//
		// public void AddBoardLocationToPotentialThreeCardMoves(int boardLocationToAdd)
		// {
		// 	_positionToPlaceCardOnBoardThreeCardsList.Add(boardLocationToAdd);
		// }
		//
		// public void AddCardNumberToPotentialThreeCardMoves(int cardNumToAdd)
		// {
		// 	_positionInHandForCardPlacementThreeCardsList.Add(cardNumToAdd);
		// }
		// public void AddBoardLocationToPotentialFourCardMoves(int boardLocationToAdd)
		// {
		// 	_positionToPlaceCardOnBoardFourCardsList.Add(boardLocationToAdd);
		// }
		//
		// public void AddCardNumberToPotentialFourCardMoves(int cardNumToAdd)
		// {
		// 	_positionInHandForCardPlacementFourCardsList.Add(cardNumToAdd);
		// }
		//
		public void AddBoardLocationToPotentialRandomCardMoves(int boardLocationToAdd)
		{
			_positionToPlaceCardOnBoardRandomCardList.Add(boardLocationToAdd);
		}
		//
		//
		// public int RetrieveNumberOfPotentialMovesOneCard()
		// {
		// 	return _positionToPlaceCardOnBoardOneCardList.Count;
		// }
		// public int RetrieveNumberOfPotentialMovesTwoCards()
		// {
		// 	return _positionToPlaceCardOnBoardTwoCardsList.Count;
		// }
		// public int RetrieveNumberOfPotentialMovesThreeCards()
		// {
		// 	return _positionToPlaceCardOnBoardThreeCardsList.Count;
		// }
		// public int RetrieveNumberOfPotentialMovesFourCards()
		// {
		// 	return _positionToPlaceCardOnBoardFourCardsList.Count;
		// }
		public int RetrieveNumberOfPotentialMovesRandomCards()
		{
			return _positionToPlaceCardOnBoardRandomCardList.Count;
		}
		//
		// public int FindCardToPlayInOneCardList(int numberInListToSearch)
		// {
		// 	return _positionInHandForCardPlacementOneCardList[numberInListToSearch];
		// }
		// public int FindBoardLocationToPlayInOneCardList(int numberInListToSearch)
		// {
		// 	return _positionToPlaceCardOnBoardOneCardList[numberInListToSearch];
		// }
		// public int FindCardToPlayInTwoCardList(int numberInListToSearch)
		// {
		// 	return _positionInHandForCardPlacementTwoCardsList[numberInListToSearch];
		// }
		// public int FindBoardLocationToPlayInTwoCardList(int numberInListToSearch)
		// {
		// 	return _positionToPlaceCardOnBoardTwoCardsList[numberInListToSearch];
		// }
		// public int FindCardToPlayInThreeCardList(int numberInListToSearch)
		// {
		// 	return _positionInHandForCardPlacementThreeCardsList[numberInListToSearch];
		// }
		// public int FindBoardLocationToPlayInThreeCardList(int numberInListToSearch)
		// {
		// 	return _positionToPlaceCardOnBoardThreeCardsList[numberInListToSearch];
		// }
		// public int FindCardToPlayInFourCardList(int numberInListToSearch)
		// {
		// 	return _positionInHandForCardPlacementFourCardsList[numberInListToSearch];
		// }
		// public int FindBoardLocationToPlayInFourCardList(int numberInListToSearch)
		// {
		// 	return _positionToPlaceCardOnBoardFourCardsList[numberInListToSearch];
		// }
		public int FindBoardLocationToPlayInRandomCardList(int numberInListToSearch)
		{
			return _positionToPlaceCardOnBoardRandomCardList[numberInListToSearch];
		}

		public EnemyPotentialMove FindMoveInList(int whichListToSearch, int whichPositionToGrab)
		{
			switch (whichListToSearch)
			{
				case 0:
					return _zeroCardMoveList[whichPositionToGrab];
				case 1:
					return _oneCardMoveList[whichPositionToGrab];
				case 2:
					return _twoCardMoveList[whichPositionToGrab];
				case 3:
					return _threeCardMoveList[whichPositionToGrab];
				case 4:
					return _fourCardMoveList[whichPositionToGrab];
				default: return null;
			}
		}

		public int RetrieveNumberOfMovesInList(int whichListToCheck)
		{
			switch (whichListToCheck)
			{
				case 0:
					return _zeroCardMoveList.Count;
				case 1:
					return _oneCardMoveList.Count;
				case 2:
					return _twoCardMoveList.Count;
				case 3:
					return _threeCardMoveList.Count;
				case 4:
					return _fourCardMoveList.Count;
				default: return 0;
			}
		}
		
		public List<EnemyPotentialMove> RetrieveMovesList(int whichListToGrab)
		{
			switch (whichListToGrab)
			{
				case 0:
					return _zeroCardMoveList;
				case 1:
					return _oneCardMoveList;
				case 2:
					return _twoCardMoveList;
				case 3:
					return _threeCardMoveList;
				case 4:
					return _fourCardMoveList;
				default: return null;
			}
		}

		
		public void ChangeBoardLocationToInPlayEnemy(int boardLocationToChange)
		{
			_boardTripleTriadCards[boardLocationToChange].ChangeToCardInPlay();
		}
		
		
		public void ModifyEnemyCurrentHandListWhenCardIsPlayed()
		{
			currentEnemyTripleTriadCardsInHand.RemoveAt(ttMan.ttLogic.RetrieveHandPositionToPlaceCard());
			for (int i = ttMan.ttLogic.RetrieveHandPositionToPlaceCard(); i < currentEnemyTripleTriadCardsInHand.Count; i++)
			{
				//_myCurrentHandTripleTriadCards[i].positionInCardHand--;
				currentEnemyTripleTriadCardsInHand[i].MoveCardUpOneSpot();
				//_myCurrentHandTripleTriadCards[i].MoveToNewLocationInHandWhenCardIsPlayed(_myCurrentHandTripleTriadCards[i].positionInCardHand);
			}
		}

		public void UpdateDbWithValuesForEnemyCardPlacement()
		{
			_enemySelectedHandPosition = ttMan.ttLogic.RetrieveHandPositionToPlaceCard();
			_enemySelectedBoardLocation = ttMan.ttLogic.RetrieveBoardPositionToPlaceCard();
		}

		public int RetrieveEnemySelectedHandPosition()
		{
			return _enemySelectedHandPosition;
		}

		public int RetrieveEnemySelectedBoardPosition()
		{
			return _enemySelectedBoardLocation;
		}
		
		



		#endregion
	}
}
