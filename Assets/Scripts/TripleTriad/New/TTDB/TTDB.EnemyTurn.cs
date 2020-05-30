using System.Collections.Generic;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration
		
		private List<int> _positionToPlaceCardOnBoardRandomCardList = new List<int>();
		private List<int> _positionInHandForCardPlacementRandomCardList = new List<int>();
		
		private List<EnemyPotentialMove> _zeroCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _oneCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _twoCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _threeCardMoveList = new List<EnemyPotentialMove>();
		private List<EnemyPotentialMove> _fourCardMoveList = new List<EnemyPotentialMove>();

		private int  _enemySelectedBoardLocation;
		private int _enemySelectedHandPosition;

		
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
		
		public void AddBoardLocationToPotentialRandomCardMoves(int boardLocationToAdd)
		{
			_positionToPlaceCardOnBoardRandomCardList.Add(boardLocationToAdd);
		}
		public int RetrieveNumberOfPotentialMovesRandomCards()
		{
			return _positionToPlaceCardOnBoardRandomCardList.Count;
		}
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
				currentEnemyTripleTriadCardsInHand[i].MoveCardUpOneSpot();
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
