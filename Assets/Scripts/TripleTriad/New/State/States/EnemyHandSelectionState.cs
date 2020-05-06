using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public class EnemyHandSelectionState : TtState
    {
        #region Configuration

        [SerializeField] TripleTriadManager ttMan;

        #endregion

        public override void Startup(int additionalArgs = 0)
        {
            //Random.seed = System.DateTime.Now.Millisecond;
            StartCoroutine(ttMan.ttUi.InitializeEnemyHandSelectScreenUiCo());
            ttMan.ttDb.GenerateEnemyCardHand();
        }
        
    }
}
