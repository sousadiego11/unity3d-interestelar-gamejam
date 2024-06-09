using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [Header("[ Shooting ]")]
    [SerializeField] private float lazerDistance;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Material lazerMaterial;
    [SerializeField] private LayerMask layerMask;

    [Header("[ Dependencies ]")]
    [SerializeField] private CameraFollowController cam;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Canvas canvas;
    [SerializeField] private RawImage crosshair;
    [SerializeField] private GameObject puppet;

    [Header("[ Ground Check ]")]
    [SerializeField] private float groundedRadius;
    [SerializeField] private Vector3 groundedOffset;
    [SerializeField] private LayerMask groundLayer;


    [Header("[ State ]")]
    [SerializeField] bool isMoving;
    [SerializeField] public bool isRunning;
    [SerializeField] public bool isAiming;
    [SerializeField] public bool isShooting;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float velocity = 4f;
    [SerializeField] private float fallVelocity = 2f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float rotationSpeed;

    // -- Local --
    Vector3 inputDirection;
    float verticalVelocity;

    void Start() {
        LineRenderer lr = shootPoint.gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.material = lazerMaterial;
        lr.enabled = false;
    }

    void Update() {
        canvas.enabled = !SceneHandler.Singleton.MenuActive();
        if (SceneHandler.Singleton != null && !SceneHandler.Singleton.MenuActive()) {
            CheckInteractions();
            CheckGroundStatus();
            CheckHealth();

            HandleLazerShoot();
            HandleVerticalVelocity();
            HandleMovement();
        }
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * inputDirection.normalized;
        direction.y = 0f;

        Vector3 movementMotion = velocity * direction;
        movementMotion.y = verticalVelocity;
        characterController.Move(movementMotion * Time.deltaTime);

        if (isMoving) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }
    }

    void HandleVerticalVelocity() {
        if (isGrounded && verticalVelocity < 0) {
            verticalVelocity = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            verticalVelocity += Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime * fallVelocity;
    }

    void HandleLazerShoot() {
        LineRenderer lineRenderer = shootPoint.GetComponent<LineRenderer>();
        crosshair.enabled = isAiming;

        if (!isAiming || !isShooting) lineRenderer.enabled = false;
        if (isAiming && isShooting) {
            Vector3 middleScreenToWorld = cam.thisCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, lazerDistance));
            Vector3 direction = middleScreenToWorld - shootPoint.position;

            lineRenderer.SetPosition(0, shootPoint.position);
            lineRenderer.SetPosition(1, middleScreenToWorld);
            lineRenderer.enabled = true;

            if (Physics.Raycast(shootPoint.position, direction.normalized, out RaycastHit hit, lazerDistance, layerMask)) {
                lineRenderer.SetPosition(1, hit.point);
                if (hit.collider.TryGetComponent(out EnemyController enemyController)) enemyController.OnHit();
            }
        }
    }

    void CheckGroundStatus() {
        Vector3 spherePosition = transform.position + groundedOffset;
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer);
    }

    void CheckInteractions() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        inputDirection = new(x, 0f, z);
        isMoving = Mathf.Abs(inputDirection.magnitude) > 0f;
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isAiming = Input.GetKey(KeyCode.Mouse1);
        isShooting = Input.GetKey(KeyCode.Mouse0);
    }

    void CheckHealth() {
        if (healthBar.value <= healthBar.minValue) {
            SceneHandler.Singleton.ReloadScene();
            Instantiate(puppet, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + groundedOffset, groundedRadius);
    }

    public void OnHit(float damage) {
        healthBar.value -= Time.deltaTime * damage;
    }
}