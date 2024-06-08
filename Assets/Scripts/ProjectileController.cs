using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ProjectileController : MonoBehaviour {
    [SerializeField] float velocity = 5f;
    [SerializeField] float lifeTime = 5f;

    void Start() {
        StartCoroutine(DestroyAfterTime());    
    }

    void Update() {
        transform.Translate(0f, 0f, Time.deltaTime * velocity);
    }

    void OnTriggerEnter(Collider _) {
        Destroy(gameObject);
    }

    IEnumerator DestroyAfterTime() {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}