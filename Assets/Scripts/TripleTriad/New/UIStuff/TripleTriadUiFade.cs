using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public class TripleTriadUiFade : MonoBehaviour
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		[SerializeField] public Animator animator;

		#endregion

		

		#region Functions

		public void FadeInRewardScreen()
		{
			_ttMan.ttUi.RewardSelectionFadeIn();
		}

		public void StartShootingCards()
		{
			StartCoroutine(_ttMan.ttUi.MoveAllPlayerHandCardsToRewardSelection());
			StartCoroutine(_ttMan.ttUi.MoveAllEnemyHandCardsToRewardSelection());
		}

		public void ReturnBoardCardsToBase()
		{
			_ttMan.ttUi.ReturnAllBoardCardsToBase();
		}

		public void ReturnHandCardsToPreReward()
		{
			_ttMan.ttUi.MoveAllPlayerHandCardsToPreReward();
			_ttMan.ttUi.MoveAllEnemyHandCardsToPreReward();
	
		}

		public void InitializeRewardScreenUi()
		{
			_ttMan.ttUi.InitializeRewardScreenUiElements();
		}

		public void QuitGame()
		{
			//_ttMan.SendStateChange(_ttMan.showingRulesState,1);
			_ttMan.SendStateChange(_ttMan.gameStartedState);
			//TurnOffTripleTriad();
			//Application.Quit();
		}

		public void ChangeToShowingRulesState()
		{
			_ttMan.SendStateChange(_ttMan.showingRulesState);
		}

		// public void TurnOffTripleTriad()
		// {
		// 	SoundManager.instance.PlayIntroLoop(SoundManager.instance.bgmToPlay);
		// 	StartCoroutine(UIFade.instance.EndCardBattleFade(0.5f));
		//
		// }
		
		public void ChangeToTitleScreenState()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer) 
			{
				SoundManager.instance.PlayBgm(0);
			}
			else
			{
				SoundManager.instance.PlayIntroLoop(9);
				SoundManager.instance.CacheNextMusic(10);
			}

			_ttMan.SendStateChange(_ttMan.titleScreenState);

		}

		private void IsLoading()
		{
			_ttMan.ttUi.isLoading = true;
		}

		private void IsNotLoading()
		{
			_ttMan.ttUi.isLoading = false;
		}
		
		public void StartShowingRulesFadeIn()
		{
			animator.SetTrigger("showingRulesFadeIn");
		}
		public void SwitchToShowingRulesState()
		{
			_ttMan.SendStateChange(_ttMan.showingRulesState);
		}
		
		public void StartTitleScreenFadeOut()
		{
			animator.SetTrigger("titleScreenFadeOut");
		}

		public void SwitchToOpponentSelectionState()
		{
			_ttMan.SendStateChange(_ttMan.opponentSelectState);
		}

		public void TurnOffParticleSystem()
		{
			
		}
		public void TurnOnParticleSystem()
		{
			
		}
		

		#endregion
	}
}
