using UnityEngine;

namespace ETF.TripleTriad
{

	public class GameEndedState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		
		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttMan.ttUi.FinishedGameFadeOut();
		}

		#region Functions



		#endregion
	}
}
