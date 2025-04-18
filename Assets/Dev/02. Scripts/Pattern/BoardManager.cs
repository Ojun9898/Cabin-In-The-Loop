using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public GameObject tilePrefab; // 타일 프리팹
    public Vector2Int boardSize = new Vector2Int(5, 5); // 보드 사이즈
    public int[,] tileArray; // 보드 배열

    public GameObject[] turrets; // 생성될 터렛
    public GameObject[] prevTurrets; // 미리보기 터렛
    
    public GameObject currentTurret;
    private int turretIndex;
    private bool isSelected;
    
    public Button[] selectButtons;

    void Start()
    {
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int j = i;
            selectButtons[i].onClick.AddListener(() => OnSelectTurret(j));
        }
    }
    
    public void CreateBoard()
    {
        tileArray = new int[boardSize.x, boardSize.y];
        
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int z = 0; z < boardSize.y; z++)
            {
                GameObject tile = Instantiate(tilePrefab, this.transform);
                
                tile.transform.position = new Vector3(x, 0, z);
            }
        }
    }

    public void RayToBoard()
    {
        if (!isSelected) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = Mathf.RoundToInt(hit.collider.transform.position.x);
            int z = Mathf.RoundToInt(hit.collider.transform.position.z);

            if (tileArray[x, z] == 0)
                currentTurret.transform.position = new Vector3(x, 0, z);

            if (Input.GetMouseButtonDown(0))
                CreateTurret(turrets[turretIndex], x, z);
        }
    }
    
    private void CreateTurret(GameObject turretPrefab, int x, int z)
    {
        isSelected = false;
        Destroy(currentTurret);

        if (tileArray[x, z] == 0)
        {
            currentTurret = Instantiate(turretPrefab, this.transform);
            currentTurret.transform.position = new Vector3(x, 0, z);

            tileArray[x, z] = 1;
        }
    }
    

    private void OnSelectTurret(int index)
    {
        isSelected = true;
        turretIndex = index;
        currentTurret = prevTurrets[index];
        currentTurret = Instantiate(currentTurret, this.transform);
    }
    
    public void OnChangeTurret(int index)
    {
        currentTurret = turrets[index];
    }
}