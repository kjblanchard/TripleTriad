using System;
using UnityEngine;

namespace ETF.TripleTriad
{
    public class TripleTriadManager : MonoBehaviour
    {
        #region Configuration

        [Header("Script References")] public TTUI ttUi;
        public TTDB ttDb;
        public TTLogic ttLogic;
        [SerializeField] TtStateMachine ttStateMachine;

        [Header("States")] 
        public DisabledState disabledState;

        public TitleScreenState titleScreenState;
        public OpponentSelectState opponentSelectState;
        public GameStartedState gameStartedState;
        public ShowingRulesState showingRulesState;
        public CardSelectionState cardSelectionState;
        public CardConfirmationState cardConfirmationState;
        public EnemyHandSelectionState enemyHandSelectionState;
        public TurnSelectionState turnSelectionState;
        public PlayerTurnState playerTurnState;
        public LocationSelectionState locationSelectionState;
        public EnemyTurnState enemyTurnState;
        public BetweenTurnState betweenTurnState;
        public EndGameState endGameState;
        public RewardSelectionState rewardSelectionState;
        public RewardConfirmState rewardConfirmState;
        public RewardSelectedState rewardSelectedState;
        public GameEndedState gameEndedState;

        [Header("Singleton")] 
        
        public static TripleTriadManager instance;

        #endregion


        private void Awake()
        {
            instance = this;
            
        }

        private void Start()
        {

            SendStateChange(gameStartedState);
            //gameObject.SetActive(false);
        }

        void Update()
        {
            ttStateMachine.RunCurrentState();
        }

        #region Functions

        public void SendStateChange(TtState stateToChangeTo,int additionalArgs=0)
        {
            GC.Collect();
            ttStateMachine.ChangeState(stateToChangeTo,additionalArgs);
        }

        public void SendChangeToPreviousState()
        {
            GC.Collect();
            ttStateMachine.ChangeStateToPreviousState();
        }

        public TtState RetrieveCurrentState()
        {
            return ttStateMachine.ReturnCurrentState();
        }

        public TtState RetrievePreviousState()
        {
            return ttStateMachine.ReturnPreviousState();
        }

        // public void SwitchToEnemyTurn()
        // {
        //     fingerAnimationChanger.ChangeTurn();
        // }

        #endregion



    }
}
