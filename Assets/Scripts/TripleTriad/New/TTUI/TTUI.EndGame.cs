using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration

		[Header("End Game Stuff")] [SerializeField]
		private Canvas _endGameCanvas;

		[SerializeField] private Animator _youWinAnimator;
		[SerializeField] private Animator _drawAnimator;
		[SerializeField] private Animator _youLoseAnimator;
		//[SerializeField] private Text _endGameText;




		#endregion



		#region Functions

		public void InitializeEndGameInitialUi()
		{
			isLoading = true;
			turnIndicatorFingerAnimator.gameObject.SetActive(false);
			_endGameCanvas.enabled = true;
		}
		
		
		public void InitializeWinEndGameUi()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer) 
			{
				SoundManager.instance.PlayBgm(3);
			}
			else
			{
				SoundManager.instance.PlayIntroLoop(3);
				SoundManager.instance.CacheNextMusic(9);
			}

			//UpdateEnemyDialogBoxToEndGameWin();
			//ttMan.ttUi.enemyHandDialogBoxAnimator.gameObject.SetActive(true);
			//PlayEnemyHandDialogBoxAnimation();
			//_endGameText.text = "You Win!";
			_youWinAnimator.SetTrigger("play");

		}
		public void InitializeTieEndGameUi()
		{
			//UpdateEnemyDialogBoxToEndGameTie();
			// ttMan.ttUi.enemyHandDialogBoxAnimator.gameObject.SetActive(true);
			// PlayEnemyHandDialogBoxAnimation();
			//_endGameText.text = "Tie..";
			//_youWinAnimator.SetTrigger("tie");
			_drawAnimator.SetTrigger("play");

		}
		public void InitializeLoseEndGameUi()
		{
			SoundManager.instance.PlaySFX(16);
			//UpdateEnemyDialogBoxToEndGameLose();
			// ttMan.ttUi.enemyHandDialogBoxAnimator.gameObject.SetActive(true);
			// PlayEnemyHandDialogBoxAnimation();
			//_endGameText.text = "Loser!";
			_youLoseAnimator.SetTrigger("play");

		}
		
		
		
		// private void UpdateEnemyDialogBoxToEndGameWin()
		// {
		// 	enemyDialogBoxDialogText.text = ttdb.WhatWillTheEnemySayWhenPlayerWins();
		// }
		//
		// public void UpdateEnemyDialogBoxToEndGameLose()
		// {
		// 	enemyDialogBoxDialogText.text = ttdb.WhatWillTheEnemySayWhenPlayerLoses();
		// }
		//
		// public void UpdateEnemyDialogBoxToEndGameTie()
		// {
		// 	enemyDialogBoxDialogText.text = ttdb.WhatWillTheEnemySayWhenPlayerTies();
		// }


		#endregion
	}
}
