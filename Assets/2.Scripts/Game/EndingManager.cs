using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingManager : Singleton<EndingManager>
{
    [SerializeField] private Altar altar;
    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject deadEndingPrefab;
    [SerializeField] private GameObject aliveEndingPrefab;

    private bool isAlive = true;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "(Bake)Laboratory" && altar == null)
            altar = FindObjectOfType<Altar>();
    
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (isAlive && altar != null && altar.isPlayerInAltar)
        {
            ShowAliveEnding();
        }
    }

    public void UpdateIsAlive(bool alive)
    {
        isAlive = alive;
    }

    public void ShowDeadEnding()
    {
        deadEndingPrefab.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowAliveEnding()
    {
        aliveEndingPrefab.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClickHomeButton()
    {
        PauseManager.Instance.DestroyAllWithTag();
        SceneManager.LoadScene("Main");
    }
}