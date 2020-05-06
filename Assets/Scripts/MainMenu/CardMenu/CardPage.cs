using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPage : MonoBehaviour
{
    public List<Transform> fingerLocations = new List<Transform>();
    public GameObject[] cardGameObjects;

    public Text[] cardNameLocations;
    public Text[] cardQuantityLocations;
    public Image[] cardImageLocations;

    [Header("this is for the ttbattles")]
    public Text[] cardDescriptions;

    public int currentActiveCardsOnPage = 10;
}
