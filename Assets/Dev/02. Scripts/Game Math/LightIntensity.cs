using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{
    private Light light;
    private float theta;
    public float speed = 1f;
    public float power = 1f;
    
    void Start()
    {
        light = this.GetComponent<Light>();
    }

    void Update()
    {
        theta += Time.deltaTime * speed;

        // light.intensity = Mathf.Abs(7 * Mathf.Sin(7 * theta / 10) + 3 * Mathf.Sin(3 * theta / 2) + 5 * Mathf.Cos(5 * theta / 3));
        
        light.intensity = Mathf.PerlinNoise(theta, 0f) * power;
    }
}