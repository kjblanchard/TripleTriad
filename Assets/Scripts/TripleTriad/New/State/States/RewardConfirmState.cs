using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public class RewardConfirmState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;

		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttMan.ttDb.InitializeRewardConfirmDb();
			//_ttMan.ttUi.isLoading = false;
			_ttMan.ttUi.InitializeRewardConfirmUi();
		}

		public override void Execute()
		{
			ListenForUserInput();
			_ttMan.ttUi.KeepFingerOnProperLocationInRewardConfirm();
		}

		public override void End()
		{
			
		}

		#region Functions

		private void ListenForUserInput()
		{
			if((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("right")) && _ttMan.ttLogic.CanIScrollRightInRewardConfirm())
			{

				_ttMan.ttDb.MoveCursorRightInRewardConfirm();
				SoundManager.instance.PlaySFX(0);

			}
			else if ((Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("left"))&& _ttMan.ttLogic.CanIScrollLeftInRewardConfirm())
			{
				_ttMan.ttDb.MoveCursorLeftInRewardConfirm();
				SoundManager.instance.PlaySFX(0);

			}
			else if ((Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2")) && _ttMan.ttLogic.CanISelectInRewardSelection())
			{
				_ttMan.ttDb.SetCursorToCancelRewardConfirmation();
			}

			else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) && _ttMan.ttLogic.CanISelectInRewardSelection())
			{
				if (_ttMan.ttDb.RetrieveCurrentSelectionInRewardConfirm() == 1)
				{
					_ttMan.ttUi.isLoading = true;
					_ttMan.ttUi.GoingBackToRewardSelectionUi();
					_ttMan.ttUi.FlipCurrentCardSelected();
					SoundManager.instance.PlaySFX(1);
				}
				else
				{
					_ttMan.ttUi.isLoading = true;
					_ttMan.SendStateChange(_ttMan.rewardSelectedState);
				}

			}
		}

		#endregion
	}
}
