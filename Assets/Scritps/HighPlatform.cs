using UnityEngine;

public class HighPlatform : MonoBehaviour
{
    public static bool isLanding;

    // Start is called before the first frame update
    private void Start()
    {
        isLanding = false;
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
            if (rigid.position.y > 0)
            {
                GetComponents<BoxCollider2D>()[0].enabled = true;
            }

            if (rigid.velocity.y > 0)
            {
                GetComponents<BoxCollider2D>()[0].enabled = false;
            }

            if (rigid.velocity.y < 0 && rigid.position.y > 0)
            {
                GetComponents<BoxCollider2D>()[0].enabled = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isLanding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isLanding = false;
        }
    }
}