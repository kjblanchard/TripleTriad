using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration

		[Header("Location Selection")]
		
		[SerializeField] private Transform[] locationSelectionFingerLocation;

		[SerializeField] private GameObject locationSelectionFinger;
		[SerializeField] private Canvas _infoPanelCanvas;
		[SerializeField] private Text _infoPanelCardText;

		#endregion

		#region Functions

		public void KeepFingerOnProperLocationInLocationSelection()
		{
			if (isLoading == false)
			{
				if (!locationSelectionFinger.gameObject.activeInHierarchy && ttMan.RetrieveCurrentState() == ttMan.locationSelectionState)
				{
                    
					locationSelectionFinger.gameObject.SetActive(true);
				}

				locationSelectionFinger.transform.position =
					locationSelectionFingerLocation[ttdb.RetrieveLocationSelectionCurrentSelection()].transform.position;
			}
			else
			{
				locationSelectionFinger.gameObject.SetActive(false);
			}
		}

		public void UpdateInfoPanelCardName()
		{
			_infoPanelCardText.text =
				ttdb.RetrieveTripleTriadCardInBoardSelection(ttdb.RetrieveLocationSelectionCurrentSelection()).whatCardIAm.cardName;
		}

		public void PlayCardInLocationSelection()
		{
			_infoPanelCanvas.enabled = false;
			ttdb.RetrieveTripleTriadCardInPlayerSelection().CardPlayed();
		}

		public void PlayCardToBoardInLocationSelection(int boardLocation, Card cardToBecome)
		{
			ttdb.RetrieveTripleTriadCardInBoardSelection(boardLocation).SetMyCurrentCard(cardToBecome);
			if (ttMan.RetrieveCurrentState() == ttMan.locationSelectionState)
			{
				ttdb.RetrieveTripleTriadCardInBoardSelection(boardLocation).ChangeImageToPlayer();
				ttdb.RetrieveTripleTriadCardInBoardSelection(boardLocation).cardOwnedByPlayer = true;

			}
			else
			{
				ttdb.RetrieveTripleTriadCardInBoardSelection(boardLocation).ChangeImageToEnemy();
			}
			ttdb.RetrieveTripleTriadCardInBoardSelection(boardLocation).MoveToBoard();
		}

		public void RealignCardsInHandAfterCardIsPlayed()
		{
			//play the animation to move him up by one
			
		}

		public void InitializeLocationSelectionUiElements()
		{
			playerSelectionFinger.SetActive(false);

			ControlInfoPanelInLocationSelection();
		}

		public void ControlInfoPanelInLocationSelection()
		{
			if (ttdb.CheckIfCardIsInTheBoardLocation())
			{
				_infoPanelCanvas.enabled = true;
				UpdateInfoPanelCardName();
			}
			else
			{
				_infoPanelCanvas.enabled = false;
			}
		}

		public int RetrieveLocationSelectionTransformCount()
		{
			return locationSelectionFingerLocation.Length;
		}

		#endregion
	}
}
