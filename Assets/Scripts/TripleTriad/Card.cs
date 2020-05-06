using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
public class Card : ScriptableObject, IComparable<Card>
{
    public int cardID = 0;
    public string description = null;

    public enum cardRarity
    {
        Common,
        Uncommon,
        Rare,
        Elite,
        Super
    }

    public cardRarity cardsRarity;
    public string cardName;


    public Sprite playerImage;
    public Sprite enemyImage;
    public int amountOwned = 0;
    public bool hasOwnedBefore = false;
    // [Range(1,10)]
    // public int cardLevel;
    // public bool myCard;
    // public bool enemyCard;
    // public int cardSlot;
    public int percentChanceForEnemyToUseInBattle;

    //new test stuff
    public int[] cardValues = new int[4];
    
    public int CompareTo(Card compareId)
    {       // A null value means that this object is greater.
        if (compareId == null){
            return 1;  
        }
        else {
            return cardID.CompareTo(compareId.cardID);
        }
    }
    
}

