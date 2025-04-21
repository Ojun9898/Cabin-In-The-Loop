using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Animator anim;
    public ParticleSystem ps;

    public Transform shootTf;
    
    private float timer;
    public float shootCooldown = 0.5f;
    
    public List<Transform> targets = new List<Transform>();
    
    public Transform currentTarget;
    public Transform headTf;

    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (currentTarget != null) // 타겟 있음
        {
            headTf.LookAt(currentTarget); // 타겟을 바라보는 기능
            Shoot(); // 총알을 발사하는 기능
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MONSTER"))
        {
            targets.Add(other.transform);

            SetTarget();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MONSTER"))
            SetTarget(other.transform);
    }

    public void SetTarget()
    {
        if (targets.Count > 0)
            currentTarget = targets[0];
    }

    public void SetTarget(Transform prevTarget)
    {
        targets.Remove(prevTarget);
        
        if (targets.Count > 0)
            currentTarget = targets[0];
    }

    private void Shoot()
    {
        timer += Time.deltaTime;
        if (timer >= shootCooldown)
        {
            timer = 0f;
            ps.Play();
            anim.SetTrigger("Shoot");
            CreateBullet();
        }
    }

    private void CreateBullet()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, shootTf.position, shootTf.rotation);

        bulletObj.GetComponent<Rigidbody>().AddForce(shootTf.forward * 50f, ForceMode.Impulse);
    }

}