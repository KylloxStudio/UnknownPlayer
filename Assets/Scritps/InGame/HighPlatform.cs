using UnityEngine;

public class HighPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();
            if (rigid.position.y > -3.6f && rigid.position.y < 0.6f)
            {
                if (rigid.velocity.y > 0.3f)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                    }
                }
                else if (rigid.position.y > 0)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();
            if (rigid.position.y > 0.5f)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
                }
            }
            if (rigid.position.y < -0.3f)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();
            if (rigid.velocity.y < 0 && rigid.position.y < 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }
}