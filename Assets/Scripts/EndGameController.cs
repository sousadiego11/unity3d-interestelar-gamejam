using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameController : MonoBehaviour
{
    void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out PlayerController _) && SceneHandler.Singleton != null) {
            SceneHandler.Singleton.EnableEndMenu();
        };
    }
}
