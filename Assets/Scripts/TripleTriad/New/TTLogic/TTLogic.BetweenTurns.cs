
namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion



		#region Functions

		public bool CheckToSeeIfWeShouldEndTheGame()
		{
			return ttDb.NumberOfBoardCardsInPlay() >= 9;
		}

		#endregion
	}
}
