using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float speed = 1f;
    
    void Update()
    {
        this.transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}