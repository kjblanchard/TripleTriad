using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration

		[Header("Showing Rules Stuff")] 
		// [SerializeField]
		// private Canvas _tripleTriadFullCanvas;
		[SerializeField]
		private Canvas _showingRulesCanvas;
		[SerializeField] private Transform[] _showingRulesFingerLocations;
		public Animator showingRulesAnimator;
		[Header("EnemyHandReferences")] private EnemyCardHand.WhatTypeOfCardPlayerAmI _typeOfEnemyPlayer;
		private List<Card> _currentEnemyRareCards;
		
		[SerializeField] private Text _typeOfWinRewardText;
		[SerializeField] private Text _isEnemyHandOpenText;
		[SerializeField] private Text _isRandomCardSelectionText;
		[SerializeField] private Text _isPlusText;
		[SerializeField] private Text _isSameText;
		private static readonly int kOpening = Animator.StringToHash("opening");

		#endregion



		#region Functions

		
		
		public void InitializeShowingRulesUi()
		{
			_tripleTriadBoardImage.enabled = true;
			_showingRulesCanvas.enabled = true;
			showingRulesAnimator.SetTrigger(kOpening);
			_tripleTriadBoardImage.color = Color.white;
			UpdateRulesText();
		}

		public void UpdateRulesText()
		{
			_typeOfWinRewardText.text = ttdb.RetrieveRuleWinReward();
			_isRandomCardSelectionText.text = ttdb.RetrieveRuleRandom();
			_isEnemyHandOpenText.text = ttdb.RetrieveRuleEnemyHandOpen();
			_isPlusText.text = ttdb.RetrieveRulePlus();
			_isSameText.text = ttdb.RetrieveRuleSame();
		}


		public void KeepFingerOnProperLocationInShowingRules()
		{
		
			//finger is turned on and off when in update method by the isloading bool
			if (isLoading == false)
			{
				if (!fingerPrefab.gameObject.activeInHierarchy)
				{
					fingerPrefab.gameObject.SetActive(true);
				}

				fingerPrefab.transform.position =
					_showingRulesFingerLocations[ttdb.ReturnFingerPositionInShowingRules()].transform.position;
			}
			else
			{
				fingerPrefab.gameObject.SetActive(false);
			}	
		}

		public int ReturnShowingRulesFingerPositionCount()
		{
			return _showingRulesFingerLocations.Length;
		}

		public void TurnOffShowingRulesCanvas()
		{
			_showingRulesCanvas.enabled = false;
		}
		

		#endregion
	}
}
