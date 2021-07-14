using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{

    //GLOBAL
    public string[] movementOptions = new string[] { "Character", "Flythrough" };
    public int selectedOptionIndex = 0;
    public int movementSpeed = 5;
    public int speedMultiplier = 6;
    public float camSensitivity = 0.25f;
    private Vector3 unClickedRotation;
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)


    //CHARACTER VARIABLES
    public float gravity = -9.8f;
    public float characterRadius = 0.5f;
    public float characterHeight = 2.0f;
    public float jumpHeight = 1.0f;
    public bool alignToMesh = false;


    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;


    private void Start()
    {
        if (selectedOptionIndex == 0)
        {
            if(photonView.IsMine)
            {
                characterController = this.gameObject.AddComponent<CharacterController>();
            }
        }

        unClickedRotation = transform.eulerAngles;
    }
    void Update()
    {

        //turn off cameras that don't belong to me
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            if (photonView.gameObject.GetComponentInChildren<Camera>() != null)
            {
                photonView.gameObject.GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
        }

        if(photonView.IsMine)
        {
            #region MOUSE ORBITTING

            if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) // right and middle click only 
            {
                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSensitivity, lastMouse.x * camSensitivity, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
                transform.eulerAngles = lastMouse;
                unClickedRotation = lastMouse;
            }
            else
            {
                transform.eulerAngles = unClickedRotation;
            }
            lastMouse = Input.mousePosition;

            #endregion


            #region CHARACTER MOVEMENT

            //if character movement
            if (selectedOptionIndex == 0)
            {

                if (characterController)
                {
                    if (characterController.enabled == false)
                    {
                        characterController.enabled = true;
                    }

                    characterRadius = Mathf.Clamp(characterRadius, 0.02f, 1f);
                    characterController.radius = characterRadius;
                    characterController.height = characterHeight;

                    isGrounded = characterController.isGrounded;

                    if (isGrounded && velocity.y < 0)
                    {
                        velocity.y = 0f;
                    }

                    float x = Input.GetAxis("Horizontal");
                    float z = Input.GetAxis("Vertical");
                    Vector3 moveDirection = transform.right * x + transform.forward * z;


                    //accerlerate if LeftShift is pressed
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        characterController.Move(moveDirection * movementSpeed * speedMultiplier * Time.deltaTime);
                    }
                    else
                    {
                        characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
                    }

                    // changes the height position of the player..
                    if (Input.GetButtonDown("Jump") && isGrounded)
                    {
                        velocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                    }

                    //aligns the Y axis of the player as it moves to the normal of the mesh
                    if (alignToMesh)
                    {
                        RaycastHit hit;
                        Ray downRay = new Ray(transform.position, -Vector3.up);

                        if (Physics.Raycast(downRay, out hit))
                        {
                            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                        }
                    }

                    //bring down the velocity to the ground 
                    velocity.y += gravity * Time.deltaTime;
                    characterController.Move(velocity * Time.deltaTime);

                }
            }

            #endregion


            #region FLYTHROUGH MOVEMENT

            //if flythrough movement
            else
            {
                if (characterController)
                {
                    if (characterController.enabled == true)
                        characterController.enabled = false;
                }

                Vector3 p = GetBaseInput();

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space))
                {
                    p = p * speedMultiplier;
                }
                else
                {
                    p = p * movementSpeed;
                }

                p = p * Time.deltaTime;
                Vector3 newPosition = transform.position;

                transform.Translate(p);

                if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
                {
                    newPosition.y = transform.position.y;
                }
                else
                {
                    newPosition.x = transform.position.x;
                    newPosition.z = transform.position.z;
                }

                transform.position = newPosition;
            }
            #endregion
        }


    }

    private Vector3 GetBaseInput()
    {
        //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(PlayerMovement))]
public class PlayerMovementEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as PlayerMovement;

        myScript.selectedOptionIndex = EditorGUILayout.Popup(myScript.selectedOptionIndex, myScript.movementOptions);
        myScript.movementSpeed = EditorGUILayout.IntSlider("Player Speed", myScript.movementSpeed, 1, 10);
        myScript.speedMultiplier = EditorGUILayout.IntSlider("Speed Multiplier", myScript.speedMultiplier, 1, 10);
        myScript.camSensitivity = EditorGUILayout.FloatField("Camera Sensitivity", myScript.camSensitivity);


        //only show these inputs if character movement is selected
        if (myScript.selectedOptionIndex == 0)
        {
            myScript.gravity = EditorGUILayout.FloatField("Gravity", myScript.gravity);
            myScript.characterRadius = EditorGUILayout.FloatField("Character Radius", myScript.characterRadius);
            myScript.characterHeight = EditorGUILayout.FloatField("Character Height", myScript.characterHeight);
            myScript.alignToMesh = GUILayout.Toggle(myScript.alignToMesh, "Align To Mesh");
        }
    }
}

#endif
