using UnityEngine;

namespace ETF.TripleTriad
{

	public partial class TTDB
	{

		#region Configuration
		[Header("Showing Rules Stuff")]
		[Range(0, 1)] private int _currentFingerPositionInShowingRules;

		#endregion


		#region Functions

		public int ReturnFingerPositionInShowingRules()
		{
			return _currentFingerPositionInShowingRules;
		}

		public void InitializeShowingRulesDbValues()
		{
			CardInventory.instance
				.CreateUsableBattleCardLists();
			_currentFingerPositionInShowingRules = 0;
		}

		public void ScrollRightInShowingRulesDb()
		{
			_currentFingerPositionInShowingRules++;
		}
		
		public void ScrollLeftInShowingRulesDb()
		{
			_currentFingerPositionInShowingRules--;
		}

		public void CancelButtonInShowingRules()
		{
			_currentFingerPositionInShowingRules = 1;
		}

		#endregion
	}
}
