namespace ETF.TripleTriad
{

	public class GameStartedState : TtState
	{

		#region Configuration
		
		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttUi.InitializeAndCacheAllUiElementsForGameStart();
			GameManager.instance.TurnOnDirectionalJoystick();
			_ttDb.InitializeDbValuesForStartingTripleTriad();
			_ttUi.StartGameFadeIn();

		}

		#region Functions



		#endregion
	}
}
