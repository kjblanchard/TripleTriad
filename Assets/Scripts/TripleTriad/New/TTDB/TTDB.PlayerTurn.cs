
namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration
		
		private int _playerTurnCurrentSelection;
		private bool _isComingFromLocationCancel;

		private int _currentCardsInHand;



		#endregion
		

		#region Functions

		public int RetrievePlayerTurnCurrentSelection()
		{
			return _playerTurnCurrentSelection;
		}

		public void MovePlayerTurnCurrentSelectionDown()
		{
			_playerTurnCurrentSelection++;
		}

		public void MovePlayerTurnCurrentSelectionUp()
		{
			_playerTurnCurrentSelection--;
		}

		public void MovePlayerTurnCurrentSelectionTop()
		{
			_playerTurnCurrentSelection = 0;
		}

		public void MovePlayerTurnCurrentSelectionToBottom()
		{
			_playerTurnCurrentSelection = _myCurrentHandTripleTriadCards.Count - 1;
		}

		public void InitializePlayerTurnDbValues()
		{
			_playerTurnCurrentSelection = 0;
		}

		public bool RetrievePlayerTurnIsComingFromCancelScreen()
		{
			return _isComingFromLocationCancel;
		}
		
		#endregion
	}
}
