using System.Collections;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animation elevatorAnim;
    [SerializeField] private Transform[] floors;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private CharacterController playerController;

    [HideInInspector] public bool isElevatorMoving = false;
    [HideInInspector] public bool isPlayerInElevator = false;
    [HideInInspector] public bool isArrived = false;

    private Vector3 lastElevatorPosition;

    private void Start()
    {
        lastElevatorPosition = rb.position;
<<<<<<< HEAD:Assets/2.Scripts/Game/Map/ElevatorController.cs
        playerTransform = GameManager.Instance.playerTransform;
        playerController = GameManager.Instance.playerTransform.GetComponent<CharacterController>();
=======
>>>>>>> 1f949ed ([추가] 병합 및 씬 분리):Assets/2.Scripts/Game/ElevatorController.cs
    }

    private void Update()
    {
        if (isPlayerInElevator)
        {
            Vector3 delta = rb.position - lastElevatorPosition;
            if (delta != Vector3.zero)
            {
                playerController.Move(delta);
            }
        }

        lastElevatorPosition = rb.position;
    }


    public void OpenDoor()
    {
        elevatorAnim.Play("ElevatorOpen");
    }

    public void CloseDoor()
    {
        elevatorAnim.Play("ElevatorClose");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInElevator = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInElevator = false;
        }
    }

    public IEnumerator MoveElevator(int currentFloor)
    {
        if (isElevatorMoving || currentFloor >= floors.Length) yield break;

        isElevatorMoving = true;

        Vector3 start = rb.position;
        Vector3 end = floors[currentFloor].position;
        float duration = 2.0f;
        float elapsed = 0f;

        audioSource.Play();

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(start, end, elapsed / duration);
            rb.MovePosition(newPosition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.Stop();
        rb.MovePosition(end);

        isElevatorMoving = false;
        isArrived = true;
    }
}