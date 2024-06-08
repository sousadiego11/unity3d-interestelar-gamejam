using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("[ Shooting ]")]
    [SerializeField] private List<Transform> eyes;
    [SerializeField] private float lazerDistance;
    [SerializeField] private Transform debugPoint;
    [SerializeField] private LayerMask layerMask;

    [Header("[ Dependencies ]")]
    [SerializeField] private CameraFollowController cam;
    [SerializeField] private CharacterController characterController;

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
    [SerializeField] private float velocity = 1f;
    [SerializeField] private float rotationSpeed;

    // -- Local --
    Vector3 inputDirection;

    void Update() {
        CheckInteractions();
        CheckGroundStatus();

        HandleLazerShoot();
        HandleMovement();
    }

    void HandleMovement() {
        Vector3 direction = cam.transform.rotation * inputDirection.normalized;
        direction.y = 0f;

        Vector3 movementMotion = velocity * direction;
        characterController.Move(movementMotion * Time.deltaTime);

        if (isMoving) {
            Quaternion rotationDirection = Quaternion.LookRotation(direction);
            Quaternion rotationOffset = Quaternion.RotateTowards(transform.rotation, rotationDirection, rotationSpeed * Time.deltaTime);
            transform.rotation = rotationOffset;
        }
    }

    void HandleLazerShoot() {
        if (isAiming) {
            Vector3 middleScreenToWorld = cam.thisCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, lazerDistance));
            debugPoint.position = middleScreenToWorld;

            foreach (Transform eye in eyes) {
                if (isShooting) {
                    Vector3 eyeLazerDirection = (middleScreenToWorld - eye.position).normalized;
                    Debug.DrawLine(eye.position, middleScreenToWorld, Color.green);

                    if (Physics.Raycast(eye.position, eyeLazerDirection, out RaycastHit hit, lazerDistance, layerMask)) {
                        if (hit.collider.TryGetComponent(out EnemyController enemyController)) {
                            enemyController.OnHit();
                        }
                    }
                }
            }
        }
    }

    void CheckGroundStatus() {
        Vector3 spherePosition = transform.position + groundedOffset;
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayer, QueryTriggerInteraction.Ignore);
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

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + groundedOffset, groundedRadius);
    }
}