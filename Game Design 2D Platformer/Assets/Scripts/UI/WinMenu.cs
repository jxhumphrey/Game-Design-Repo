using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
  [SerializeField] AudioClip pressSound;
  [SerializeField] AudioClip openSound;
  [SerializeField] GameObject winMenu;

    void OnEnable() {
        Cursor.visible = true;
        GameManager.Instance.audioSource.PlayOneShot(openSound);
        Time.timeScale = 0f;
    }

    public void Quit() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
