using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WeaponSelectionUI : MonoBehaviour
{
    public GameObject weaponButtonPrefab;
    public Transform weaponButtonParent;
    public float selectionTimeLimit = 10f;
    public TextMeshProUGUI countdownText; 

    private List<WeaponData> _availableWeapons = new();
    private bool _selectionMade;
    private Coroutine _timeoutCoroutine;

    private MonoBehaviour _cameraControl; // 카메라 컨트롤러 캐싱용

    public void Initialize(List<WeaponData> weapons)
    {
        foreach (Transform child in weaponButtonParent)
        {
            Destroy(child.gameObject);
        }

        _availableWeapons = weapons;
        _selectionMade = false;

        foreach (var weapon in weapons)
        {
            GameObject button = Instantiate(weaponButtonPrefab, weaponButtonParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = weapon.displayName;
            button.GetComponent<Button>().onClick.AddListener(() => EquipWeapon(weapon));
        }

        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        DisableCameraControl(); // 카메라 멈춤
        _timeoutCoroutine = StartCoroutine(SelectionTimeout());
    }

    private void EquipWeapon(WeaponData weapon)
    {
        if (_selectionMade) return;
        _selectionMade = true;

        WeaponController controller = FindObjectOfType<WeaponController>();
        if (controller != null)
        {
            controller.EquipWeapon(weapon.weaponType);
        }

        CloseUI();
    }

    private IEnumerator SelectionTimeout()
    {
        float timeLeft = selectionTimeLimit;

        while (timeLeft > 0f)
        {
            countdownText.text = $"선택 시간: {timeLeft:F1}초";
            timeLeft -= Time.unscaledDeltaTime;
            yield return null;
        }

        countdownText.text = "시간 초과! 무기 자동 선택 중...";

        if (!_selectionMade)
        {
            WeaponData randomWeapon = _availableWeapons[Random.Range(0, _availableWeapons.Count)];
            EquipWeapon(randomWeapon);
        }
    }

    private void CloseUI()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        EnableCameraControl(); // 카메라 다시 활성화

        if (_timeoutCoroutine != null)
        {
            StopCoroutine(_timeoutCoroutine);
            _timeoutCoroutine = null;
        }
    }

    private void DisableCameraControl()
    {
        _cameraControl = FindObjectOfType<FirstPersonCamera>(); 
        if (_cameraControl != null)
            _cameraControl.enabled = false;
    }

    private void EnableCameraControl()
    {
        if (_cameraControl != null)
            _cameraControl.enabled = true;
    }
}
