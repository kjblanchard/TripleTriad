using UnityEngine;

namespace ETF.TripleTriad
{

	public class RewardSelectedState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		private TripleTriadCardInHand _wonCard;
		private int _whoWonTheGame;

		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_whoWonTheGame = _ttMan.ttDb.RetrieveWhoWonTheGame();

			if (_whoWonTheGame == 1)
			{//playerwon
				_wonCard = _ttMan.ttDb.fullEnemyTripleTriadCards[
					_ttMan.ttDb.RetrieveNumberOfCurrentSelectionInRewardSelection()];
				_ttMan.ttUi.InitializeRewardSelectedUiWin(_wonCard.whatCardIAm.cardName);
				_wonCard.cardAnimator.SetTrigger("rewardSelected");
			}
			else
			{//enemy won
				_wonCard = _ttMan.ttDb.RetrievePlayerTripleTriadCardAtLocation(additionalArgs);
				_ttMan.ttUi.InitializeRewardSelectedUiLose(_wonCard.whatCardIAm.cardName);
				_wonCard.cardAnimator.SetTrigger("rewardSelected");
				
			}

		}

		public override void Execute()
		{
			ListenForUserInput();
		}
		

		#region Functions

		private void ListenForUserInput()
		{
			if (_ttMan.ttUi.isLoading) return;
			if ((Input.GetKeyDown(KeyCode.Space)) || Input.GetButtonDown("Fire1"))
			{
				SoundManager.instance.PlaySFX(_whoWonTheGame == 1 ? 17 : 6);
				_wonCard.cardAnimator.SetTrigger("rewardConfirmed");
				_ttMan.ttUi.isLoading = true;
				switch (_whoWonTheGame)
				{
					case 1:
						CardInventory.instance.AddCardToInventory(_wonCard.whatCardIAm.cardID);
						CheckToSeeIfOpponentShouldLoseTheCard();
						break;
					case 3:
						CardInventory.instance.RemoveCardFromInventory(_wonCard.whatCardIAm.cardID);
						CheckToSeeIfOpponentShouldGainTheCard();
						break;
				}
			}

		}

		private void CheckToSeeIfOpponentShouldLoseTheCard()
		{
			if (_ttDb.CheckToSeeIfCardIsInRareList(_wonCard.whatCardIAm.cardID))
			{
				_ttMan.ttUi.RemoveCardFromCurrentOpponent(_wonCard.whatCardIAm.cardID);
			}
		}
		private void CheckToSeeIfOpponentShouldGainTheCard()
		{
			if (_wonCard.whatCardIAm.cardsRarity == Card.cardRarity.Rare ||
			    _wonCard.whatCardIAm.cardsRarity == Card.cardRarity.Elite)
			{
				_ttMan.ttUi.AddCardToCurrentOpponent(_wonCard.whatCardIAm.cardID);
			}
		}

		#endregion
	}
}
