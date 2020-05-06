using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace ETF.TripleTriad
{
    public class TtStateMachine : MonoBehaviour
    {

        [SerializeField] TtState _currentState;
        [SerializeField] TtState _previousState;


        public void RunCurrentState()
        {

            _currentState.Execute();

        }

        public void ChangeState(TtState stateToChangeTo, int additionalArguments = 0)
        {
            if (_currentState != null)
            {
                _currentState.End();
                _previousState = _currentState;
            }

            _currentState = stateToChangeTo;
            _currentState.Startup(additionalArguments);
        }

        public void ChangeStateToPreviousState()
        {
            _currentState.End();
            _currentState = _previousState;
            _currentState.Startup();
        }

        public TtState ReturnCurrentState()
        {
            return _currentState;
        }
        
        public TtState ReturnPreviousState()
        {
            return _previousState;
        }
    }
}
