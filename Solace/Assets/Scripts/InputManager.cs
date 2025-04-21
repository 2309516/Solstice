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


    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;
    Ray ray;


    [Header("Spine Settings")]
    public Transform spine;
    public Vector3 spineOffest;
    
    [Header("Head Rotation Settings")]
    public float LookAtPoint = 2.8f;

    Transform mainCamera;

    public BowController bowScript;
    bool isAiming;

    public bool testAim;

    void Start()
    {
        movementScript = GetComponent<PlayerMovement>();
        mainCamera = Camera.main.transform;
    }


    void Update()
    {
        
        isAiming = Input.GetButton(input.aimInput);

        if (testAim)
        {
            isAiming = true;
        }

        movementScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.sideInput));
        movementScript.Sprint(Input.GetButton(input.sprintInput));
        movementScript.CharacterAim(isAiming);

        if (isAiming)
        {   
            Aim();
            movementScript.CharacterPullString(Input.GetButton(input.fire));
        }
        else
        {
            bowScript.removeCrosshair();
        }
    }

    private void LateUpdate()
    {
        if (isAiming)
        {
            RotateCharacterSpine();
        }
    }

    private void Aim()
    {
        Vector3 camPosition = mainCamera.position;
        Vector3 direction = mainCamera.forward;

        ray = new Ray(camPosition, direction);

        if(Physics.Raycast(ray, out hit, 500f, aimLayers))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            bowScript.ShowCrosshair(hit.point);
        }
        else
        {
            bowScript.removeCrosshair();
        }
    }

    private void RotateCharacterSpine()
    {
        spine.LookAt(ray.GetPoint(50));
        spine.Rotate(spineOffest);
    }

    public void Pull()
    {
        bowScript.PullString();

    }

    public void EnableArrow()
    {
        bowScript.PickArrow();

    }

    public void Disable()
    {
        bowScript.DisableArrow();
    }

    public void Release()
    {
        bowScript.Release();
    }
}
