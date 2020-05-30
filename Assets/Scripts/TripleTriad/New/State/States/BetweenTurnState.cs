using UnityEngine;

namespace ETF.TripleTriad
{

	public class BetweenTurnState : TtState
	{

		#region Configuration
		
		[SerializeField] private TripleTriadManager _ttMan;
		[SerializeField] private FingerAnimationChanger _turnSelectionFinger;

		#endregion

		
		public override void Startup(int additionalArgs = 0)
		{
			_ttUi.InitializeBetweenTurnStateUi();
			_ttUi.UpdateScoreCount();
			
			if (!_ttLogic.CheckToSeeIfWeShouldEndTheGame())
			{
				var stateComingFrom = _ttMan.RetrievePreviousState();
				if (stateComingFrom == _ttMan.locationSelectionState)
				{
					_turnSelectionFinger.PlayChangeToEnemyTurnAnimation();
				}
				else if (stateComingFrom == _ttMan.enemyTurnState)
				{
					_turnSelectionFinger.PlayChangeToPlayerTurnAnimation();
				}
			}else
			{
				_ttMan.SendStateChange(_ttMan.endGameState);
			}
			
		}



		#region Functions



		#endregion
	}
}
