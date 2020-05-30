using UnityEngine;
using Random = UnityEngine.Random;

namespace ETF.TripleTriad
{
    public partial class TTLogic
    {
        [SerializeField] bool myTurnFirst;
        [SerializeField] bool enemyTurnFirst;

        private int _tempTurnValue;

        public int TurnSelector()
        {
            if (myTurnFirst)
            {
                return 0;
            }
            else if (enemyTurnFirst)
            {
                return 1;
            }
            else
            {
                _tempTurnValue = Random.Range(0, 2);
                return _tempTurnValue;
            }
        }


    }

}