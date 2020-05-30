
using UnityEngine;

namespace ETF.TripleTriad
{

	public class EndGameState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager _ttMan;
		private int _whoWonTheGame;
		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttMan.ttUi.InitializeEndGameInitialUi();

			if (_ttMan.ttDb.NumberOfPlayerOwnedCards() > _ttMan.ttDb.NumberOfEnemyOwnedCards())
			{
				_whoWonTheGame = 1;
				_ttMan.ttUi.InitializeWinEndGameUi();
			}
			else if (_ttMan.ttDb.NumberOfPlayerOwnedCards() == _ttMan.ttDb.NumberOfEnemyOwnedCards())
			{
				_ttMan.ttUi.InitializeTieEndGameUi();
				_whoWonTheGame = 2;
			}
			else
			{
				_ttMan.ttUi.InitializeLoseEndGameUi();
				_whoWonTheGame = 3;
			}

		}

		public override void Execute()
		{
			ListenForUserInput();
		}

		public override void End()
		{
			
		}

		#region Functions

		private void ListenForUserInput()
		{
			if ((!Input.GetKeyDown(KeyCode.Space) && !Input.GetButtonDown("Fire1")) || _ttMan.ttUi.isLoading) return;
			switch (_whoWonTheGame)  
			{
				//playerwin
				case 1:
					_ttMan.ttUi.RewardSelectionFadeOut();
					_ttMan.SendStateChange(_ttMan.rewardSelectionState,1);
					
					break;
				//tie
				case 2:
					_ttMan.ttUi.isLoading = true;
					_ttMan.SendStateChange(_ttMan.gameEndedState);
					break;
				//playerlose
				case 3:
					_ttMan.ttUi.RewardSelectionFadeOut();
					_ttMan.SendStateChange(_ttMan.rewardSelectionState,3);
					break;
				
			}

		}
		
		

		#endregion
	}
}
