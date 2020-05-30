using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{

	public partial class TTUI
	{

		#region Configuration

		[Header("Between Turn Stuff")] [SerializeField]
		private Canvas _scoreDisplayCanvas;
		[SerializeField] private Text _playerScoreText;
		[SerializeField] private Text _enemyScoreText;


		#endregion
	

		#region Functions

		public void UpdateScoreCount()
		{
			_playerScoreText.text = ttdb.NumberOfPlayerOwnedCards().ToString();
			_enemyScoreText.text = ttdb.NumberOfEnemyOwnedCards().ToString();
		}

		public void InitializeBetweenTurnStateUi()
		{
			_infoPanelCanvas.enabled = false;
		}

		
		#endregion
	}
}
