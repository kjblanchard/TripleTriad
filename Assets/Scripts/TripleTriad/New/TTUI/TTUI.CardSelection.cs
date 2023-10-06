using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public partial class TTUI
    {
        [Range(0, 1)] [SerializeField] float _continuousAutoScrollSpeed;
        [Range(0, 1)] [SerializeField] float _continuousFastAutoScrollSpeed;


        [Header("Card Selection Stuff")]
        [SerializeField] private Canvas _cardSelectionCanvas;
        [Header("State Controls")] public bool isLoading = false;

        [Header("Scroll Rect Stuff")] [SerializeField]
        ScrollRect imTheScrollRect;

        private float _initialValue;
        private float _targetValue;

        [SerializeField] Transform battleScrollRectSpacer;
        [Range(0, 10f)] [SerializeField] float[] scrollSpeed;

        [Header("Current List of Pages")] public List<TTCardPage> listOfPages = new List<TTCardPage>();

        private TTCardPage _pageToModify;
        private int _totalOfCardsOnLastPage;
        private int _totalPages;
        private int _pageToPutThisOn;
        private int _spotToPutThisOn;

        [Header("Prefab References")] [SerializeField]
        GameObject fingerPrefab;

        [SerializeField] GameObject battlePagePrefab;

        [Header("Card Select - UI references")] [SerializeField]
        Text cardDescription;

        [SerializeField] Image cardImage;
        [SerializeField] Text cardRarity;
        [SerializeField] Text pageNumText;

        [Header("CardSelectionAnimators")] [SerializeField]
        Animator cardDisplayAnimator;

        [SerializeField] Animator[] myHandCardAnimators;
        [SerializeField] Animator cardConfirmAnimator;
        [SerializeField] Animator cardSelectionWindowAnimator;
        [SerializeField] Animator cardSelectionCardDisplayAnimator;

        [Header("My Hand References")] [SerializeField]
        Image[] myHandCardImages;

        [SerializeField] Transform[] myHandFingerPoints;



        public void GeneratePages()
        {
            //run calculations in the db, and then generate the amount of pages based on the number of cards in your inventory
            ttdb.CalculateTtBattleNumbers();
            _totalOfCardsOnLastPage = ttdb.RetrieveTotalNumberOfCardsOnLastBattlePage();
            _totalPages = ttdb.RetrieveTotalNumberOfBattlePages();
            if (listOfPages.Count > 0)
            {
                for (int i = 0; i < listOfPages.Count; i++)
                {
                    Destroy(listOfPages[i].gameObject);
                }
            }
            listOfPages.Clear();


            for (int i = 0; i < _totalPages; i++)
            {
                GameObject newBattlePage = Instantiate(battlePagePrefab, battleScrollRectSpacer, false);
                listOfPages.Add(newBattlePage.GetComponent<TTCardPage>());
            }

            ModifyLastPagesActiveCards(_totalOfCardsOnLastPage);
        }

        private void ModifyLastPagesActiveCards(int totalOfCardsOnLastPage)
        {
            //Activates or deactivates the cards on the last page, based on how many cards are on it
            if (totalOfCardsOnLastPage != 0)
            {
                _pageToModify = listOfPages[listOfPages.Count - 1];
                _pageToModify.currentActiveCardsOnPage = 0;
                for (int i = 0; i < _pageToModify.cardGameObjects.Length; i++)
                {
                    if (totalOfCardsOnLastPage > i)
                    {
                        _pageToModify.cardGameObjects[i].SetActive(true);
                        _pageToModify.currentActiveCardsOnPage++;
                    }
                    else
                    {
                        _pageToModify.cardGameObjects[i].SetActive(false);
                    }
                }
            }
        }

        public void InitializeUiFromTtBattleList()
        {
            //take your renumbered list, and populate them onto the pages that your cursor points to, based on if you own the card, if you've lost the card, or if you've never owned the card
            var numberOfCardsOnPage = ttdb.RetrieveTotalNumberOfCardsOnPage();
            for (int i = 0; i < ttdb.currentBattleSelectableCards.Count; i++)
            {
                _pageToPutThisOn = i / numberOfCardsOnPage;
                _spotToPutThisOn = i - (_pageToPutThisOn * 10);
                listOfPages[_pageToPutThisOn].cardQuantityLocations[_spotToPutThisOn].text =
                    ttdb.currentBattleSelectableCards[i].amountOwned.ToString();
                listOfPages[_pageToPutThisOn].cardNameLocations[_spotToPutThisOn].text =
                    ttdb.currentBattleSelectableCards[i].cardName;
            }
        }

        public void ResetScrollRectPosition()
        {
            imTheScrollRect.horizontalNormalizedPosition = 0;
        }

        public IEnumerator PlayCardSelectionPageScrollAnimation(
            CardSelectionState.PageScrollDirections whichWayToScroll)
        {
            switch (whichWayToScroll)
            {
                case CardSelectionState.PageScrollDirections.GoingForward:
                    var initialValue = imTheScrollRect.horizontalNormalizedPosition;
                    var target = initialValue + (float) 1 / (listOfPages.Count - 1);

                    isLoading = true;
                    while (initialValue != target)
                    {
                        initialValue = Mathf.MoveTowards(initialValue, target,
                            scrollSpeed[listOfPages.Count - 1] * Time.deltaTime);
                        imTheScrollRect.horizontalNormalizedPosition = initialValue;
                        yield return null;
                    }

                    isLoading = false;
                    break;
                case CardSelectionState.PageScrollDirections.GoingBackward:
                    //these have a weird speed issue, so have to use a scroll speed array that has different values for different amount of pages
                    _initialValue = imTheScrollRect.horizontalNormalizedPosition;
                    _targetValue = _initialValue - (float) 1 / (listOfPages.Count - 1);

                    isLoading = true;
                    while (_initialValue != _targetValue)
                    {
                        _initialValue = Mathf.MoveTowards(_initialValue, _targetValue,
                            scrollSpeed[listOfPages.Count - 1] * Time.deltaTime);
                        imTheScrollRect.horizontalNormalizedPosition = _initialValue;
                        yield return null;
                    }

                    isLoading = false;
                    break;
            }
        }

        public void KeepFingerOnProperLocationInCardSelection()
        {
            //finger is turned on and off when in update method by the isloading bool
            if (isLoading == false)
            {
                if (!fingerPrefab.gameObject.activeInHierarchy)
                {
                    fingerPrefab.gameObject.SetActive(true);
                }

                fingerPrefab.transform.position = listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .fingerLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].transform.position;
            }
            else
            {
                fingerPrefab.gameObject.SetActive(false);
            }
        }

        public void UpdateTheBigCardDisplayUi()
        {
            //This updates various field on the screen, depending on if you own the card, if you've lost the card, or if you've never owned the card
            cardDescription.text = ttdb.currentBattleSelectableCards[ttdb.RetrieveCardSelectionCurrentSpotInInventory()]
                .description;
            cardRarity.text = ttdb.currentBattleSelectableCards[ttdb.RetrieveCardSelectionCurrentSpotInInventory()]
                .cardsRarity.ToString();
            cardImage.sprite = ttdb.currentBattleSelectableCards[ttdb.RetrieveCardSelectionCurrentSpotInInventory()]
                .playerImage;
            PlayDisplayCardAnimation();
        }

        public void UpdatePageNum()
        {
            pageNumText.text = (ttdb.RetrieveCardSelectionCurrentPageNumber() + 1).ToString();
        }

        private void PlayDisplayCardAnimation()
        {
            cardDisplayAnimator.Play("Rest");
        }

        public void PlayMyHandAnimation(int cardToDisplay)
        {
            myHandCardAnimators[cardToDisplay].Play("MyHandSelect");
        }

        public void PlayRestAnimation(int cardToDisplay)
        {
            myHandCardAnimators[cardToDisplay].Play("Rest");
        }

        public void TurnOnHandAnimator()
        {
            ttdb.RetrievePlayerHandCardAtCurrentPosition().cardCanvas.enabled = true;
        }
        public void TurnOffHandAnimatorForCancel()
        {
            ttdb.RetrievePlayerHandCardAtCancelPosition().cardCanvas.enabled = false;
            PlayRestAnimation(ttMan.ttDb.currentHandSelectionsList.Count -1);
        }

        public void UpdateMyHandImage()
        {
            myHandCardImages[ttdb.currentHandSelectionsList.Count].sprite = ttdb
                .currentBattleSelectableCards[ttMan.ttDb.RetrieveCardSelectionCurrentSpotInInventory()].playerImage;
        }
        public void UpdateMyHandImagesRandom()
        {
            var ttCards = ttdb.RetrieveAllPlayerHandCardsFull();
            for (int i = 0; i < ttCards.Length; i++)
            {
                myHandCardImages[i].sprite = ttCards[i].whatCardIAm.playerImage;
            }
        }

        public void UpdateQuantityAfterSelecting()
        {
            listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .cardQuantityLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].text =
                ttdb.currentBattleQuantityForCards[ttdb.RetrieveCardSelectionCurrentSpotInInventory()].ToString();
        }

        public void UpdateColorOfCardSelectionText()
        {
            if (ttdb.currentBattleQuantityForCards[ttdb.RetrieveCardSelectionCurrentSpotInInventory()] > 0)
            {
                listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .cardNameLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].color = Color.white;
                listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .cardQuantityLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].color = Color.white;
            }
            else
            {
                listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .cardNameLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].color = Color.gray;
                listOfPages[ttdb.RetrieveCardSelectionCurrentPageNumber()]
                    .cardQuantityLocations[ttdb.RetrieveCardSelectionFingerLocationOnCurrentPage()].color = Color.gray;
            }
        }

        public void UpdateQuantityAfterRemovingFromSelection()
        {
            listOfPages[ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].pageNum]
                    .cardQuantityLocations[
                        ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotOnPage]
                    .text =
                ttdb.currentBattleQuantityForCards[
                        ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotInCardInv]
                    .ToString();
        }

        public void UpdateColorOfCardSelectionWhenRemovedText()
        {
            if (ttdb.currentBattleQuantityForCards[
                    ttdb.currentBattleQuantityForCards[
                        ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotInCardInv]] >
                0)
            {
                listOfPages[ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].pageNum]
                        .cardNameLocations[
                            ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotOnPage]
                        .color =
                    Color.white;
                listOfPages[ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].pageNum]
                        .cardQuantityLocations[
                            ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotOnPage]
                        .color =
                    Color.white;
            }
            else
            {
                listOfPages[ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].pageNum]
                        .cardNameLocations[
                            ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotOnPage]
                        .color =
                    Color.gray;
                listOfPages[ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].pageNum]
                        .cardQuantityLocations[
                            ttdb.currentHandSelectionsList[ttMan.ttDb.currentHandSelectionsList.Count - 1].spotOnPage]
                        .color =
                    Color.gray;
            }
        }


        public void TurnOnCardSelectionScreenUiElementsFromCancel()
        {
            _cardSelectionCanvas.enabled = true;
            _cardConfirmCanvas.enabled = false;


        }

        public int RetrieveNumberOfCardsActiveOnPage(int currentPageToCheck)
        {
            return listOfPages[currentPageToCheck].currentActiveCardsOnPage;
        }

        public float RetrieveRegularAutoScrollSpeed()
        {
            return _continuousAutoScrollSpeed;
        }

        public float RetrieveFastAutoScrollSpeed()
        {
            return _continuousFastAutoScrollSpeed;
        }

        public void TurnOffCardSelectionCanvas()
        {
            _cardSelectionCanvas.enabled = false;
        }

    }
}