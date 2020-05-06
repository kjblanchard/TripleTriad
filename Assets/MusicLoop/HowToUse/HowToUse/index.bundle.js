webpackJsonp([1],{

/***/ 5:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_uikit__ = __webpack_require__(3);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_uikit___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_uikit__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__js_htmlMerge_js__ = __webpack_require__(0);




const fontLoaded = function() {
    const Introloop = document.getElementById("Introloop")
    const IntroloopMotto = document.getElementById("IntroloopMotto")

    IntroloopMotto.style.opacity = 0
    Introloop.style.opacity = 0
    window.setTimeout(() => Introloop.classList.add("uk-animation-slide-left-small"), 100)
    window.setTimeout(() => IntroloopMotto.classList.add("uk-animation-slide-right-small"), 200)
}

Object(__WEBPACK_IMPORTED_MODULE_1__js_htmlMerge_js__["a" /* default */])(fontLoaded)

document.addEventListener("DOMContentLoaded",()=>{

    __WEBPACK_IMPORTED_MODULE_0_uikit___default.a.util.on("#exampleSlideshow", "itemshown", (elem, item) => {
        //console.log("SLIDE " + item.index)
        __WEBPACK_IMPORTED_MODULE_0_uikit___default.a.switcher("#exampleSwitcher").show(item.index)
    });

});


/***/ })

},[5]);