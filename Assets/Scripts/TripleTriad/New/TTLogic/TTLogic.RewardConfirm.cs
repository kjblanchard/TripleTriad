using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion
	

		#region Functions

		public bool CanIScrollRightInRewardConfirm()
		{
			return !ttUi.isLoading && ttDb.RetrieveCurrentSelectionInRewardConfirm() < 1;
		}

		public bool CanIScrollLeftInRewardConfirm()
		{
			return !ttUi.isLoading && ttDb.RetrieveCurrentSelectionInRewardConfirm() > 0;
		}

		public bool CanISelectInRewardConfirm()
		{
			return !ttUi.isLoading;
		}

		#endregion
	}
}
