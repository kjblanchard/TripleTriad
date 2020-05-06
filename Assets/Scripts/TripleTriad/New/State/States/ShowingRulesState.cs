using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public class ShowingRulesState: TtState
	{
		private static readonly int kClosing = Animator.StringToHash("closing");

		#region Configuration

		//[SerializeField] private TripleTriadManager _ttMan;

		#endregion


		public override void Startup(int additionalArgs = 0)
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer) 
			{
				SoundManager.instance.PlayBgm(2);
			}
			else
			{
				SoundManager.instance.PlayIntroLoop(1);
				SoundManager.instance.CacheNextMusic(3);
			}
			

			_ttMan.ttDb.InitializeShowingRulesDbValues();
			_ttMan.ttUi.InitializeShowingRulesUi();
		}

		public override void Execute()
		{
			ListenForUserInputs();
			_ttUi.KeepFingerOnProperLocationInShowingRules();
		}

		
		#region Functions

		private void ListenForUserInputs()
		{
			if (_ttUi.isLoading) return;
			if ((Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("right")) &&
			    _ttLogic.CanIScrollRightOnRuleSelection())
			{
				_ttDb.ScrollRightInShowingRulesDb();
				SoundManager.instance.PlaySFX(0);
			}
			else if ((Input.GetKeyDown(KeyCode.A) || Input.GetButtonDown("left")) &&
			         _ttLogic.CanIScrollLeftOnRuleSelection())
			{
				_ttDb.ScrollLeftInShowingRulesDb();
				SoundManager.instance.PlaySFX(0);

			}
			else if ((Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire2")) &&
			         _ttLogic.CanIPressCancelOnRuleSelection())
			{
				CancelButtonInShowingRules();
			}
			else if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1")) &&
			         _ttDb.ReturnFingerPositionInShowingRules() == 0)

			{
				StartCoroutine(ChangeToCardSelectionScreen());
			}
		}

		private void CancelButtonInShowingRules()
        {
            _ttMan.ttDb.CancelButtonInShowingRules();
            SoundManager.instance.PlaySFX(1);
        }

		private IEnumerator ChangeToCardSelectionScreen()
		{
			_ttUi.isLoading = true;
			_ttUi.showingRulesAnimator.SetTrigger(kClosing);
			yield return new WaitForSeconds(0.5f);
			if (_ttMan.ttDb.RetrieveIsRandomPlayerCards())
			{
				_ttMan.SendStateChange(_ttMan.cardSelectionState,2);
			}
			else
			{
				_ttMan.SendStateChange(_ttMan.cardSelectionState);

			}

		}

		#endregion
	}
}
