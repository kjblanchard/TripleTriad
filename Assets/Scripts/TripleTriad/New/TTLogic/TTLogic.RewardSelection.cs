
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
				if (playerListOfCards[i].whatCardIAm.cardID > highCardValue)
				{
					highCardSpotInHand = i;
					highCardValue = playerListOfCards[i].whatCardIAm.cardID;
				}
			}

			return highCardSpotInHand;
		}


		#endregion
	}
}
