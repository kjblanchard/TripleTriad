using UnityEngine;

namespace ETF.TripleTriad
{

	public class OpponentSelectState : TtState
	{

		#region Configuration

		private TTDB.MovementDirections _whichWayToMove;
		private string[] _cheatCode;
		private int _index;

		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			_ttUi._tripleTriadBoardImage.enabled = false;
			Initialize();
			_cheatCode = new[] { "k", "b", "c", "a", "r", "d", "s" };
			_index = 0; 
		}
		
		
		public override void Execute()
		{
			ListenForUserInput();
			_ttUi.KeepCursorOnProperSelectionOpponentSelect();
		}

		public override void End()
		{
			_ttUi.TurnOffOpponentSelectUi();
		}


		#region Functions

		private void ListenForUserInput()
		{
			if (_ttUi.isLoading) return;
			if (Input.GetKeyDown(KeyCode.D) || (Input.GetButtonDown("right")))
			{
				_whichWayToMove = TTDB.MovementDirections.Right;
				if (_ttLogic.CanIScrollInOpponentSelect(_whichWayToMove))
				{
					_ttDb.MoveOpponentSelectionInDb(_whichWayToMove);
					TurnOnBoxImageAndAnimator();
				}
			}
			else if (Input.GetKeyDown(KeyCode.A) || (Input.GetButtonDown("left")))
			{
				_whichWayToMove = TTDB.MovementDirections.Left;
				if (_ttLogic.CanIScrollInOpponentSelect(_whichWayToMove))
				{
					_ttDb.MoveOpponentSelectionInDb(_whichWayToMove);
					TurnOnBoxImageAndAnimator();
				}
			}
			else if (Input.GetKeyDown(KeyCode.W) || (Input.GetButtonDown("up")))
			{
				_whichWayToMove = TTDB.MovementDirections.Up;
				if (_ttLogic.CanIScrollInOpponentSelect(_whichWayToMove))
				{
					_ttDb.MoveOpponentSelectionInDb(_whichWayToMove);
					TurnOnBoxImageAndAnimator();
				}
			}
			else if (Input.GetKeyDown(KeyCode.S) || (Input.GetButtonDown("down")))
			{
				_whichWayToMove = TTDB.MovementDirections.Down;
				if (_ttLogic.CanIScrollInOpponentSelect(_whichWayToMove))
				{
					_ttDb.MoveOpponentSelectionInDb(_whichWayToMove);
					TurnOnBoxImageAndAnimator();
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space) || (Input.GetButtonDown("Fire1")))
			{
				if (_ttDb.RetrieveOpponentSelectionCurrentValue() == 7)
				{
					Application.Quit();
					return;
				}
				_ttDb.SetCurrentOpponentYouArePlaying();
				TurnOnSelectedAnimator();
				_ttDb.BringInCurrentSelectedEnemyInformationToDb();

			}
			if (Input.anyKeyDown)
			{
				if (Input.GetKeyDown(_cheatCode[_index]))
				{
					_index++;
				}
				else
				{
					_index = 0;
				}
			}
			if (_index == _cheatCode.Length)
			{
				SoundManager.instance.PlaySFX(21);
				_index = 0;
				_ttMan.ttUi.CheatCodeEntered();
				CardInventory.instance.CheatCodeEntered();
			}
		}
		
		private void Initialize()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				SoundManager.instance.PlayBgm(1);
			}
			else
			{
				SoundManager.instance.PlayIntroLoop(10);
				SoundManager.instance.CacheNextMusic(1);
			}
			_ttDb.InitializeOpponentSelectionDb();
			_ttUi.InitializeOpponentSelectionUi();
		}
		
		private void TurnOnProperAnimator()
		{
			_ttUi.TurnOnProperAnimatorOpponentSelection();
		}

		private void UpdateOpponent()
		{
			_ttUi.UpdateOpponentInfoOpponentSelection();
			SoundManager.instance.PlaySFX(18);
		}

		private void TurnOnBoxImageAndAnimator()
		{
			TurnOnProperAnimator();
			UpdateOpponent();
		}

		private void TurnOnSelectedAnimator()
		{
			_ttUi.SelectedAnimatorOpponentSelection();
		}

		#endregion
	}
}
