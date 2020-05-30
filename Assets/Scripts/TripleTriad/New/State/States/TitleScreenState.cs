using UnityEngine;

namespace ETF.TripleTriad
{

	public class TitleScreenState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		private TTUI _ttui;

		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttui = _ttMan.ttUi;
			_ttui.InitializeTitleScreenUi();
		}

		public override void Execute()
		{
			ListenForUserInput();
		}

		public override void End()
		{
			_ttui.TurnOffTitleScreenUi();
		}

		#region Functions

		private void ListenForUserInput()
		{
			if (!_ttui.isLoading)
			{
				if (Input.anyKey)
				{
					_ttui.isLoading = true;
					SoundManager.instance.PlaySFX(20);
					_ttui.TitleScreenFadeOut();
				}
			}
		}

		#endregion
	}
}
