using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private GameObject settingPanel;
    private bool isPaused = false;
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
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
        isPaused = true;
        if (settingPanel != null)
            settingPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
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
        DestroyAllWithTag();
        SceneManager.LoadScene("Main");
    }

    public void DestroyAllWithTag()
    {
        string tag = "DontDestroy";
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
}
