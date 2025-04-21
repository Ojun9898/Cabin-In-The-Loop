using UnityEngine;
using UnityEngine.UI;

public class BoardMatrix : MonoBehaviour
{
    public GameObject tilePrefab;

    public Vector2 boardSize = new Vector2(5, 5);

    public int[,] tileArray;

    public GameObject turretPrefab;
    public GameObject[] turrets;

    public Button[] buttons;

    void Awake()
    {
        buttons[0].onClick.AddListener(() => OnChangeTurret(0));
        buttons[1].onClick.AddListener(() => OnChangeTurret(1));
        buttons[2].onClick.AddListener(() => OnChangeTurret(2));
        buttons[3].onClick.AddListener(() => OnChangeTurret(3));
        buttons[4].onClick.AddListener(() => OnChangeTurret(4));
    }
    
    void Start()
    {
        tileArray = new int[(int)boardSize.x, (int)boardSize.y];
        
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int z = 0; z < boardSize.y; z++)
            {
                GameObject tileObj = Instantiate(tilePrefab);
                tileObj.transform.position = new Vector3(x, 0, z);

                // tileArray[x, z] = 1;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                int x = Mathf.RoundToInt(hit.collider.transform.position.x);
                int z = Mathf.RoundToInt(hit.collider.transform.position.z);

                if (tileArray[x, z] == 0)
                {
                    GameObject turretObj = Instantiate(turretPrefab);
                    turretObj.transform.position = new Vector3(x, 0, z);

                    tileArray[x, z] = 1;
                }
            }
        }
    }

    public void OnChangeTurret(int index)
    {
        turretPrefab = turrets[index];
    }
}















