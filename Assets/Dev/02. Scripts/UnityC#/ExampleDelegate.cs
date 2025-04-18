using UnityEngine;
using UnityEngine.UI;

public class ExampleDelegate : MonoBehaviour
{
    public Button[] buttons;
    
    void Start()
    {
        // buttons[0].onClick.AddListener(() => OnLog(0));
        // buttons[1].onClick.AddListener(() => OnLog(1));
        // buttons[2].onClick.AddListener(() => OnLog(2));
        // buttons[3].onClick.AddListener(() => OnLog(3));
        // buttons[4].onClick.AddListener(() => OnLog(4));

        for (int i = 0; i < 5; i++)
        {
            int k = i;
            buttons[i].onClick.AddListener(() => OnLog(k));
        }
        
    }

    public void OnLog(int index)
    {
        Debug.Log(index);
    }
}