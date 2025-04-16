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
        public string aimInput = "Fire2";
        public string fire = "Fire1";

    }
    [SerializeField]


    public InputSettings input;

    bool isAiming;

    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
    }


    void Update()
    {
        isAiming = Input.GetButton(input.aimInput);

        movementScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.sideInput));
        movementScript.Sprint(Input.GetButton(input.sprintInput));
        movementScript.CharacterAim(isAiming);

        if (isAiming)
        {   
            movementScript.CharacterPullString(Input.GetButton(input.fire));
        }
    }
}
