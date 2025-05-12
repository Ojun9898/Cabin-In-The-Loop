using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animation elevatorAnim;
    [SerializeField] private Transform[] floors;

    [HideInInspector] public bool isElevatorMoving = false;
    [HideInInspector] public bool isPlayerInElevator = false;
    [HideInInspector] public bool isArrived = false;

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
            other.transform.SetParent(transform);
            isPlayerInElevator = true;
            playerRb.isKinematic = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            isPlayerInElevator = false;
            playerRb.isKinematic = false;
        }
    }

    public IEnumerator MoveElevator(int currentFloor)
    {
        if (isElevatorMoving) yield break;
        if (currentFloor >= floors.Length) yield break;
        
        isElevatorMoving = true;

        Vector3 start = rb.position;
        Vector3 end = floors[currentFloor].position;
        float duration = 2.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 newPosition = Vector3.Lerp(start, end, elapsed / duration);
            rb.MovePosition(newPosition);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(end);
        isElevatorMoving = false;
        isArrived = true;
    }

}