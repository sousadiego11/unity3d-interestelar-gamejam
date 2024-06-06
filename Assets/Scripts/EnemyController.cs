using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] float viewDistance;
    [SerializeField] float viewAngle;
    [SerializeField] Transform playerTransform;
    [SerializeField] Renderer leftEye;
    [SerializeField] Renderer rightEye;
    [Header("Knowledge")]
    public float playerDistance;
    public bool playerInView;
    public Vector3 playerDirection;

    void Update() {
        playerDirection = playerTransform.position - transform.position;

        float dotProduct = Vector3.Dot(transform.forward, playerDirection.normalized);
        float viewTreshold = Mathf.Cos(viewAngle / 2 * Mathf.Deg2Rad);

        playerDistance = playerDirection.magnitude;
        playerInView = dotProduct >= viewTreshold;

        Debug.DrawLine(transform.position, transform.position + transform.forward * viewDistance, Color.red);
        Debug.DrawLine(transform.position, transform.position + playerDirection.normalized * viewDistance, Color.green);

        if (playerInView) {
            leftEye.material.color = Color.red;
            rightEye.material.color = Color.red;
        } else {
            leftEye.material.color = Color.blue;
            rightEye.material.color = Color.blue;
        }
    }
}
