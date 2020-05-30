using UnityEngine;

namespace ETF.TripleTriad
{

	public class RewardSelectionState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		private int whoWon;

		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			if (additionalArgs != 4)
			{
				whoWon = additionalArgs;
				_ttMan.ttDb.SetWhoWonTheGame(whoWon);
				if (whoWon == 1)
				{
					_ttMan.ttDb.InitializeRewardSelectionDb();
				}
				
			}

			if (whoWon == 4)
			{//coming back from card selection screen
				_ttMan.ttUi.GoingBackToRewardSelectionUi();
				_ttMan.ttUi.isLoading = false;
				

			}
		}

		public override void Execute()
		{
			if (whoWon != 1 && whoWon != 4) return;
			ListenForUserInputs();
			_ttMan.ttUi.keepRewardSelectionFingerOnProperLocation();

		}

		#region Functions

		private void ListenForUserInputs()
		{
			if ((Input.GetKeyDown(KeyCode.D) ||
			    Input.GetButtonDown("right")) && _ttMan.ttLogic.CanIScrollRightInRewardSelection())
			{
				_ttMan.ttDb.MoveRewardSelectionCurrentOptionRight();
				SoundManager.instance.PlaySFX(0);
				_ttMan.ttUi.UpdateTextInInfoPanelRewardSelection();

			}
			else if ((Input.GetKeyDown(KeyCode.A) ||
			         Input.GetButtonDown("left")) && _ttMan.ttLogic.CanIScrollLeftInRewardSelection())
			{
				_ttMan.ttDb.MoveRewardSelectionCurrentOptionLeft();
				SoundManager.instance.PlaySFX(0);
				_ttMan.ttUi.UpdateTextInInfoPanelRewardSelection();
			}

			else if ((Input.GetKeyDown(KeyCode.Space) ||
			         Input.GetButtonDown("Fire1")) && _ttMan.ttLogic.CanISelectInRewardSelection())
			{
				_ttMan.ttUi.FlipCurrentCardSelected();
			}
		}


		#endregion
	}
}
