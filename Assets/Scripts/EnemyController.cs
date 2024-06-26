using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : Hackeable
{
    [Header("[ Properties ]")]
    [SerializeField] float viewDistance;
    [SerializeField] float viewAngle;
    [SerializeField] float patrolRadius;
    [SerializeField] float shootReloadTime;
    [SerializeField] Renderer leftEye;
    [SerializeField] Renderer rightEye;
    [SerializeField] LayerMask maskPlayerAndCover;
    [SerializeField] float coverLayer;
    [SerializeField] ProjectileController projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField]  Animator animator;
    [SerializeField]  Light chestLight;

    [Header("[ Knowledge ]")]
    public bool playerInFov;
    public bool playerDetected;
    public float playerDistance;
    public bool isPatrolling;
    public bool isReloadingShoot;

    // -- Local --
    private GameObject player;
    private NavMeshAgent navAgent;

    void Start() {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        CheckStatus();
        CheckStunned();
        CheckPlayerDetection();
        CheckColors();
        
        Chase();
        Patrol();
    }

    void Chase() {
        if (playerDetected && !isDisabled) {
            isPatrolling = false;
            navAgent.SetDestination(player.transform.position);
            SoundBoard.Singleton.BlendBetween(Audio.AudioEnum.LonelinessSFX, Audio.AudioEnum.WarningSFX, 2f);
            Attack();
        } else {
            SoundBoard.Singleton.BlendBetween(Audio.AudioEnum.WarningSFX, Audio.AudioEnum.LonelinessSFX, 5f);
        }
    }

    void Attack() {
        if (!isReloadingShoot) {
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            Quaternion projectileRotation = Quaternion.LookRotation(playerDirection);
            SoundBoard.Singleton.PlayOneShot(Audio.AudioEnum.EnemyShootSFX);

            Instantiate(projectilePrefab, shootPoint.position, projectileRotation);
            StartCoroutine(ReloadShoot());
        }
    }

    void Patrol() {
        if (!isPatrolling && !playerDetected || isDisabled) navAgent.ResetPath();
        if (!playerDetected && !isDisabled && (!navAgent.hasPath || navAgent.remainingDistance <= navAgent.stoppingDistance)) {
            isPatrolling = true;
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * patrolRadius + transform.position;

            NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, 1);
            navAgent.SetDestination(hit.position);
        }
    }

    void CheckStunned() {
        animator.SetBool("isStunned", isDisabled);
        chestLight.enabled = !isDisabled;
    }

    void CheckPlayerDetection() {
        if (HasPlayerTransform() && HasPlayerInRange()) {
            Vector3 playerDirection = player.transform.position - transform.position;

            float dotProduct = Vector3.Dot(transform.forward, playerDirection.normalized);
            float viewTreshold = Mathf.Cos(viewAngle / 2 * Mathf.Deg2Rad);

            playerDistance = playerDirection.magnitude;
            playerInFov = dotProduct >= viewTreshold;
            playerDetected = playerInFov && Physics.Raycast(transform.position, playerDirection.normalized, out RaycastHit hit, viewDistance, maskPlayerAndCover) && hit.collider.gameObject.layer != coverLayer;

            Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, Color.red);
            Debug.DrawLine(transform.position, transform.position + playerDirection.normalized * viewDistance, Color.green);
        } else {
            playerInFov = false;
            playerDetected = false;
        }
    }

    void CheckColors() {
        Color targetColor = isDisabled ? Color.yellow : playerDetected ? Color.red : Color.blue;
        
        leftEye.material.color = targetColor;
        leftEye.material.EnableKeyword("_EMISSION");
        leftEye.material.SetColor("_EmissionColor", targetColor);

        rightEye.material.color = targetColor;
        rightEye.material.EnableKeyword("_EMISSION");
        rightEye.material.SetColor("_EmissionColor", targetColor);
    }

    bool HasPlayerTransform() {
        return player != null;
    }

    bool HasPlayerInRange() {
        return (player.transform.position - transform.position).magnitude <= viewDistance;
    }
    
    IEnumerator ReloadShoot() {
        isReloadingShoot = true;
        yield return new WaitForSeconds(shootReloadTime);
        isReloadingShoot = false;
    }
}
