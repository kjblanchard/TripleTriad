using UnityEngine;

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


			_youWinAnimator.SetTrigger("play");

		}
		public void InitializeTieEndGameUi()
		{

			_drawAnimator.SetTrigger("play");

		}
		public void InitializeLoseEndGameUi()
		{
			SoundManager.instance.PlaySFX(16);
			_youLoseAnimator.SetTrigger("play");

		}
		
		#endregion
	}
}
