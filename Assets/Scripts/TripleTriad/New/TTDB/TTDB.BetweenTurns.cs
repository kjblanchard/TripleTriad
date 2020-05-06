using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration


		#endregion
	

		#region Functions

		public int NumberOfBoardCardsInPlay()
		{
			var numberOfCardsInPlay = 0;
			for (int i = 0; i < _boardTripleTriadCards.Length ; i++)
			{
				if (_boardTripleTriadCards[i].cardInPlay)
				{
					numberOfCardsInPlay++;
				}
			}

			return numberOfCardsInPlay;
		}

		public int NumberOfPlayerOwnedCards()
		{
			var cardsOwnedByPlayer = 0;
			for (int i = 0; i < _boardTripleTriadCards.Length; i++)
			{
				if (_boardTripleTriadCards[i].cardInPlay && _boardTripleTriadCards[i].cardOwnedByPlayer)
				{
					cardsOwnedByPlayer++;
				}
			}

			cardsOwnedByPlayer += _myCurrentHandTripleTriadCards.Count;
			return cardsOwnedByPlayer;
		}
		public int NumberOfEnemyOwnedCards()
		{
			var cardsOwnedByEnemy = 0;
			for (int i = 0; i < _boardTripleTriadCards.Length; i++)
			{
				if (_boardTripleTriadCards[i].cardInPlay && !_boardTripleTriadCards[i].cardOwnedByPlayer)
				{
					cardsOwnedByEnemy++;
				}
			}

			cardsOwnedByEnemy += currentEnemyTripleTriadCardsInHand.Count;
			return cardsOwnedByEnemy;
		}
		


		#endregion
	}
}
