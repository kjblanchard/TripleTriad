using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion



		#region Functions

		public bool CanIScrollRightInRewardSelection()
		{
			return 
				!ttUi.isLoading && ttDb.RetrieveNumberOfCurrentSelectionInRewardSelection() > 0;
			
		}

		public bool CanIScrollLeftInRewardSelection()
		{
			return !ttUi.isLoading && ttDb.RetrieveNumberOfCurrentSelectionInRewardSelection() < 4;
		}

		public bool CanISelectInRewardSelection()
		{
			return !ttUi.isLoading;
		}

		public int EnemyRewardSelection()
		{
			var playerListOfCards = ttDb.RetrieveAllPlayerHandCardsFull();
			var highCardSpotInHand = 0;
			var highCardValue = 0;
			for (int i = 0; i < playerListOfCards.Length; i++)
			{
				//print($"I'm looking at {playerListOfCards[i].whatCardIAm.cardName}, and his ID is {playerListOfCards[i].whatCardIAm.cardID}");
				if (playerListOfCards[i].whatCardIAm.cardID > highCardValue)
				{
					//	print($"Since {playerListOfCards[i].whatCardIAm.cardName}, has a higher ID, he is now the highcard");
					highCardSpotInHand = i;
					highCardValue = playerListOfCards[i].whatCardIAm.cardID;
				}
			}

			return highCardSpotInHand;
		}


		#endregion
	}
}
