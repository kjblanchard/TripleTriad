using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration
		[Header("Reward Confirm Stuff")]
		private int _currentSelectionInRewardConfirm = 0 ;

		#endregion



		#region Functions

		public int RetrieveCurrentSelectionInRewardConfirm()
		{
			return _currentSelectionInRewardConfirm;
		}

		public void MoveCursorRightInRewardConfirm()
		{
			_currentSelectionInRewardConfirm++;
		}
		
		public void MoveCursorLeftInRewardConfirm()
		{
			_currentSelectionInRewardConfirm--;
		}

		public void SetCursorToCancelRewardConfirmation()
		{
			_currentSelectionInRewardConfirm = 1;
		}

		public void InitializeRewardConfirmDb()
		{
			_currentSelectionInRewardConfirm = 0;
		}

		#endregion
	}
}
