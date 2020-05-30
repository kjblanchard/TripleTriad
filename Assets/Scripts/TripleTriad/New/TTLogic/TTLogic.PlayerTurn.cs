
namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion

		#region Functions

		public bool CanIScrollDownInPlayerTurn()
		{
			return ttDb.RetrievePlayerTurnCurrentSelection() < ttDb.RetrieveCurrentTripleTriadHandLength() -1 && ttDb.RetrieveCurrentTripleTriadHandLength() > 1 && !ttUi.isLoading;
		}

		public bool CanIScrollUpInPlayerTurn()
		{
			return ttDb.RetrievePlayerTurnCurrentSelection() > 0&& ttDb.RetrieveCurrentTripleTriadHandLength() > 1 && !ttUi.isLoading;
		}
		

		public bool CanILoopDownInPlayerTurn()
		{
			return ttDb.RetrievePlayerTurnCurrentSelection() == ttDb.RetrieveCurrentTripleTriadHandLength()-1 && ttDb.RetrieveCurrentTripleTriadHandLength() > 1 && !ttUi.isLoading;
		}

		public bool CanILoopUpInPlayerTurn()
		{
			return ttDb.RetrievePlayerTurnCurrentSelection() == 0 && ttDb.RetrieveCurrentTripleTriadHandLength() > 1 && !ttUi.isLoading;
		}

		#endregion
	}
}
