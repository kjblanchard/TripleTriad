

namespace ETF.TripleTriad
{

	public partial class TTLogic
	{

		#region Configuration


		#endregion



		#region Functions

		public bool CanIScrollRightOnRuleSelection()
		{
			return ttDb.ReturnFingerPositionInShowingRules() < ttUi.ReturnShowingRulesFingerPositionCount() - 1 && !ttUi.isLoading;
		}
		
		public bool CanIScrollLeftOnRuleSelection()
		{
			return ttDb.ReturnFingerPositionInShowingRules() > 0 && !ttUi.isLoading ;
		}
		
		public bool CanIPressCancelOnRuleSelection()
		{
			return ttDb.ReturnFingerPositionInShowingRules() == 0 && !ttUi.isLoading ;
		}

		#endregion
	}
}
