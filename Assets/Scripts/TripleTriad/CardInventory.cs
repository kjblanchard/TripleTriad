using System.Collections.Generic;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    [Header("Card List and Empty Card Reference")]
    public Card[] masterCardList;
    public List<Card> renumberedCardList = new List<Card>();
    public List<Card> ttBattleCardList = new List<Card>();
    // public List<Card> ttCardListOfCommonCards = new List<Card>();
    // public List<Card> ttCardListOfUncommonCards = new List<Card>();
    // public List<Card> ttCardListOfRareCards = new List<Card>();
    // public List<Card> ttCardListOfEliteCards = new List<Card>();
    private readonly List<int> _temporaryCardQuantityList = new List<int>();
    

    public Card emptyCard;

    [Header("Used for Your Usable Cards when entering Card Battle")]
    public string[] cardList;
    public int[] cardAmount;
    public string[] enemyCards;
    
    [SerializeField] int _numberOfCardsOnPage = 10;
    private int _totalNumberOfCards;
    private int _totalNumberOfPages;
    private int _totalNumberOfCardsOnLastPage;






    [Header("Singleton")]
    public static CardInventory instance;

    private void Awake()
    {//Singleton reference
        instance = this;
        int cardInventoryCount = FindObjectsOfType<CardInventory>().Length;
        if (cardInventoryCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CalculateNumbers()
    {
        //looks at your master card list, and figures out how many pages and remaining cards are needed, these Values are referenced by other scripts
        _totalNumberOfCards = masterCardList.Length;
        var notRoundedUpPagesCount = ((float) _totalNumberOfCards) / _numberOfCardsOnPage;
        _totalNumberOfPages = (int) Mathf.Ceil(notRoundedUpPagesCount);
        _totalNumberOfCardsOnLastPage = _totalNumberOfCards % _numberOfCardsOnPage;
    }

        public void CreateUsableBattleCardLists()
    {
        ttBattleCardList.Clear();
        _temporaryCardQuantityList.Clear();
        
        for (int i = 0; i < masterCardList.Length; i++)
        {
            if (masterCardList[i].amountOwned > 0)
            {
                ttBattleCardList.Add(masterCardList[i]);
            }
        }
        ttBattleCardList.Sort();
        for (int i = 0; i < ttBattleCardList.Count; i++)
        {
            _temporaryCardQuantityList.Add(ttBattleCardList[i].amountOwned);
        }
    }

        public void CheatCodeEntered()
        {
            for (int i = 0; i < masterCardList.Length; i++)
            {
                masterCardList[i].amountOwned = 3;
            }
        }

        

    public void AddCardToInventory(int cardId)
    {
        for (int i = 0; i < masterCardList.Length; i++)
        {
            if (masterCardList[i].cardID == cardId)
            {
                masterCardList[i].amountOwned++;
                break;
            } 
        }
    }

    public Card CardLookupById(int cardId)
    {
        for (int i = 0; i < masterCardList.Length; i++)
        {
            if (masterCardList[i].cardID == cardId)
            {
                return masterCardList[i];
            }
        }

        return masterCardList[0];
    }

    public void RemoveCardFromInventory(int cardId)
    {
        for (int i = 0; i < masterCardList.Length; i++)
        {
            if (masterCardList[i].cardID == cardId)
            {
                masterCardList[i].amountOwned--;
                break;
            } 
        }
    }





    public int RetrieveNumberOfCardsOnPage()
    {//other functions use this to get the private variables
        return _numberOfCardsOnPage;
    }

    public int RetrieveTotalNumberOfCards()
    {//other functions use this to get the private variables
        return _totalNumberOfCards;
    }

    public int RetrieveTotalNumberOfPages()
    {//other functions use this to get the private variables
        return _totalNumberOfPages;
    }
    public int RetrieveTotalNumberOfCardsOnLastPage()
    {//other functions use this to get the private variables
        return _totalNumberOfCardsOnLastPage;
    }

    public int RetrieveTotalNumberOfCardsInMasterList()
    {
        return masterCardList.Length;
    }

    public List<Card> RetrieveBattleCardList()
    {
        return ttBattleCardList;
    }

    public List<int> RetrieveBattleQuantityList()
    {
        return _temporaryCardQuantityList;
    }

    public int RetrieveTotalNumberOfBattleCards()
    {
        return ttBattleCardList.Count;
    }

}
