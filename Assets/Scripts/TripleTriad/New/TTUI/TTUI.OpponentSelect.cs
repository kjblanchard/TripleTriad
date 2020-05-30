using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration

		[Header("Opponent Selection Stuff")]
		[SerializeField] private Canvas _opponentSelectionCanvas;

		public OpponentBox[] _opponentBoxes;
		public Image _opponentSelectCursorImage;
		
		private static readonly int kRest = Animator.StringToHash("rest");
		private static readonly int kIdle = Animator.StringToHash("idle");
		private static readonly int kSelected = Animator.StringToHash("selected");


		[SerializeField] private Text _opponentNameText;
		[SerializeField] private Text _opponentDescriptionText;
		[SerializeField] private Text _isRandomText;
		[SerializeField] private Text _isOpenText;
		[SerializeField] private Text _rareCardText;
		[SerializeField] private Text _aiDifficultyText;
		private static readonly int kCheatEntered = Animator.StringToHash("cheatEntered");
		private static readonly int kShowingRulesFadeOut = Animator.StringToHash("showingRulesFadeOut");

		#endregion


		#region Functions

		public void InitializeOpponentSelectionUi()
		{
			_opponentSelectionCanvas.enabled = true;
		}
		
		public void KeepCursorOnProperSelectionOpponentSelect()
		{
			if (!isLoading)
			{
				if (!_opponentSelectCursorImage.gameObject.activeInHierarchy)
				{
					_opponentSelectCursorImage.gameObject.SetActive(true);
				}

				_opponentSelectCursorImage.transform.position = _opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()]
					.cursorTransform.transform.position;
			}
			else
			{
				_opponentSelectCursorImage.gameObject.SetActive(false);
			}
		}
		public void ReturnCursorToDefaultOpponentSelect()
		{
			_opponentSelectCursorImage.transform.position = _opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()]
					.cursorTransform.transform.position;
			
	}

		public void TurnOnProperAnimatorOpponentSelection()
		{
			for (int i = 0; i < _opponentBoxes.Length; i++)
			{
				
				if (_opponentBoxes[i].canSelectOpponent)
				{
					_opponentBoxes[i].opponentAnimator.SetBool(kIdle,false);
				}

			}

			var spotToLook = ttdb.RetrieveOpponentSelectionCurrentValue();
			if (!_opponentBoxes[spotToLook].canSelectOpponent) return;
			_opponentBoxes[spotToLook].opponentAnimator.SetBool(kIdle, true);

		}

		public void CheatCodeEntered()
		{
			_opponentBoxes[6].opponentAnimator.SetTrigger(kCheatEntered);
		}

		public void SelectedAnimatorOpponentSelection()
		{
			if (_opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()].canSelectOpponent)
				_opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()].opponentAnimator.SetTrigger(kSelected);
		}

		public void StartShowingRulesFadeOut()
		{
			_tripleTriadUiFade.animator.SetTrigger(kShowingRulesFadeOut);
		}

		public void UpdateOpponentInfoOpponentSelection()
		{

			if (_opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()].canSelectOpponent)
			{
				var enemyInfo = _opponentBoxes[ttdb.RetrieveOpponentSelectionCurrentValue()].opponentCardInfo;
				_opponentNameText.text = enemyInfo.RetrieveName();
				_opponentDescriptionText.text = enemyInfo.RetrieveDescription();
				_isOpenText.text = enemyInfo.RetrieveOpen();
				_aiDifficultyText.text = enemyInfo.RetrieveDifficulty();
				_rareCardText.text = enemyInfo.RetrieveRares();
				_isRandomText.text = enemyInfo.RetrieveRandom();
			}
			else
			{
				var blank = "";
				_opponentNameText.text = blank;
				_opponentDescriptionText.text = blank;
				_isOpenText.text = blank;
				_aiDifficultyText.text = blank;
				_rareCardText.text = blank;
				_isRandomText.text = blank;
			}


		}

		public void TurnOffOpponentSelectUi()
		{
			_opponentSelectionCanvas.enabled = false;
			ttdb.InitializeOpponentSelectionDb();
			ReturnCursorToDefaultOpponentSelect();
		}

		public void RemoveCardFromCurrentOpponent(int cardId)
		{
			_opponentBoxes[ttdb.RetrieveCurrentOpponentInOpponentList()].opponentCardInfo.RemoveRareFromList(cardId);
			
		}


		
		public void AddCardToCurrentOpponent(int cardId)
		{
			_opponentBoxes[ttdb.RetrieveCurrentOpponentInOpponentList()].opponentCardInfo.AddRareToList(cardId);
			
		}

		#endregion
	}
}
