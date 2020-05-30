using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETF.TripleTriad
{
    public class TTCardPage : MonoBehaviour
    {
        public List<Transform> fingerLocations = new List<Transform>();
        public GameObject[] cardGameObjects;

        public Text[] cardNameLocations;

        public Text[] cardQuantityLocations;

        public int currentActiveCardsOnPage = 10;
    }
}

