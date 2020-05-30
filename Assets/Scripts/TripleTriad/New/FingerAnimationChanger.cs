
using UnityEngine;

namespace ETF.TripleTriad
{
    public class FingerAnimationChanger : MonoBehaviour
    {
        [SerializeField] private TripleTriadManager _ttMan;
        [SerializeField] Animator fingerAnimator;
        //private static readonly int kWhichWay = Animator.StringToHash("WhichWay");
        private static readonly int kGoToPlayerSide = Animator.StringToHash("GoToPlayerSide");
        private static readonly int kGoToEnemySide = Animator.StringToHash("GoToEnemySide");
        private static readonly int kPlayerToEnemy = Animator.StringToHash("PlayerToEnemy");
        private static readonly int kEnemyToPlayer = Animator.StringToHash("EnemyToPlayer");


        private void ChangeToPlayerTurn()
        {
            _ttMan.ttUi.InitializeBetweenTurnUiEnemyToPlayer();
            _ttMan.SendStateChange(_ttMan.playerTurnState);
        }
        private void ChangeToEnemyTurn()
        {

            _ttMan.SendStateChange(_ttMan.enemyTurnState);
        }
        

        public void ChooseWhichAnimationToPlay()
        {
            if (TripleTriadManager.instance.ttDb.ReturnWhichPlayerGetsToGoFirst() == 0)
            {
                fingerAnimator.SetTrigger(kGoToPlayerSide);

            }
            else
            {
                fingerAnimator.SetTrigger(kGoToEnemySide);

            }
        }

        public void PlayChangeToEnemyTurnAnimation()
        {//these are used to call from ttman to start the animation/turn change
            fingerAnimator.SetTrigger(kPlayerToEnemy);
        }
        
        public void PlayChangeToPlayerTurnAnimation()
        {
            fingerAnimator.SetTrigger(kEnemyToPlayer);
        }

        private void ChangeFromPlayerToEnemyState()
        {
            _ttMan.ttUi.InitializeBetweenTurnUiPlayerToEnemy();
        }
        
        private void ChangeFromEnemyToPlayerState()
        {
            _ttMan.ttUi.InitializeBetweenTurnUiEnemyToPlayer();
        }

    }

}