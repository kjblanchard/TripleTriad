
namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion
		

		#region Functions

		public bool CanIScrollRightInLocationSelection()
		{
			return ttDb.RetrieveLocationSelectionCurrentSelection() < ttUi.RetrieveLocationSelectionTransformCount()-1 && !ttUi.isLoading;
		}

		public bool CanIScrollLeftInLocationSelection()
		{
			return ttDb.RetrieveLocationSelectionCurrentSelection() > 0 && !ttUi.isLoading;
		}

		public bool CanIMoveDownInLocationSelection()
		{
			return ttDb.RetrieveLocationSelectionCurrentSelection() <
			       (ttUi.RetrieveLocationSelectionTransformCount() - 3) && !ttUi.isLoading;
		}

		public bool CanIMoveUpInLocationSelection()
		{
			return ttDb.RetrieveLocationSelectionCurrentSelection() > 2 && !ttUi.isLoading;
		}

		public bool CanIPlaceCardInLocationSelection()
		{
			return !ttDb.RetrieveTripleTriadCardInBoardSelection(ttDb.RetrieveLocationSelectionCurrentSelection()).cardInPlay && !ttUi.isLoading;
		}
		

		#endregion
	}
}
