using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel;
    private bool isPaused = false;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        if (settingPanel != null)
            settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (settingPanel != null)
            settingPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (settingPanel != null)
            settingPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnClickCloseButton()
    {
        if (isPaused)
            ResumeGame();
    }

    public void OnClickExitButton()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.enabled = false;
            cam.GetComponent<AudioListener>().enabled = false;
        }
        
        SceneManager.LoadScene("Main");
    }
}
