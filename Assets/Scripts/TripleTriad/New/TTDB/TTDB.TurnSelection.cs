using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public partial class TTDB
    {
        private int _whichPlayerGetsToGoFirst;


        public void SetWhoGetsToGoFirst(int whoGoesFirst)
        {
            _whichPlayerGetsToGoFirst = whoGoesFirst;
        }

        public int ReturnWhichPlayerGetsToGoFirst()
        {
            return _whichPlayerGetsToGoFirst;
        }

    }
}
