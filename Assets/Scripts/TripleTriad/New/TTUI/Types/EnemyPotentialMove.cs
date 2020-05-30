
using System;

namespace ETF.TripleTriad
{

	public class EnemyPotentialMove : IComparable<EnemyPotentialMove>
	{

		#region Configuration

		private readonly int _boardLocation;
		private readonly int _cardLocation;
		private float _moderateCardsDefenseValue = 0;
		private Card _enemyCardReference;
		private int _amountOfDefSidesCalculated = 0;
		
		public int CompareTo(EnemyPotentialMove compareDefense)
		{       // A null value means that this object is greater.
			if (compareDefense == null){
				return 1;  
			}
			else {
				return compareDefense._moderateCardsDefenseValue.CompareTo(_moderateCardsDefenseValue);
			}
		}
		

		#endregion

		public EnemyPotentialMove(int boardLocation, int cardLocation, Card enemyCardReference = null)
		{
			_boardLocation = boardLocation;
			_cardLocation = cardLocation;
			_enemyCardReference = enemyCardReference;

		}

		#region Functions

		public int RetrieveBoardLocation()
		{
			return _boardLocation;
		}

		public int RetrieveCardLocation()
		{
			return _cardLocation;
		}
		
		public Card RetrieveCard()
		{
			return _enemyCardReference;
		}

		public void CalculateDefenseValue()
		{
			_moderateCardsDefenseValue =  (_moderateCardsDefenseValue/_amountOfDefSidesCalculated);
		}

		public void AddToDefenseValue(int amountToAdd)
		{
			_amountOfDefSidesCalculated++;
			_moderateCardsDefenseValue = _moderateCardsDefenseValue + amountToAdd;
		}

		public float RetrieveDefense()
		{
			return _moderateCardsDefenseValue;
		}

		#endregion
	}
}
