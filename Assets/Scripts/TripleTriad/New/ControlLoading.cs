using UnityEngine;

namespace ETF.TripleTriad
{

	public class ControlLoading : MonoBehaviour
	{

		#region Configuration

		private TTUI _ttui;

		#endregion

		private void Awake()
		{
			_ttui = FindObjectOfType<TTUI>();
		}

		#region Functions

		private void ControlUiLoadingModifier(int shouldIBeLoading)
		{
			if (shouldIBeLoading == 0)
			{
				_ttui.isLoading = true;
			}
			else
			{
				_ttui.isLoading = false;
			}
		}


		private void TurnOffShowingRulesCanvas()
		{
			_ttui.TurnOffShowingRulesCanvas();
		}

		private void TurnOffCardSelectCanvas()
		{
			_ttui.TurnOffCardSelectionCanvas();
		}
		
		private void TurnOffCardConfirmCanvas()
		{
			_ttui.TurnOffCardConfirmCanvas();
		}

		private void StartShowingRulesFade()
		{
			_ttui.StartShowingRulesFadeOut();
		}
		

		#endregion
	}
}
