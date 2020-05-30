using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration
		[Header("Title Screen Stuff")]
		[SerializeField] private Canvas _titleScreenCanvas;

		[SerializeField] private ParticleSystem _starParticleSystem;

		#endregion



		#region Functions

		public void InitializeTitleScreenUi()
		{
			_starParticleSystem.gameObject.SetActive(true);
			_titleScreenCanvas.enabled = true;
		}

		public void TurnOffTitleScreenUi()
		{
			_starParticleSystem.gameObject.SetActive(false);
			_titleScreenCanvas.enabled = false;
		}

		public void TitleScreenFadeOut()
		{
			_tripleTriadUiFade.StartTitleScreenFadeOut();
		}

		public void TurnOnParticleSystem()
		{
			_starParticleSystem.gameObject.SetActive(true);
		}
		public void TurnOffParticleSystem()
		{
			_starParticleSystem.gameObject.SetActive(false);
		}


		#endregion
	}
}
