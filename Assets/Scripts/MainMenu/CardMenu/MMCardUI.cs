using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMCardUI : MonoBehaviour
{
    [Header("State Controls")]
    public bool isLoading = false;

    [Header("Scroll Rect Stuff")]
    [SerializeField] ScrollRect imTheScrollRect;
    [SerializeField] Transform scrollRectSpacer;
    [Range(0,10f)]
    [SerializeField] float[] scrollSpeed;

    [Header("Current List of Pages")]
    public List<CardPage> listOfPages = new List<CardPage>();

    [Header("Prefab References")]
    [SerializeField] GameObject fingerPrefab;
    [SerializeField] GameObject pagePrefab;

    [Header("UI References for Card Displays")]
    [SerializeField] Text currentPageText;
    [SerializeField] Text bigCardNameText;
    [SerializeField] Text bigCardNumberText;
    [SerializeField] Image bigCardImage;
    [SerializeField] Text smallCardNameText;
    [SerializeField] Text smallCardRarityText;
    [SerializeField] Text smallCardOriginText;
    [SerializeField] Text smallCardDescriptionText;

    public void ResetScrollRectPosition()
    {
        imTheScrollRect.horizontalNormalizedPosition = 0;
    }

    public IEnumerator MoveForwardScrollAnimation(int pagesCount)
    {//these have a weird speed issue, so have to use a scroll speed array that has different values for different amount of pages
        var initialValue = imTheScrollRect.horizontalNormalizedPosition;
        var target = imTheScrollRect.horizontalNormalizedPosition + (float)1 / (pagesCount -1);

        isLoading = true;
        while (initialValue != target)
        {

            initialValue = Mathf.MoveTowards(initialValue, target, scrollSpeed[pagesCount -1 ] * Time.deltaTime);
            imTheScrollRect.horizontalNormalizedPosition = initialValue;
            yield return null;
        }
        isLoading = false;

    }
    public IEnumerator MoveBackwardScrollAnimation(int pagesCount)
    {//these have a weird speed issue, so have to use a scroll speed array that has different values for different amount of pages
        var initialValue = imTheScrollRect.horizontalNormalizedPosition;
        var target = imTheScrollRect.horizontalNormalizedPosition - (float)1 / (pagesCount -1);

        isLoading = true;
        while (initialValue != target)
        {
            initialValue = Mathf.MoveTowards(initialValue, target, scrollSpeed[pagesCount -1] * Time.deltaTime);
            imTheScrollRect.horizontalNormalizedPosition = initialValue;
            yield return null;
        }
        isLoading = false;

    }
    public void KeepFingerOnProperLocation(int pageNum, int fingerPos)
    {//finger is turned on and off when in update method by the isloading bool
        if(isLoading == false)
        {
            if (!fingerPrefab.gameObject.activeInHierarchy) { fingerPrefab.gameObject.SetActive(true); }
            fingerPrefab.transform.position = listOfPages[pageNum - 1].fingerLocations[fingerPos].transform.position;
        }
        else
        {
            fingerPrefab.gameObject.SetActive(false);
        }

    }

    public void GeneratePages()
    {//run calculations, and then generate the amount of pages based on the number of cards in your inventory
        CardInventory.instance.CalculateNumbers();
        var totalOfCardsOnLastPage = CardInventory.instance.RetrieveTotalNumberOfCardsOnLastPage();
        var totalPages = CardInventory.instance.RetrieveTotalNumberOfPages();
        listOfPages.Clear();

        for (int i = 0; i < totalPages; i++)
        {
            GameObject newPage = Instantiate(pagePrefab, scrollRectSpacer, false) as GameObject;
            listOfPages.Add(newPage.GetComponent<CardPage>());

        }

        ModifyLastPagesActiveCards(totalOfCardsOnLastPage);
    }

    private void ModifyLastPagesActiveCards(int totalOfCardsOnLastPage)
    {//Activates or deactivates the cards on the last page, based on how many cards are on it
        if (totalOfCardsOnLastPage != 0)
        {
            var pageToModify = listOfPages[listOfPages.Count - 1];
            pageToModify.currentActiveCardsOnPage = 0;
            for (int i = 0; i < pageToModify.cardGameObjects.Length; i++)
            {
                if (totalOfCardsOnLastPage > i)
                {
                    pageToModify.cardGameObjects[i].SetActive(true);
                    pageToModify.currentActiveCardsOnPage++;
                }
                else
                {
                    pageToModify.cardGameObjects[i].SetActive(false);
                }
            }
        }
    }

    public void InitializeUIFromRenumberedList()
    {//take your renumbered list, and populate them onto the pages that your cursor points to, based on if you own the card, if you've lost the card, or if you've never owned the card
        int pageToPutThisOn = 0;
        int positionToPutThisOn = 0;
        var numberOfCardsOnPage = CardInventory.instance.RetrieveNumberOfCardsOnPage();
        for (int i = 0; i < CardInventory.instance.renumberedCardList.Count; i++)
        {
            pageToPutThisOn = (int)i / numberOfCardsOnPage;
            positionToPutThisOn = (int)(i - (pageToPutThisOn*10));
            if (CardInventory.instance.renumberedCardList[i].amountOwned > 0)
            {
                listOfPages[pageToPutThisOn].cardQuantityLocations[positionToPutThisOn].text = CardInventory.instance.renumberedCardList[i].amountOwned.ToString();
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].text = CardInventory.instance.renumberedCardList[i].cardName;
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].color = Color.white;
                listOfPages[pageToPutThisOn].cardImageLocations[positionToPutThisOn].sprite = CardInventory.instance.renumberedCardList[i].playerImage;
            }
            else if(CardInventory.instance.renumberedCardList[i].hasOwnedBefore)
            {
                listOfPages[pageToPutThisOn].cardQuantityLocations[positionToPutThisOn].text = "0";
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].text = CardInventory.instance.renumberedCardList[i].cardName;
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].color = Color.gray;
                listOfPages[pageToPutThisOn].cardImageLocations[positionToPutThisOn].sprite = CardInventory.instance.renumberedCardList[i].playerImage;

            }
            else
            {
                listOfPages[pageToPutThisOn].cardQuantityLocations[positionToPutThisOn].text = "0";
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].text = CardInventory.instance.emptyCard.cardName;
                listOfPages[pageToPutThisOn].cardNameLocations[positionToPutThisOn].color = Color.gray;
                listOfPages[pageToPutThisOn].cardImageLocations[positionToPutThisOn].sprite = CardInventory.instance.emptyCard.playerImage;

            }
        }
    }

    public void UpdateUIElementsOnCursorMove(int cardToLookup)
    {//This updates various field on the screen, depending on if you own the card, if you've lost the card, or if you've never owned the card
        if (CardInventory.instance.renumberedCardList[cardToLookup].amountOwned != 0)
        {
            bigCardNameText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardName;
            bigCardNumberText.text = "# " + CardInventory.instance.renumberedCardList[cardToLookup].cardID.ToString();
            bigCardImage.sprite = CardInventory.instance.renumberedCardList[cardToLookup].playerImage;
            smallCardNameText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardName;
            smallCardRarityText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardsRarity.ToString();
            smallCardOriginText.text = "N/A";
            smallCardDescriptionText.text = CardInventory.instance.renumberedCardList[cardToLookup].description;
        }
        else if (CardInventory.instance.renumberedCardList[cardToLookup].hasOwnedBefore)
        {
            bigCardNameText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardName;
            bigCardNumberText.text = "# " + CardInventory.instance.renumberedCardList[cardToLookup].cardID.ToString();
            bigCardImage.sprite = CardInventory.instance.renumberedCardList[cardToLookup].playerImage;
            smallCardNameText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardName;
            smallCardRarityText.text = CardInventory.instance.renumberedCardList[cardToLookup].cardsRarity.ToString();
            smallCardOriginText.text = "You Lost it to {Someone}";
            smallCardDescriptionText.text = CardInventory.instance.renumberedCardList[cardToLookup].description;
        }
        else
        {
            bigCardNameText.text = CardInventory.instance.emptyCard.cardName;
            bigCardNumberText.text = "";
            bigCardImage.sprite = CardInventory.instance.emptyCard.playerImage;
            smallCardNameText.text = "";
            smallCardRarityText.text = "";
            smallCardOriginText.text = "";
            smallCardDescriptionText.text = "";
        }
    }

    public void UpdatePageNumber(int currentPage =1)
    {//Updates the page number on the top of the screen
        currentPageText.text = currentPage.ToString();
    }


    public void RenumberCardList()
    {//Go through your Master Card list 4 times, each looking for the type of rarity that you are sorting it by, and then renumber your card ID's on your cards and add them to your Renumberd ListView
        int currentCardNumberToBeHandedOut = 0;
        CardInventory.instance.renumberedCardList.Clear();

        for (int i = 0; i < CardInventory.instance.masterCardList.Length; i++)
        {
            if (CardInventory.instance.masterCardList[i].cardsRarity == Card.cardRarity.Common)
            {

                CardInventory.instance.masterCardList[i].cardID = currentCardNumberToBeHandedOut;
                currentCardNumberToBeHandedOut++;
                CardInventory.instance.renumberedCardList.Add(CardInventory.instance.masterCardList[i]);
                continue;
            }
        }
        for (int i = 0; i < CardInventory.instance.masterCardList.Length; i++)
        {
            if (CardInventory.instance.masterCardList[i].cardsRarity == Card.cardRarity.Uncommon)
            {

                CardInventory.instance.masterCardList[i].cardID = currentCardNumberToBeHandedOut;
                currentCardNumberToBeHandedOut++;
                CardInventory.instance.renumberedCardList.Add(CardInventory.instance.masterCardList[i]);
                continue;
            }
        }
        for (int i = 0; i < CardInventory.instance.masterCardList.Length; i++)
        {
            if (CardInventory.instance.masterCardList[i].cardsRarity == Card.cardRarity.Rare)
            {

                CardInventory.instance.masterCardList[i].cardID = currentCardNumberToBeHandedOut;
                currentCardNumberToBeHandedOut++;
                CardInventory.instance.renumberedCardList.Add(CardInventory.instance.masterCardList[i]);
                continue;
            }

        }
        for (int i = 0; i < CardInventory.instance.masterCardList.Length; i++)
        {
            if (CardInventory.instance.masterCardList[i].cardsRarity == Card.cardRarity.Elite)
            {

                CardInventory.instance.masterCardList[i].cardID = currentCardNumberToBeHandedOut;
                currentCardNumberToBeHandedOut++;
                CardInventory.instance.renumberedCardList.Add(CardInventory.instance.masterCardList[i]);
                continue;
            }
        }

    }



}
