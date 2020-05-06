using UnityEngine;

namespace ETF.TripleTriad
{

	public class TripleTriadBoardLogic : MonoBehaviour
	{
	
		#region Configuration
		
		[SerializeField] private TripleTriadCardOnBoard[] _tripleTriadBoardCards;
		private int _adjacentCardLocationToCheckCached;
		private int _myCardValueCached;
		private int _enemyCardValueLocationToCheck;
		private int _enemyCardValueCached;
		
	
		#endregion
		
		#region Functions

		// public void CheckToSeeIfCardsAreFlipping(int locationCardPlaced)
		// {
		// 	for (int i = 0; i < _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency.Length; i++)
		// 	{
		// 		_adjacentCardLocationToCheckCached = _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency[i];
		// 		//print($"checking adjacency [i], which is location {_adjacentCardLocationToCheckCached} and is he in play?  {_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay} ");
		// 		if (_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay)
		// 		{
		// 			_myCardValueCached = _tripleTriadBoardCards[locationCardPlaced].whatCardIAm
		// 				.cardValues[_tripleTriadBoardCards[locationCardPlaced].myValueToCheck[i]];
		// 			_enemyCardValueCached = _tripleTriadBoardCards[_adjacentCardLocationToCheckCached].whatCardIAm
		// 				.cardValues[_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i]];
		// 			//print($"my value is {_myCardValueCached} and his value is {_enemyCardValueCached} and he is at location {_adjacentCardLocationToCheckCached}");
		// 			if (_myCardValueCached > _enemyCardValueCached)
		// 			{
		// 				switch (_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i])
		// 				{
		// 					case 0:
		// 						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipUp();
		// 						break;
		// 					case 1:
		// 						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipRight();
		// 						break;
		// 					case 2:
		// 						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipDown();
		// 						break;
		// 					case 3:
		// 						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipLeft();
		// 						break;
		// 				}
		// 			}
		// 		}
		// 	}
		// }
		
		
		public void CheckToSeeIfCardsAreFlippingForPlayer(int locationCardPlaced)
		{
			for (int i = 0; i < _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency.Length; i++)
			{
				_adjacentCardLocationToCheckCached = _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency[i];
				//print($"checking adjacency [i], which is location {_adjacentCardLocationToCheckCached} and is he in play?  {_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay} ");
				if (_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay && !_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardOwnedByPlayer)
				{
					_myCardValueCached = _tripleTriadBoardCards[locationCardPlaced].whatCardIAm
						.cardValues[_tripleTriadBoardCards[locationCardPlaced].myValueToCheck[i]];
					_enemyCardValueCached = _tripleTriadBoardCards[_adjacentCardLocationToCheckCached].whatCardIAm
						.cardValues[_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i]];
					//print($"my value is {_myCardValueCached} and his value is {_enemyCardValueCached} and he is at location {_adjacentCardLocationToCheckCached}");
					if (_myCardValueCached > _enemyCardValueCached)
					{
						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardOwnedByPlayer = true;

						switch (_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i])
						{
							case 0:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipUp();
								break;
							case 1:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipRight();
								break;
							case 2:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipDown();
								break;
							case 3:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipLeft();
								break;
						}
					}
				}
			}
		}
		
		public void CheckToSeeIfCardsAreFlippingForEnemy(int locationCardPlaced)
		{
			for (int i = 0; i < _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency.Length; i++)
			{
				_adjacentCardLocationToCheckCached = _tripleTriadBoardCards[locationCardPlaced].locationBoardAdjacency[i];
				//print($"checking adjacency [i], which is location {_adjacentCardLocationToCheckCached} and is he in play?  {_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay} ");
				if (_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardInPlay && _tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardOwnedByPlayer)
				{
					_myCardValueCached = _tripleTriadBoardCards[locationCardPlaced].whatCardIAm
						.cardValues[_tripleTriadBoardCards[locationCardPlaced].myValueToCheck[i]];
					_enemyCardValueCached = _tripleTriadBoardCards[_adjacentCardLocationToCheckCached].whatCardIAm
						.cardValues[_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i]];
					//print($"my value is {_myCardValueCached} and his value is {_enemyCardValueCached} and he is at location {_adjacentCardLocationToCheckCached}");
					if (_myCardValueCached > _enemyCardValueCached)
					{
						_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].cardOwnedByPlayer = false;

						switch (_tripleTriadBoardCards[locationCardPlaced].adjacencyValueToCheck[i])
						{
							case 0:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipUp();
								break;
							case 1:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipRight();
								break;
							case 2:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipDown();
								break;
							case 3:
								_tripleTriadBoardCards[_adjacentCardLocationToCheckCached].BoardCardFlipLeft();
								break;
						}
					}
				}
			}
		}


		#endregion
	}
}
