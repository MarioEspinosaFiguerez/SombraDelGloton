using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoterTrap : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireInterval = 2f;
    public float shootSpeed = 10f;
    public float timer = 0f;
    public string playerTag = "Player";
    public float detectionRange = 10f;
    public bool requirePlayerDetection = false;
    private Transform playerTransform;
    private float fieldOfView = 45f;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if(player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        bool shouldShoot = false;

        if(!requirePlayerDetection)
        {
            shouldShoot = true;
        }
        else if(playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float distance = directionToPlayer.magnitude;
            if(distance <= detectionRange)
            {
                float angle = Vector3.Angle(new Vector3(- transform.up.z, 0, 0), directionToPlayer.normalized);
                if(angle <= (fieldOfView * 2f))
                {
                    shouldShoot = true;
                }
            }
        }

        if(shouldShoot)
        {
            timer += Time.deltaTime;
            if(timer >= fireInterval)
            {
                Shoot();
                timer = 0f;
            }
        }
    }

    private void Shoot()
    {
        if(bulletPrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position,shootPoint.rotation);
           
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            bulletScript.SetDirection(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        if(requirePlayerDetection)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Vector3 leftLimit = Quaternion.Euler(- transform.up.z, 0, (-fieldOfView / 2f) - 90) * transform.forward;
            Vector3 rightLimit = Quaternion.Euler(- transform.up.z, 0, (fieldOfView / 2f) - 90) * transform.forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, - transform.up.z * leftLimit * detectionRange);
            Gizmos.DrawRay(transform.position, - transform.up.z * rightLimit * detectionRange);
        }        
    }

}
