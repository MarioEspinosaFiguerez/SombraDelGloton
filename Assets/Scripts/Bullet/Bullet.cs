using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletDamage;

    // -1 -> Left / 1 -> Right
    private float direction;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBullet();
    }

    public void SetDirection(bool directionRight)
    {
        this.direction = directionRight ? 1 : -1;
    }

    private void MoveBullet() => transform.Translate(Vector3.right * direction * bulletSpeed * Time.deltaTime);

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("COLLIDER BULLET");
        if (collider.CompareTag("Enemy"))
        {
            Transform parentTransform = collider.transform.parent;
            EnemyHajime enemyHealth = parentTransform.GetComponent<EnemyHajime>();

            enemyHealth.UpdateLife(-bulletDamage);
        }
        if (collider.CompareTag("Akechi"))
        {
            Boss akechiScript = collider.transform.GetComponent<Boss>();
            akechiScript.UpdateLife(-bulletDamage, transform.position);
        }
        if (collider.CompareTag("Player"))
        {
            PlayerLife player = collider.GetComponent<PlayerLife>() ?? collider.GetComponentInParent<PlayerLife>();
            if (player != null)
            {
                player.ChangeCurrentLife(-bulletDamage);
            }
            else
            {
                Debug.LogWarning("No encontrado el componente PlayerLife");
            }
        }
        Destroy(gameObject);
    }
}