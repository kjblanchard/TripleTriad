using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETF.TripleTriad
{
    public class CardSelectionState : TtState
    {
        #region configuration


        private int _numberToIncrement;
        private bool _regularAutoScrollSpeed;
        //[SerializeField] TripleTriadManager ttMan;

        //reference to the gamestate that we were coming from

        //reference to the running coroutine
        Coroutine _continuousCoroutineReferenceDown;
        Coroutine _continuousCoroutineReferenceLeft;
        Coroutine _continuousCoroutineReferenceRight;
        Coroutine _continuousCoroutineReferenceUp;


        //enums to use for methods
        public enum PageScrollDirections
        {
            GoingForward,
            GoingBackward,
        }

        private enum ModifyingCardQuantity
        {
            IsAddingCard,
            IsRemovingCard,
        }

        public enum WhichUpDownDirection
        {
            IsMovingDown,
            IsMovingUp,
        }
        
        #endregion

        public override void Startup(int additionalArgs = 0)
        {
            if (additionalArgs == 0)
            {
                InitializeDbForCardSelection();
                InitializeUiForCardSelection();
            }
            else if (additionalArgs == 1)
            {
                _ttUi.TurnOnCardSelectionScreenUiElementsFromCancel();
                CancelLastSelection();
            }
            else if (additionalArgs == 2)
            {
                InitializeDbForCardSelection();
                StartCoroutine(_ttDb.UpdateMyHandTripleTriadCardsWithRandomSelection());
                //ttMan.ttDb.UpdateMyHandTripleTriadCardsWithMySelectionList();
            }
        }
        
        public override void Execute()
        {
            ListenForUserInputs();
            _ttUi.KeepFingerOnProperLocationInCardSelection();
        }

        public override void End()
        {
            if (_continuousCoroutineReferenceDown != null)
            {
                StopCoroutine(_continuousCoroutineReferenceDown);

            }
            if (_continuousCoroutineReferenceUp != null)
            {
                StopCoroutine(_continuousCoroutineReferenceUp);

            }
            if (_continuousCoroutineReferenceRight != null)
            {
                StopCoroutine(_continuousCoroutineReferenceRight);

            }
            if (_continuousCoroutineReferenceLeft != null)
            {
                StopCoroutine(_continuousCoroutineReferenceLeft);

            }
        }

        #region Functions


        private void ListenForUserInputs()
        {
            if (_ttUi.isLoading)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.D) || (Input.GetButtonDown("right")))
            {
                if (_continuousCoroutineReferenceRight != null)
                {
                    StopTheCoroutineScrollingRight();
                }

                _continuousCoroutineReferenceRight =
                    StartCoroutine(PageScrollContinuouslyCo(PageScrollDirections.GoingForward));
            }
            else if (Input.GetKeyDown(KeyCode.A) || (Input.GetButtonDown("left")))
            {
                if (_continuousCoroutineReferenceLeft != null)
                {
                    StopTheCoroutineScrollingLeft();
                }
                _continuousCoroutineReferenceLeft =
                    StartCoroutine(PageScrollContinuouslyCo(PageScrollDirections.GoingBackward));
            }
            else if (Input.GetKeyDown(KeyCode.W) || (Input.GetButtonDown("up")))
            {
                if (_continuousCoroutineReferenceUp != null)
                {
                    StopTheCoroutineScrollingUp();
                }

                _continuousCoroutineReferenceUp =
                    StartCoroutine(ScrollContinuously(WhichUpDownDirection.IsMovingUp));
            }
            else if (Input.GetKeyDown(KeyCode.S) || (Input.GetButtonDown("down")))
            {
                if (_continuousCoroutineReferenceDown != null)
                {
                    StopTheCoroutineScrollingDown();
                }

                _continuousCoroutineReferenceDown =
                    StartCoroutine(ScrollContinuously(WhichUpDownDirection.IsMovingDown));
            }
            else if (Input.GetKeyDown(KeyCode.Space) || (Input.GetButtonDown("Fire1")))
            {
                CardSelection();
            }
            else if (Input.GetKeyDown(KeyCode.C) || (Input.GetButtonDown("Fire2")))
            {
                CancelLastSelection();
            }

            if (Input.GetKeyUp(KeyCode.D) || (Input.GetButtonUp("right")))
            {
                if (_continuousCoroutineReferenceRight!= null)
                {
                    StopTheCoroutineScrollingRight();
                }
            }
            else if (Input.GetKeyUp(KeyCode.A) || (Input.GetButtonUp("left")))
            {
                if (_continuousCoroutineReferenceLeft != null)
                {
                    StopTheCoroutineScrollingLeft();
                }
            }
            else if (Input.GetKeyUp(KeyCode.W) || (Input.GetButtonUp("up")))
            {
                if (_continuousCoroutineReferenceUp != null)
                {
                    StopTheCoroutineScrollingUp();
                }
            }
            else if (Input.GetKeyUp(KeyCode.S) || (Input.GetButtonUp("down")))
            {
                if (_continuousCoroutineReferenceDown != null)
                {
                    StopTheCoroutineScrollingDown();
                }
            }
        }

        private void ChangePage(PageScrollDirections whichWayToScroll)
        {
            switch (whichWayToScroll)
            {
                case PageScrollDirections.GoingForward:

                    if (_ttLogic.CanISwitchPages(whichWayToScroll))
                    {
                        StartCoroutine(_ttUi.PlayCardSelectionPageScrollAnimation(whichWayToScroll));
                        MoveCursorToNextPageInUiAndDb(whichWayToScroll);
                        SoundManager.instance.PlaySFX(6);
                    }

                    break;
                case PageScrollDirections.GoingBackward:
                    if (_ttLogic.CanISwitchPages(whichWayToScroll))
                    {
                        StartCoroutine(_ttUi.PlayCardSelectionPageScrollAnimation(whichWayToScroll));
                        MoveCursorToNextPageInUiAndDb(whichWayToScroll);
                        SoundManager.instance.PlaySFX(6);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(whichWayToScroll), whichWayToScroll, null);
            }
        }

        private void MoveCursorUpDownInMenu(WhichUpDownDirection whichWayToScroll)
        {
            if (!_ttLogic.CanIScrollOnCardSelect(whichWayToScroll)) return;
            if (_ttLogic.AreYouGoingToLoopInCardSelection(whichWayToScroll))
            {
                LoopInCurrentMenu(whichWayToScroll);
            }
            else
            {
                MoveCursorOneSpotInInventory(whichWayToScroll);
            }

            SoundManager.instance.PlaySFX(0);
            _ttUi.UpdateTheBigCardDisplayUi();
        }

        private void MoveCursorOneSpotInInventory(WhichUpDownDirection whichWayAreYouGoing)
        {
            _ttDb.UpdateCardSelectionFingerPositionForScrollingInDb(whichWayAreYouGoing);
        }

        private void LoopInCurrentMenu(WhichUpDownDirection whichUpDown)
        {
            _ttDb.UpdateCardSelectionFingerPositionForLoopingInDb(whichUpDown);
        }

        private void MoveCursorToNextPageInUiAndDb(PageScrollDirections scrollDirection)
        {
            _ttDb.UpdateCardSelectionFingerPositionForPageScrollingInDb(scrollDirection);
            _ttUi.UpdatePageNum();
            if (_ttLogic.DidYouGoPastTheCardSelectionList())
            {
                BounceCursorUpOnPageMove();
            }
            else
            {

                _ttUi.UpdateTheBigCardDisplayUi();

            }
        }

        private void BounceCursorUpOnPageMove()
        {
            _ttDb.UpdateCardSelectionInfoOnScrollingPastEndOfPage();
            _ttUi.UpdateTheBigCardDisplayUi();
        }


        private IEnumerator ScrollContinuously(WhichUpDownDirection scrollDirection)

        {
            _numberToIncrement = 0;
            _regularAutoScrollSpeed = true;
            while (true)
            {
                if (_regularAutoScrollSpeed)
                {
                    MoveCursorUpDownInMenu(scrollDirection);
                    _numberToIncrement++;
                    if (_numberToIncrement >= 2)
                    {
                        _regularAutoScrollSpeed = false;
                    }

                    yield return new WaitForSeconds(_ttUi.RetrieveRegularAutoScrollSpeed());

                }
                else if (!_regularAutoScrollSpeed)
                {
                    MoveCursorUpDownInMenu(scrollDirection);
                    yield return new WaitForSeconds(_ttUi.RetrieveFastAutoScrollSpeed());

                }
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private IEnumerator PageScrollContinuouslyCo(PageScrollDirections whichWayToScroll)
        {
            while (true)
            {
                ChangePage(whichWayToScroll);
                yield return new WaitForSeconds(_ttUi.RetrieveRegularAutoScrollSpeed() * .75f);

            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void CardSelection()
        {
            if (!_ttLogic.CanSelectCardInCardSelection(_ttDb.RetrieveCardSelectionCurrentSpotInInventory()))
            {
                SoundManager.instance.PlaySFX(3);
                return;
            }
            //ttMan.ttUi.TurnOnHandAnimator();
            _ttUi.UpdateMyHandImage();
            _ttUi.PlayMyHandAnimation(_ttDb.currentHandSelectionsList.Count);
            AddChoiceToCardSelectionList();
            ModifyCardQuantity(ModifyingCardQuantity.IsAddingCard);
            SoundManager.instance.PlaySFX(6);
            if (_ttDb.currentHandSelectionsList.Count == 5)
            {
                _ttMan.SendStateChange(_ttMan.cardConfirmationState);
            }
        }

        private void ModifyCardQuantity(ModifyingCardQuantity whatToDoWithQuantity)
        {
            //used to modify the card select quantity in screen and in the db
            switch (whatToDoWithQuantity)
            {
                case ModifyingCardQuantity.IsAddingCard:
                    _ttDb.currentBattleQuantityForCards[
                        _ttDb.RetrieveCardSelectionCurrentSpotInInventory()]--;
                    if (_ttDb.currentBattleQuantityForCards[
                            _ttDb.RetrieveCardSelectionCurrentSpotInInventory()] >
                        0)
                    {
                        _ttUi.UpdateQuantityAfterSelecting();
                    }
                    else
                    {
                        _ttUi.UpdateColorOfCardSelectionText();
                        _ttUi.UpdateQuantityAfterSelecting();
                    }

                    break;
                case ModifyingCardQuantity.IsRemovingCard:
                    var spotInSelectionsListToCheck = _ttDb.currentHandSelectionsList.Count - 1;
                    _ttDb.currentBattleQuantityForCards[
                        _ttDb.currentHandSelectionsList[spotInSelectionsListToCheck].spotInCardInv]++;
                    _ttUi.UpdateQuantityAfterRemovingFromSelection();
                    _ttUi.UpdateColorOfCardSelectionWhenRemovedText();
                    break;
            }
        }

        private void AddChoiceToCardSelectionList()
        {
            //used when you press select
            var cardChoice = new LastItemChosen(_ttDb.RetrieveCardSelectionCurrentPageNumber(),
                _ttDb.RetrieveCardSelectionFingerLocationOnCurrentPage(),
                _ttDb.RetrieveCardSelectionCurrentSpotInInventory());
            _ttDb.currentHandSelectionsList.Add(cardChoice);
        }

        private void CancelLastSelection()
        {
            //used when you press cancel button
            if (!_ttLogic.CanRemoveCardFromCardSelection()) return;
            ModifyCardQuantity(ModifyingCardQuantity.IsRemovingCard);
            _ttUi.TurnOffHandAnimatorForCancel();
            //ttMan.ttUi.PlayRestAnimation(ttMan.ttDb.currentHandSelectionsList.Count - 1);
            _ttDb.currentHandSelectionsList.RemoveAt(_ttDb.currentHandSelectionsList.Count - 1);
            SoundManager.instance.PlaySFX(1);
        }

        private void StopTheCoroutineScrollingRight()
        {
            StopCoroutine(_continuousCoroutineReferenceRight);
        }
        private void StopTheCoroutineScrollingUp()
        {
            StopCoroutine(_continuousCoroutineReferenceUp);
        }
        private void StopTheCoroutineScrollingDown()
        {
            StopCoroutine(_continuousCoroutineReferenceDown);
        }
        private void StopTheCoroutineScrollingLeft()
        {
            StopCoroutine(_continuousCoroutineReferenceLeft);
        }


        private void InitializeDbForCardSelection()
        {
            _ttDb.ClearBattleSelectionsList();
            _ttDb.BringInUsableBattleCards();
            _ttDb.InitializeCardSelectionValuesInDB();
        }

        private void InitializeUiForCardSelection()
        {
            _ttUi.TurnOnCardSelectionScreenUiElementsFromCancel();
            _ttUi.ResetScrollRectPosition();
            _ttUi.GeneratePages();
            _ttUi.InitializeUiFromTtBattleList();
            _ttUi.UpdatePageNum();
            _ttUi.UpdateTheBigCardDisplayUi();
            _ttUi.isLoading = false;
        }

        #endregion

    }
}
