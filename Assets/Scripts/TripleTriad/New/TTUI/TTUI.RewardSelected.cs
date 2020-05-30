using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration
		[Header("RewardSelected Stuff")]
		[SerializeField] private Canvas _cardWonCanvas;
		[SerializeField] private Text _cardWonText;

		#endregion


		#region Functions

		public void InitializeRewardSelectedUiWin(string cardName)
		{
			_cardWonText.text = $"{cardName} card obtained!";
			_rewardConfirmCanvas.enabled = false;
			_cardRewardCanvas.enabled = false;
		}
		public void InitializeRewardSelectedUiLose(string cardName)
		{
			_cardWonText.text = $"{cardName} card lost..";
			_rewardConfirmCanvas.enabled = false;
			_cardRewardCanvas.enabled = false;
		}

		public void TurnOnCardWonCanvas()
		{
			_cardWonCanvas.enabled = true;
		}
		
		public void FinishedGameFadeOut()
		{
			_tripleTriadUiFade.animator.SetTrigger("finishGameFadeOut");
		}

		#endregion
	}
}
