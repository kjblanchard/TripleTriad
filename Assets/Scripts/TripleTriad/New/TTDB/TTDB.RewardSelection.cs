using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration

		private int _rewardSelectionCurrentOption;
		private int _whoWonTheGame;
		

		#endregion



		#region Functions

		public TripleTriadCardOnBoard[] RetrieveAllCardsOnBoard()
		{
			return _boardTripleTriadCards;
		}

		public TripleTriadCardInHand[] RetrieveAllPlayerHandCardsFull()
		{
			return _myFullHandTripleTriadCards;
		}
		public TripleTriadCardInHand RetrievePlayerHandCardAtCurrentPosition()
		{
			return _myFullHandTripleTriadCards[currentHandSelectionsList.Count ];
		}
		public TripleTriadCardInHand RetrievePlayerHandCardAtCancelPosition()
		{
			return _myFullHandTripleTriadCards[currentHandSelectionsList.Count - 1];
		}

		public Transform RetrieveCursorPositionInRewardScreen()
		{
			return fullEnemyTripleTriadCards[_rewardSelectionCurrentOption].fingerPointer.transform;
		}

		public int RetrieveNumberOfCurrentSelectionInRewardSelection()
		{
			return _rewardSelectionCurrentOption;
		}

		public void MoveRewardSelectionCurrentOptionRight()
		{
			_rewardSelectionCurrentOption--;
		}

		public void MoveRewardSelectionCurrentOptionLeft()
		{
			_rewardSelectionCurrentOption++;
		}

		public TripleTriadCardInHand RetrieveTripleTriadCardInEnemyHandSelected()
		{
			return fullEnemyTripleTriadCards[_rewardSelectionCurrentOption];
		}

		public void InitializeRewardSelectionDb()
		{
			_rewardSelectionCurrentOption = 4;
		}

		public int RetrieveWhoWonTheGame()
		{
			return _whoWonTheGame;
		}

		public void SetWhoWonTheGame(int whoWon)
		{
			_whoWonTheGame = whoWon;
		}

		public TripleTriadCardInHand RetrievePlayerTripleTriadCardAtLocation(int locationToGrab)
		{
			return _myFullHandTripleTriadCards[locationToGrab];
		}



		#endregion
	}
}
