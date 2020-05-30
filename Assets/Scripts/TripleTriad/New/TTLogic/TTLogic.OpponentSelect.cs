
namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion



		#region Functions

		public bool CanIScrollInOpponentSelect(TTDB.MovementDirections whichDirectionToMove)
		{
			var currentSpot = ttDb.RetrieveOpponentSelectionCurrentValue();
			switch (whichDirectionToMove)
			{
				case TTDB.MovementDirections.Up:
					return currentSpot > 2;
				case TTDB.MovementDirections.Right:
				return currentSpot < 8 && currentSpot != 2 && currentSpot != 5;
				case TTDB.MovementDirections.Down:
					return currentSpot < 6;
				case TTDB.MovementDirections.Left:
					return currentSpot > 0 && currentSpot != 3 && currentSpot != 6;
				default:
					return false;
				
			}
			
		}


		#endregion
	}
}
