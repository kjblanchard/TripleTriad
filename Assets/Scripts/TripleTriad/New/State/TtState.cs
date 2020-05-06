using UnityEngine;

namespace ETF.TripleTriad
{
    public abstract class TtState : MonoBehaviour
    {
        protected static TTUI _ttUi;
        protected static TripleTriadManager _ttMan;
        protected static TTDB _ttDb;
        protected static TTLogic _ttLogic;
        protected static bool hasInitiated;

        private void Awake()
        {
            if (!hasInitiated)
            {
                hasInitiated = true;
                _ttUi = FindObjectOfType<TTUI>();
                _ttMan = FindObjectOfType<TripleTriadManager>();
                _ttDb = FindObjectOfType<TTDB>();
                _ttLogic = FindObjectOfType<TTLogic>();
            }
            
        }

        public virtual void Startup(int additionalArgs = 0)
        {

        }

        public virtual void Execute()
        {

        }

        public virtual void End()
        {

        }
        

    }
}
