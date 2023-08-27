using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyBullet", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.y == 0)
        {
            transform.Translate(transform.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(transform.right * -1 * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DestroyBullet();
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().OnDamaged(60, 1.7f, 0, transform.position);
        }
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
