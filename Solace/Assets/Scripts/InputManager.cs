using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class InputManager : MonoBehaviour
{
    PlayerMovement movementScript;
    [System.Serializable]
    public class InputSettings
    {
        public string forwardInput = "Vertical";
        public string sideInput = "Horizontal";
        public string sprintInput = "Sprint";
    }
    [SerializeField]
    public InputSettings input;


    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
    }


    void Update()
    {
        movementScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.sideInput));
        movementScript.Sprint(Input.GetButton(input.sprintInput));
    }
}
