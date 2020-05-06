using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class MMCardController : MonoBehaviour
{

    [Header("CardUI Script Reference")]
    [SerializeField] MMCardUI cardUI;

    [Header("PageScrolling Logic")]
    private PageScrollDirections currentPageScrollDirection;
    private enum PageScrollDirections
    {
        GoingForward,
        GoingBackward,
    }
    [Header("CoroutineHolderForReference")]
    Coroutine continuousCoroutineReference;

    [Header("ScrollSpeedControllersForUp-Down")]
    [SerializeField] float autoScrollSpeed = 1.0f;
    [SerializeField] bool regularAutoScrollSpeed = true;
    [SerializeField] float fastAutoScrollSpeed = 0.10f;

    [Header("Cursor Memory")]
    [SerializeField] int fingerLocationOnCurrentPage;
    [SerializeField] int currentPageNumber = 1;
    [SerializeField] int currentSpotInCardInventory;

    [Header("Singleton")]
    public static MMCardController instance;

    [Header("Animator Reference")] //used for modifying bools and such, can likely remove in future
    [SerializeField] Animator coryAnimator;



    private void Awake()
    {//Used to make sure there is only one instance - singleton
        instance = this;
        int cardControllerCount = FindObjectsOfType<MMCardController>().Length;
        if (cardControllerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }


    void Start()
    {//Runs the initialization values so that it will not be broken or have values when it opens from the last time

        InitializeCardAlbum();

    }

    private void Update()
    {
        var _menuOpened = false;
        if (_menuOpened)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (continuousCoroutineReference != null) { StopCoroutine(continuousCoroutineReference); }
                currentPageScrollDirection = PageScrollDirections.GoingForward;
                continuousCoroutineReference = StartCoroutine(PageScrollContinuously());
                SetCoryBoolWalking();//for fun
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (continuousCoroutineReference != null) { StopCoroutine(continuousCoroutineReference); }
                currentPageScrollDirection = PageScrollDirections.GoingBackward;
                continuousCoroutineReference = StartCoroutine(PageScrollContinuously());
                SetCoryBoolWalking();//for fun

            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (continuousCoroutineReference != null) { StopCoroutine(continuousCoroutineReference); }
                continuousCoroutineReference = StartCoroutine(ScrollContinuously(false));
                SetCoryBoolSwordIdle();//for fun

            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (continuousCoroutineReference != null) { StopCoroutine(continuousCoroutineReference); }
                continuousCoroutineReference = StartCoroutine(ScrollContinuously(true));
                SetCoryBoolSwordIdle();//for fun
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(SetCoryBoolSwordAtk2());//for fun
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                StopCoroutine(continuousCoroutineReference);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                StopCoroutine(continuousCoroutineReference);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                StopCoroutine(continuousCoroutineReference);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                StopCoroutine(continuousCoroutineReference);
            }

            cardUI.KeepFingerOnProperLocation(currentPageNumber, fingerLocationOnCurrentPage);
        }

    }

    private void ChangePage()
    {
        bool canPageSwitch;
        var totalNumberOfPages = CardInventory.instance.RetrieveTotalNumberOfPages();

        switch (currentPageScrollDirection)
        {
            case PageScrollDirections.GoingForward:

                canPageSwitch = CanISwitchPages(true);
                if (canPageSwitch)
                {
                    StartCoroutine(cardUI.MoveForwardScrollAnimation(totalNumberOfPages));
                    MoveCursorToNextPage(true);
                    SoundManager.instance.PlaySFX(6);

                }
                break;
            case PageScrollDirections.GoingBackward:
                canPageSwitch = CanISwitchPages(false);
                if (canPageSwitch)
                {
                    StartCoroutine(cardUI.MoveBackwardScrollAnimation(totalNumberOfPages));
                    MoveCursorToNextPage(false);
                    SoundManager.instance.PlaySFX(6);
                }
                break;
            default:
                break;

        }
    }

    private void InitializeCardAlbum()
    {
        StopAllCoroutines();//in here just to not forget about some of the coroutines from the auto scrollers, they need to be closed when menu is closed
        cardUI.ResetScrollRectPosition();
        cardUI.GeneratePages();
        cardUI.RenumberCardList();
        cardUI.InitializeUIFromRenumberedList();
        currentPageNumber = 1;
        currentSpotInCardInventory = 0;
        fingerLocationOnCurrentPage = 0;
        cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);
        cardUI.UpdatePageNumber();

    }

    private void MoveCursorInMenu(bool isIncrementing = false)
    {
        if (isIncrementing)
        {
            bool canScroll = CanIScroll(isIncrementing);
            if (canScroll)
            {
                if (fingerLocationOnCurrentPage >= cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage - 1)
                {
                    fingerLocationOnCurrentPage = 0;
                    currentSpotInCardInventory = currentSpotInCardInventory - cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage + 1;
                }
                else
                {
                    fingerLocationOnCurrentPage++;
                    currentSpotInCardInventory++;
                }
                SoundManager.instance.PlaySFX(0);
                cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);
            }

        }
        else
        {
            bool canScroll = CanIScroll(isIncrementing);
            if (canScroll)
            {
                if (fingerLocationOnCurrentPage == 0)
                {
                    fingerLocationOnCurrentPage = cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage - 1;
                    currentSpotInCardInventory = currentSpotInCardInventory + cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage - 1;

                }
                else
                {
                    fingerLocationOnCurrentPage--;
                    currentSpotInCardInventory--;
                }
                SoundManager.instance.PlaySFX(0);
                cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);
            }

        }

    }
    private void MoveCursorToNextPage(bool isIncrementing)
    {
        if (isIncrementing)
        {
            currentPageNumber++;
            currentSpotInCardInventory = currentSpotInCardInventory + 10;
            if (currentSpotInCardInventory < CardInventory.instance.masterCardList.Length)
            {
                cardUI.UpdatePageNumber(currentPageNumber);
                cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);
            }
            else
            {
                cardUI.UpdatePageNumber(currentPageNumber);
                fingerLocationOnCurrentPage = cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage - 1;
                currentSpotInCardInventory = CardInventory.instance.masterCardList.Length - 1;
                cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);

            }
        }
        else
        {
            currentPageNumber--;
            currentSpotInCardInventory = currentSpotInCardInventory - 10;
            cardUI.UpdatePageNumber(currentPageNumber);
            cardUI.UpdateUIElementsOnCursorMove(currentSpotInCardInventory);
        }

    }

    private bool CanIScroll(bool isIncrementing)
    {
        if (cardUI.isLoading) { return false; }
        if (isIncrementing)
        {
            if (cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (cardUI.listOfPages[currentPageNumber - 1].currentActiveCardsOnPage > 1)

            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private bool CanISwitchPages(bool isIncrementing)
    {
        if (cardUI.isLoading) { return false; }
        if (isIncrementing)
        {
            if (currentPageNumber < cardUI.listOfPages.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (currentPageNumber > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private IEnumerator ScrollContinuously(bool isIncrementing)
    {
        int numberToIncrement = 0;
        regularAutoScrollSpeed = true;
        while (true)
        {
            if (regularAutoScrollSpeed)
            {
                MoveCursorInMenu(isIncrementing);
                numberToIncrement++;
                if (numberToIncrement >= 2) { regularAutoScrollSpeed = false; }
                yield return new WaitForSeconds(autoScrollSpeed);
            }
            else if (!regularAutoScrollSpeed)
            {
                MoveCursorInMenu(isIncrementing);
                yield return new WaitForSeconds(fastAutoScrollSpeed);
            }
        }

    }

    private IEnumerator PageScrollContinuously()
    {
        while (true)
        {
            ChangePage();
            yield return new WaitForSeconds(autoScrollSpeed);
        }
    }

    private void SetCoryBoolWalking()
    {

        coryAnimator.SetBool("isWalking", true);
        coryAnimator.SetBool("isSwordIdling", false);


    }
    private IEnumerator SetCoryBoolSwordAtk2()
    {

        coryAnimator.SetBool("isSwordAttack2", true);
        yield return null;
        coryAnimator.SetBool("isSwordAttack2", false);


    }

    private void SetCoryBoolSwordIdle()
    {

        coryAnimator.SetBool("isWalking", false);
        coryAnimator.SetBool("isSwordIdling", true);


    }
}