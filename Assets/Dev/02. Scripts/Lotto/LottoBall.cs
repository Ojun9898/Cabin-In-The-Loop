using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LottoBall : MonoBehaviour
{
    public TMP_Text textNumber;
    public TextMeshProUGUI textUI;
    
    
    
    private bool isScale;

    void Start()
    {
        textNumber = this.transform.GetChild(0).GetComponent<TMP_Text>();

        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isScale)
        {
            this.transform.localScale += Vector3.one * Time.deltaTime * 2f;

            if (this.transform.localScale.x >= 1f)
            {
                isScale = true;
                
                this.transform.localScale = Vector3.one;
            }
        }
    }
    
}