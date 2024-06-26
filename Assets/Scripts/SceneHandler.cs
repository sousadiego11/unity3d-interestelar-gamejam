using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AGOUtils;

public class SceneHandler : ExtensibleSingleton<SceneHandler> {

    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas endMenu;

    void Start() {
        endMenu.enabled = false;
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
