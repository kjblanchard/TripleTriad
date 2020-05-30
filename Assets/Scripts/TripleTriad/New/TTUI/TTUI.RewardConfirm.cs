using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration
		[Header("Reward Confirm Stuff")]
		[SerializeField] private Transform[] _rewardScreenConfirmFingerPoints;
		[SerializeField] private Canvas _rewardConfirmCanvas;

		#endregion
	

		#region Functions

		public void InitializeRewardConfirmUi()
		{
			_rewardConfirmCanvas.enabled = true;
		}
		public void KeepFingerOnProperLocationInRewardConfirm()
		{
			if (isLoading == false)
			{
				if (!fingerPrefab.gameObject.activeInHierarchy)
				{
                    
					fingerPrefab.gameObject.SetActive(true);
				}

				fingerPrefab.transform.position =
					_rewardScreenConfirmFingerPoints[ttdb.RetrieveCurrentSelectionInRewardConfirm()].transform
						.position;
			}
			else
			{
				fingerPrefab.gameObject.SetActive(false);
			}
		}

		public void GoingBackToRewardSelectionUi()
		{
			_rewardConfirmCanvas.enabled = false;
		}

		#endregion
	}
}
