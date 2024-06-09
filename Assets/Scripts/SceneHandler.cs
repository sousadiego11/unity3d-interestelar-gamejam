using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {

    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas endMenu;

    public static SceneHandler Singleton;
    private void Awake() {
        endMenu.enabled = false;
        if (Singleton == null) {
            Singleton = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (Input.anyKeyDown) {
            mainMenu.enabled = false;
            HideCursor();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void ReloadScene() {
        StartCoroutine(ReloadSceneCoroutine());
    }

    public bool MenuActive() {
        return mainMenu.enabled || endMenu.enabled;
    }

    public void EnableEndMenu() {
        endMenu.enabled = true;
    }

    public void HideCursor() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator ReloadSceneCoroutine() {
        yield return new WaitForSeconds(4f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
