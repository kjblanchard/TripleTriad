using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration

		private int _currentFingerLocationInLocationSelection;
		[SerializeField] private TripleTriadCardOnBoard[] _boardTripleTriadCards;
		

		#endregion


		#region Functions

		public int RetrieveLocationSelectionCurrentSelection()
		{
			return _currentFingerLocationInLocationSelection;
		}

		public void MoveLocationFingerRightInDb()
		{
			_currentFingerLocationInLocationSelection++;
		}

		public void MoveLocationFingerLeftInDb()
		{
			_currentFingerLocationInLocationSelection--;
		}

		public void MoveLocationFingerDownInDb()
		{
			_currentFingerLocationInLocationSelection += 3;
		}

		public void MoveLocationFingerUpInDb()
		{
			_currentFingerLocationInLocationSelection -= 3;
		}

		public TripleTriadCardInHand RetrieveTripleTriadCardInPlayerSelection()
		{
			return _myCurrentHandTripleTriadCards[_playerTurnCurrentSelection];
		}
		public TripleTriadCardInHand RetrieveNextTripleTriadCardInPlayerSelection()
		{
			return _myFullHandTripleTriadCards[_playerTurnCurrentSelection + 1];
		}

		public TripleTriadCardOnBoard RetrieveTripleTriadCardInBoardSelection(int boardLocation)
		{
			return _boardTripleTriadCards[boardLocation];
		}

		public TripleTriadCard RetrieveCard2InMyHandTest()
		{
			return _myFullHandTripleTriadCards[1];
		}
		
		
		public void InitializeLocationSelectionDbValues()
		{
			_currentFingerLocationInLocationSelection = 4;
		}

		public void modifyCurrentHandListWhenCardIsPlayed()
		{
			_myCurrentHandTripleTriadCards.RemoveAt(_playerTurnCurrentSelection);
			for (int i = _playerTurnCurrentSelection; i < _myCurrentHandTripleTriadCards.Count; i++)
			{
				//_myCurrentHandTripleTriadCards[i].positionInCardHand--;
				_myCurrentHandTripleTriadCards[i].MoveCardUpOneSpot();
				//_myCurrentHandTripleTriadCards[i].MoveToNewLocationInHandWhenCardIsPlayed(_myCurrentHandTripleTriadCards[i].positionInCardHand);
			}
		}

		public void ChangeCurrentLocationCardToPlayed()
		{
			_boardTripleTriadCards[_currentFingerLocationInLocationSelection].ChangeToCardInPlay();
		}

		public int RetrieveBoardLocationsCount()
		{
			return _boardTripleTriadCards.Length;
		}

		public bool CheckIfCardIsInTheBoardLocation()
		{
			return _boardTripleTriadCards[_currentFingerLocationInLocationSelection].cardInPlay;
		}
		

		#endregion
	}
}
