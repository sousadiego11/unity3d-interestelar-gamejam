using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("[ Properties ]")]
    [SerializeField] float viewDistance;
    [SerializeField] float viewAngle;
    [SerializeField] Renderer leftEye;
    [SerializeField] Renderer rightEye;
    [SerializeField] float maxStamina;
    [SerializeField] float stamina;
    [SerializeField] Slider staminaSlider;
    [SerializeField] bool stunned;
    [SerializeField] bool isRegeneratingStamina;
    [SerializeField] LayerMask maskPlayerAndCover;

    [Header("[ Knowledge ]")]
    public bool playerInFov;
    public bool playerDetected;
    public float playerDistance;

    // -- Local --
    private GameObject player;
    private NavMeshAgent navAgent;

    void Start() {
        stamina = maxStamina;
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        CheckStatus();
        CheckPlayerDetection();
        Chase();
    }

    void Chase() {
        if (playerDetected && !stunned) {
            navAgent.SetDestination(player.transform.position);
        }
    }

    void CheckStatus() {
        stunned = stamina <= 0f || isRegeneratingStamina;
        if (stunned && !isRegeneratingStamina) {
            StartCoroutine(RegenerateStamina());
        }
    }

    void CheckPlayerDetection() {
        if (HasPlayerTransform() && HasPlayerInRange()) {
            Vector3 playerDirection = player.transform.position - transform.position;

            float dotProduct = Vector3.Dot(transform.forward, playerDirection.normalized);
            float viewTreshold = Mathf.Cos(viewAngle / 2 * Mathf.Deg2Rad);

            playerDistance = playerDirection.magnitude;
            playerInFov = dotProduct >= viewTreshold;
            playerDetected = playerInFov && Physics.Raycast(transform.position, playerDirection.normalized, out RaycastHit hit, viewDistance, maskPlayerAndCover) && !hit.collider.CompareTag("Cover");

            Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, Color.red);
            Debug.DrawLine(transform.position, transform.position + playerDirection.normalized * viewDistance, Color.green);
        } else {
            playerInFov = false;
            playerDetected = false;
        }

        leftEye.material.color = playerDetected ? Color.red : Color.blue;
        rightEye.material.color = playerDetected ? Color.red : Color.blue;
    }

    bool HasPlayerTransform() {
        return player != null;
    }

    bool HasPlayerInRange() {
        return (player.transform.position - transform.position).magnitude <= viewDistance;
    }

    public void OnHit() {
        stamina = Mathf.Clamp(stamina - Time.deltaTime, 0f, maxStamina);
        staminaSlider.value = stamina;
    }
    
    IEnumerator RegenerateStamina() {
        isRegeneratingStamina = true;
        while (stamina < maxStamina) {
            stamina = Mathf.Clamp(stamina + Time.deltaTime * 0.5f, 0f, maxStamina);
            staminaSlider.value = stamina;
            yield return null;
        }
        isRegeneratingStamina = false;
    }
}
