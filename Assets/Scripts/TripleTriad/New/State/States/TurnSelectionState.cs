
using UnityEngine;

namespace ETF.TripleTriad
{
    public class TurnSelectionState : TtState
    {
        #region Configuration

        [SerializeField] TripleTriadManager ttMan;

        #endregion

        public override void Startup(int additionalArgs = 0)
        {
            ttMan.ttUi.InitializeTurnSelection();
            ttMan.ttDb.SetWhoGetsToGoFirst(ttMan.ttLogic.TurnSelector());
            ttMan.ttUi.switchToFingerTurnSelectionAnimation();
        }

        public override void Execute()
        {
        }

        public override void End()
        {
            ttMan.ttUi.LeavingTurnSelectionUiChange();
        }


    }
}
