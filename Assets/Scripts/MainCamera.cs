using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainCamera : MonoBehaviour
{

    public bool shouldDisplayTransition;

    [SerializeField] private bool testing = true;
    public Transform target; // We want it to only focus on the "transform" category cause thats all that matters
    // we use target and not player, as we will likely want the camera to focus on different things at different times

    // Start is called before the first frame update, so that there is no blank space

    public Tilemap theMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit; //these private vectors will store the furthest that the camera can go in those directions, so the game can know how far to move the camera
    //make sure that you set theMap public to the largest tile in the map

    private float halfHeight;
    private float halfWidth;
    //Only half of our camera is going off the edge, so we just need to move the half

    public int musicToPlay; //what music to play will be set in the camera screen
    private bool musicStarted;

    public string mapDisplayText = "Welcome to Garbage";
    //[SerializeField] private CinemachineVirtualCamera followCam;



}
