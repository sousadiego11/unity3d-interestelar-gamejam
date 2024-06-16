using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("[ Properties ]")]
    [SerializeField] PlayerController player;

    [Header("[ Camera View ]")]
    [SerializeField] Vector3 camOffset;

    // -- Local --
    float pitchPos;
    float yawPos;
    [HideInInspector] public Camera thisCamera;

    void Start() {
        thisCamera = GetComponent<Camera>();
    }

    void Update() {
        if (player != null && SceneHandler.Singleton != null && !SceneHandler.Singleton.MenuActive()) {
            pitchPos += Input.GetAxis("Mouse Y") * -1;
            pitchPos = Mathf.Clamp(pitchPos, -20f, 50f);
            yawPos += Input.GetAxis("Mouse X");

            Quaternion rotationMatrix = Quaternion.Euler(pitchPos, yawPos, 0f);
            Vector3 newPositionOffset = Vector3.forward * camOffset.z + Vector3.up * camOffset.y + Vector3.right * camOffset.x;
            Vector3 newPosition = player.transform.position - rotationMatrix * newPositionOffset;

            transform.SetPositionAndRotation(newPosition, rotationMatrix);
        }
    }

    public void ExecShake() {
        StartCoroutine(Shake());
    }

    IEnumerator Shake() {
        Vector3 originalPosition = transform.localPosition;
        float duration = 0.1f;
        float elapsed = 0f;
        
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float range = 0.04f;
            float x = UnityEngine.Random.Range(-range, range);
            float y = UnityEngine.Random.Range(-range, range);

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}