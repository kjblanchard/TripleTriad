using System;
using System.Collections;
using UnityEngine;

namespace ETF.TripleTriad
{
    public class CardSelectionState : TtState
    {
        #region configuration


        private int _numberToIncrement;
        private bool _regularAutoScrollSpeed;

        bool _rightButtonDown, _leftButtonDown, _upButtonDown, _downButtonDown = false;
        int _currentUpDownIterations = 0;
        float _pageTurnTime = 0;
        float _upDownSelectionWaitTime = 0;


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
            }
            _leftButtonDown = _rightButtonDown = false;
        }

        public override void Execute()
        {
            if (_pageTurnTime > 0)
            {
                Math.Clamp(_pageTurnTime -= Time.deltaTime, 0, float.MaxValue);
            }
            if (_upDownSelectionWaitTime > 0)
            {
                Math.Clamp(_upDownSelectionWaitTime -= Time.deltaTime, 0, float.MaxValue);
            }
            ListenForUserInputs();
            _ttUi.KeepFingerOnProperLocationInCardSelection();
        }

        public override void End()
        {
        }

        #region Functions

        private void ListenForUserInputs()
        {

            if (Input.GetKeyDown(KeyCode.D) || (Input.GetButtonDown("right")))
            {
                _rightButtonDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.A) || (Input.GetButtonDown("left")))
            {
                _leftButtonDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.W) || (Input.GetButtonDown("up")))
            {
                _upButtonDown = true;
                // _currentUpDownIterations += Time.deltaTime;
            }
            else if (Input.GetKeyDown(KeyCode.S) || (Input.GetButtonDown("down")))
            {
                _downButtonDown = true;
                // _currentUpDownIterations += Time.deltaTime;
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
                _rightButtonDown = false;
            }
            else if (Input.GetKeyUp(KeyCode.A) || (Input.GetButtonUp("left")))
            {
                _leftButtonDown = false;
            }
            else if (Input.GetKeyUp(KeyCode.W) || (Input.GetButtonUp("up")))
            {
                _upButtonDown = false;
                _currentUpDownIterations = 0;
            }
            else if (Input.GetKeyUp(KeyCode.S) || (Input.GetButtonUp("down")))
            {
                _downButtonDown = false;
                _currentUpDownIterations = 0;
            }
            if (_rightButtonDown)
            {
                ChangePage(PageScrollDirections.GoingForward);
            }
            if (_leftButtonDown)
            {
                ChangePage(PageScrollDirections.GoingBackward);
            }
            if (_upButtonDown)
            {
                MoveCursorUpDownInMenu(WhichUpDownDirection.IsMovingUp);
            }
            if (_downButtonDown)
            {
                MoveCursorUpDownInMenu(WhichUpDownDirection.IsMovingDown);
            }
        }

        private void ChangePage(PageScrollDirections whichWayToScroll)
        {
            if (_pageTurnTime > 0)
                return;
            _pageTurnTime = _ttUi.RetrieveRegularAutoScrollSpeed() * .75f;

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
            if (_upDownSelectionWaitTime > 0)
                return;

            _upDownSelectionWaitTime = _currentUpDownIterations >= 2 ? _ttUi.RetrieveFastAutoScrollSpeed() : _ttUi.RetrieveRegularAutoScrollSpeed();

            if (!_ttLogic.CanIScrollOnCardSelect(whichWayToScroll)) return;
            _currentUpDownIterations++;
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

        private void CardSelection()
        {
            if (!_ttLogic.CanSelectCardInCardSelection(_ttDb.RetrieveCardSelectionCurrentSpotInInventory()))
            {
                SoundManager.instance.PlaySFX(3);
                return;
            }
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
            var cardChoice = new LastItemChosen(_ttDb.RetrieveCardSelectionCurrentPageNumber(),
                _ttDb.RetrieveCardSelectionFingerLocationOnCurrentPage(),
                _ttDb.RetrieveCardSelectionCurrentSpotInInventory());
            _ttDb.currentHandSelectionsList.Add(cardChoice);
        }

        private void CancelLastSelection()
        {
            if (!_ttLogic.CanRemoveCardFromCardSelection()) return;
            ModifyCardQuantity(ModifyingCardQuantity.IsRemovingCard);
            _ttUi.TurnOffHandAnimatorForCancel();
            _ttDb.currentHandSelectionsList.RemoveAt(_ttDb.currentHandSelectionsList.Count - 1);
            SoundManager.instance.PlaySFX(1);
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
