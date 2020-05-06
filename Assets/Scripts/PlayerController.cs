// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayerController : MonoBehaviour
// {
//     public Rigidbody2D theRB; 
//     public float moveSpeed; 
//     public Animator myAnim; 
//
//     public static PlayerController instance; 
//
//     public string areaTransitionName; 
//     private Vector3 bottomLeftLimit;
//     private Vector3 topRightLimit;
//
//     public bool canMove = true;
//
//     public BoxCollider2D[] interactColliders;
//     public bool justStartedMoving;
//
//
//
//     void Start()
//     {
//
//         //bottomLeftLimit =new Vector3(-8.5f, -4.5f, 0);
//         //topRightLimit = new Vector3(8.5f, 4.5f, 0);
//         if (instance == null) 
//         {
//             instance = this; 
//         }
//         else
//         {
//             if (instance != this) 
//             {
//                 Destroy(gameObject); 
//             }
//         }
//         DontDestroyOnLoad(gameObject); 
//
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (canMove == true && GameManager.instance.dialogActive == false && !GameManager.instance.fadingBetweenAreas)
//         {
//             theRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * moveSpeed; //Uses variable theRB's velocity to get axisraw from Horizontal and vertical and multiply by movespeed variable
//
//         }
//         else if (canMove == false || GameManager.instance.fadingBetweenAreas)
//         {
//             theRB.velocity = Vector2.zero;
//         }
//
//         myAnim.SetFloat("moveX", theRB.velocity.x); //sets the float of moveX inside of our animation by getting theRB's velocity from the input for x or y
//         myAnim.SetFloat("moveY", theRB.velocity.y);
//
//         if (canMove == true && GameManager.instance.dialogActive == false )
//             {
//             if (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal")  < 0 || Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0) //If statement to check for input of 1 for idle animation
//             {
//                 if (canMove == true)
//                 {
//                     myAnim.SetFloat("lastMoveX", Input.GetAxisRaw("Horizontal"));
//                     myAnim.SetFloat("lastMoveY", Input.GetAxisRaw("Vertical"));
//
//
//                     ColliderController();
//                 }
//             }
//         }
//
//         transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);
//         //used to create the clamp to keep character in screen, referenced in the camera and the below set bounds script
//
//
//     }
//
//     public void SetBounds(Vector3 botLeft, Vector3 topRight) //Creates a function that can be called on by other scripts, this one from the camera script to prevent the character from walking off screen, I made it work above without this
//     {
//         bottomLeftLimit = botLeft + new Vector3(.5f, 1f, 0f);
//         topRightLimit = topRight + new Vector3(-.5f, -1f, 0f); 
//     }
//
//     private void ColliderController()
//     {
//         if (myAnim.GetFloat("moveX") != 0 || myAnim.GetFloat("moveY") != 0)
//         {
//
//
//             if (myAnim.GetFloat("lastMoveX") > 0 && myAnim.GetFloat("lastMoveX") > myAnim.GetFloat("lastMoveY") && myAnim.GetFloat("lastMoveX") > -myAnim.GetFloat("lastMoveY"))
//             {
//                 int counter = 1;
//                 for (int i = 0; i < interactColliders.Length; i++)
//                 {
//                     if(i == counter)
//                     {
//                         interactColliders[i].enabled = true;
//                     }
//                     else
//                     {
//                         interactColliders[i].enabled = false;
//                     }
//
//                 }
//
//             }
//             else if (myAnim.GetFloat("lastMoveY") > 0 && myAnim.GetFloat("lastMoveY") > myAnim.GetFloat("lastMoveX") && myAnim.GetFloat("lastMoveY") > -myAnim.GetFloat("lastMoveX"))
//             {
//                 int counter = 0;
//                 for (int i = 0; i < interactColliders.Length; i++)
//                 {
//                     if(i == counter)
//                     {
//                         interactColliders[i].enabled = true;
//                     }
//                     else
//                     {
//                         interactColliders[i].enabled = false;
//                     }
//
//                 }
//
//             }
//             else if (myAnim.GetFloat("lastMoveY") < 0 && myAnim.GetFloat("lastMoveY") < myAnim.GetFloat("lastMoveX"))
//             {
//                 int counter = 2;
//                 for (int i = 0; i < interactColliders.Length; i++)
//                 {
//                     if(i == counter)
//                     {
//                         interactColliders[i].enabled = true;
//                     }
//                     else
//                     {
//                         interactColliders[i].enabled = false;
//                     }
//
//                 }
//
//             }
//             else if (myAnim.GetFloat("lastMoveX") < 0 && myAnim.GetFloat("lastMoveX") < myAnim.GetFloat("lastMoveY"))
//             {
//                 int counter = 3;
//                 for (int i = 0; i < interactColliders.Length; i++)
//                 {
//                     if( i == counter)
//                     {
//                         interactColliders[i].enabled = true;
//                     }
//                     else
//                     {
//                         interactColliders[i].enabled = false;
//                     }
//
//                 }
//
//             }
//         }
//     }
// }
