﻿using System.Collections;
using UnityEngine;

namespace ETF.TripleTriad
{

	public class EnemyTurnState : TtState
	{

		#region Configuration

		[SerializeField] private TripleTriadManager ttMan;
		[Range(0f,1f)]
		[SerializeField] private float enemyThinkingTimer = 0.4f;

		[SerializeField] private WaitForSeconds inFocusWait;
		#endregion

		public override void Startup(int additionalArgs = 0)
		{
			ttMan.ttDb.ClearAllEnemyCardPlacementLists();
			inFocusWait = new WaitForSeconds(enemyThinkingTimer);
			StartCoroutine(CycleThroughCardsAnimation());
		}

		#region Functions

		private IEnumerator CycleThroughCardsAnimation()
		{
			for (int i = 0; i < ttMan.ttDb.currentEnemyTripleTriadCardsInHand.Count; i++)
			{
				ttMan.ttDb.currentEnemyTripleTriadCardsInHand[i].InFocus();
				yield return inFocusWait;
				if (i != ttMan.ttDb.currentEnemyTripleTriadCardsInHand.Count - 1)
				{
					ttMan.ttDb.currentEnemyTripleTriadCardsInHand[i].OutFocus();
				}
			}
			ttMan.ttLogic.FullEnemyTurnHandPlacementChoices();
			ttMan.ttLogic.ChooseCardToPlayFromListOfChoices();
			var boardLocation = ttMan.ttLogic.RetrieveBoardPositionToPlaceCard();
			var handPosition = ttMan.ttLogic.RetrieveHandPositionToPlaceCard();
			ttMan.ttDb.currentEnemyTripleTriadCardsInHand[handPosition].SetLocationToGoTo(boardLocation);
			ttMan.ttUi.setFocusOnEnemyCardSelected();
			ttMan.ttUi.PlayCardInEnemyTurn(handPosition);
			ttMan.ttDb.ChangeBoardLocationToInPlayEnemy(boardLocation);
		}


		#endregion
	}
}
