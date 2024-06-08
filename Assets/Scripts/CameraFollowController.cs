using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("[ Properties ]")]
    [SerializeField] PlayerController player;
    [SerializeField] float zoomSpeed;

    [Header("[ Camera View ]")]
    [SerializeField] Vector3 camOffset;
    [SerializeField] Vector3 camAimOffset;
    [SerializeField] Vector3 camRunOffset;

    // -- Local --
    float pitchPos;
    float yawPos;
    Vector3 offset;
    [HideInInspector] public Camera thisCamera;

    void Start() {
        thisCamera = GetComponent<Camera>();
    }

    void Update() {
        pitchPos += Input.GetAxis("Mouse Y") * -1;
        pitchPos = Mathf.Clamp(pitchPos, -20f, 50f);
        yawPos += Input.GetAxis("Mouse X");

        HandleOffset();
        Quaternion rotationMatrix = Quaternion.Euler(pitchPos, yawPos, 0f);
        Vector3 newPositionOffset = Vector3.forward * offset.z + Vector3.up * offset.y + Vector3.right * offset.x;
        Vector3 newPosition = player.transform.position - rotationMatrix * newPositionOffset;

        transform.SetPositionAndRotation(newPosition, rotationMatrix);
    }

    void HandleOffset() {
        offset = Vector3.Lerp(offset, player.isAiming ? camAimOffset : camOffset, Time.deltaTime * zoomSpeed);
    }
}