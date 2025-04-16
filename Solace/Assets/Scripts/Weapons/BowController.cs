using UnityEngine;
using System.Collections.Generic;

public class BowController : MonoBehaviour
{
    [Header("References")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public Camera mainCamera;
    public Transform playerTransform;

    [Header("Settings")]
    public float arrowForce = 25f;
    public float zoomFOV = 60f;
    public float normalFOV = 90f;
    public float zoomSpeed = 5f;
    public float slowMotionTimeScale = 0.3f;
    public float maxSlowdownDuration = 2f;
    public float cooldownTime = 5f;

    private bool isDrawing = false;
    private bool isZooming = false;
    private float originalTimeScale;
    private float slowDownTimer = 0f;
    private float cooldownTimer = 0f;
    private bool canActivateSlowMotion = true;

    private List<GameObject> arrows = new List<GameObject>();
    private int maxArrows = 10;

    [System.Serializable]
    public class BowSettings
    {
        [Header("ArrowSettings")]
        public float arrowCount;
        public GameObject arrowPrefab;
        public Transform arrowPosition;

        [Header("Equip & Unequip Settings")]
        public Transform EquipPos;
        public Transform UnequipPos;

        public Transform UnequipParent;
        public Transform EquipParent;

        [Header("Bow String Settings")]
        public Transform bowString;
        public Transform stringInitialPos;
        public Transform stringPullPos;
        public Transform stringInitialParent;
    }
    [SerializeField]
    public BowSettings bowSettings;

        [Header("Crosshair Settings")]
        public GameObject crosshairPrefab;
        public GameObject currentCrosshair;

        bool canPullString = false;
        bool canFireArrow = false;


    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        originalTimeScale = Time.timeScale;
    }

    private void Update()
    {
        HandleZoom();
        HandleBowDrawAndShoot();

        HandleSlowMotionCooldown();

        if (arrows.Count > maxArrows)
        {
            DestroyOldestArrow();
        }
    }

    void equipBow()
    {
        this.transform.position = bowSettings.EquipPos.position;
        this.transform.rotation = bowSettings.EquipPos.rotation;
        this.transform.parent = bowSettings.EquipParent;

    }

    void unequipBow()
    {
        this.transform.position = bowSettings.UnequipPos.position;
        this.transform.rotation = bowSettings.UnequipPos.rotation;
        this.transform.parent = bowSettings.UnequipParent;
        
    }

    public void ShowCrosshair(Vector3 crosshairPos)
    {
        if (!currentCrosshair)
        {
            currentCrosshair = Instantiate(crosshairPrefab) as GameObject;
        }
        currentCrosshair.transform.position = crosshairPos;
        currentCrosshair.transform.LookAt(Camera.main.transform);
    }

    public void removeCrosshair()
    {
        if (currentCrosshair)
        {
            Destroy(currentCrosshair);
        }
    }
    void HandleBowDrawAndShoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            ShootArrow();
            isDrawing = false;
        }
    }


    void HandleZoom()
    {
        bool isRightClick = Input.GetMouseButton(1);
        bool isInAir = !IsGrounded();

        if (isRightClick)
        {
            isZooming = true;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);

            if (isInAir && canActivateSlowMotion && Time.timeScale == 1f)
            {
                Time.timeScale = slowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                slowDownTimer = maxSlowdownDuration;
                canActivateSlowMotion = false;
            }
        }
        else
        {
            isZooming = false;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);


            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }

    void HandleSlowMotionCooldown()
    {

        if (slowDownTimer > 0f)
        {
            slowDownTimer -= Time.unscaledDeltaTime;

            if (slowDownTimer <= 0f)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }

        if (!canActivateSlowMotion)
        {
            cooldownTimer -= Time.unscaledDeltaTime;

            if (cooldownTimer <= 0f)
            {
                canActivateSlowMotion = true;
                cooldownTimer = cooldownTime;
            }
        }
    }

    void ShootArrow()
    {
        Vector3 shootDirection = mainCamera.transform.forward;
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.velocity = shootDirection * arrowForce;

        arrows.Add(arrow);
    }

    void DestroyOldestArrow()
    {
        if (arrows.Count > 0)
        {
            GameObject oldestArrow = arrows[0];
            arrows.RemoveAt(0);
            Destroy(oldestArrow);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(playerTransform.position, Vector3.down, 1.2f, LayerMask.GetMask("Ground"));
    }
}
