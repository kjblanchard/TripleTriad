
namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration


		#endregion
	

		#region Functions

		public void PlayCardInEnemyTurn(int enemyHandCardNumber)
		{
			ttdb.currentEnemyTripleTriadCardsInHand[enemyHandCardNumber].CardPlayed();
			SoundManager.instance.PlaySFX(6);
		}

		public void InitializeEnemyTurnStateUi()
		{
			enemyHandDialogBoxAnimator.gameObject.SetActive(false);
			locationSelectionFinger.SetActive(false);
		}
		
		
		public void setFocusOnEnemyCardSelected()
		{
			if (ttMan.ttLogic.RetrieveHandPositionToPlaceCard() != ttdb.currentEnemyTripleTriadCardsInHand.Count-1)
			{
				ttdb.currentEnemyTripleTriadCardsInHand[ttdb.currentEnemyTripleTriadCardsInHand.Count -1].cardAnimator.SetTrigger("outFocus");
				ttdb.currentEnemyTripleTriadCardsInHand[ttMan.ttLogic.RetrieveHandPositionToPlaceCard()].cardAnimator.SetTrigger("inFocus");
			}
			else
			{
				ttdb.currentEnemyTripleTriadCardsInHand[ttMan.ttLogic.RetrieveHandPositionToPlaceCard()].cardAnimator.SetTrigger("inFocus");

			}
			
		}

		public void setOutFocusOnEnemyCardSelected()
		{
			ttdb.currentEnemyTripleTriadCardsInHand[ttMan.ttLogic.RetrieveHandPositionToPlaceCard()].cardAnimator.SetTrigger("outFocus");

		}

		public void InitializeBetweenTurnUiPlayerToEnemy()
		{
			isLoading = true;
			playerSelectionFinger.SetActive(false);
		}
		
		public void InitializeBetweenTurnUiEnemyToPlayer()
		{
			isLoading = false;
		}

		#endregion
	}
}
