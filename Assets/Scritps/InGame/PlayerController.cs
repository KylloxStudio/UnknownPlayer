using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private Player1 player1;
    [SerializeField]
    private Player2 player2;

    private Rigidbody2D rigid;
    private Animator anim;

    public int health;
    public int stamina;

    public float maxSpeed;
    public float jumpPower;
    public float climbSpeed;
    public bool is1StepJumping;
    public bool is2StepJumping;
    public bool isMoving;
    public bool isAttacking;
    public bool isAttackCanceled;
    public bool isClimbing;
    public bool isCanJump;
    public bool isCanMove;
    public bool isCanClimb;
    public bool isDamaged;
    public bool isDead;
    public bool ignoreDamaged;

    [SerializeField]
    private Vector3 oldPosition;
    [SerializeField]
    private Vector3 curPosition;

    private void Awake()
    {
        Instance = this;

        if (NetworkManager.Instance.isPlayer1)
        {
            Destroy(player2.gameObject.GetComponent<PlayerController>());
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            Destroy(player1.gameObject.GetComponent<PlayerController>());
        }

        SendStatistics();
        SendPosition();
        SendRotation();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        maxSpeed = 8.25f;
        jumpPower = 15f;
        climbSpeed = 7.25f;

        is1StepJumping = false;
        is2StepJumping = false;
        isAttacking = false;
        isAttackCanceled = false;
        isClimbing = false;
        isCanJump = true;
        isCanMove = true;
        isCanClimb = false;
        isDamaged = false;
        isDead = false;

        oldPosition = transform.position;
        curPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Move
        isCanMove = CheckCanMove();
        if (isCanMove)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rigid.AddForce(Vector2.left * 1.1f, ForceMode2D.Impulse);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rigid.AddForce(Vector2.right * 1.1f, ForceMode2D.Impulse);
            }

            if (rigid.velocity.x > maxSpeed) // Right Max Speed
            {
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            }
            else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
            {
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
            }
        }

        // Jump
        isCanJump = CheckCanJump();
        if (isCanJump && Input.GetKeyDown(KeyCode.W))
        {
            if (is2StepJumping)
            {
                return;
            }

            if (is1StepJumping)
            {
                is2StepJumping = true;
            }

            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            is1StepJumping = true;
        }

        // Walking Animation
        if (Mathf.Abs(rigid.velocity.x) > 0.3f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        // Player Direction
        if (!(isDamaged || isDead || anim.GetBool("isAttacking_04") || anim.GetBool("isAttacking_05") || anim.GetBool("isDashing")))
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        SendStatistics();
        SendPosition();
        SendRotation();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rigid.velocity.y < 0)
        {
            if (is1StepJumping)
            {
                is1StepJumping = false;
            }

            if (is2StepJumping)
            {
                is2StepJumping = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            OnClimb();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            OffClimb();
        }
    }

    private bool CheckCanMove()
    {
        if (!(isAttacking || isDamaged || isDead || anim.GetBool("isDashing") || Input.GetKey(KeyCode.LeftShift)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckCanJump()
    {
        if (!(isDamaged || isDead || is2StepJumping || isCanClimb || anim.GetBool("isAttacking_03") || anim.GetBool("isAttacking_05") || anim.GetBool("isDashing")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnClimb()
    {
        if (transform.position.y < 0.55f)
        {
            isCanClimb = true;
        }
        else
        {
            isCanClimb = false;
        }

        if (Input.GetKey(KeyCode.W))
        {
            if (isCanClimb)
            {
                isClimbing = true;
                maxSpeed = 3.5f;
                rigid.drag = 10f;
                rigid.gravityScale = 0;
                GetComponents<BoxCollider2D>()[0].enabled = false;
                transform.Translate(transform.up * climbSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (isCanClimb)
            {
                isClimbing = true;
                maxSpeed = 3.5f;
                rigid.drag = 10f;
                rigid.gravityScale = 0;
                GetComponents<BoxCollider2D>()[0].enabled = false;
                transform.Translate(transform.up * climbSpeed * -1 * Time.deltaTime);
            }
        }
    }

    private void OffClimb()
    {
        isCanClimb = false;
        if (isClimbing)
        {
            isClimbing = false;
            maxSpeed = 8.25f;
            GetComponents<BoxCollider2D>()[0].enabled = true;
            rigid.gravityScale = 5f;
            rigid.drag = 1f;
        }
    }

    public void Death()
    {
        health = 0;
        isDead = true;
        StopAllCoroutines();

        isClimbing = false;
        isAttacking = false;
        isCanClimb = false;

        anim.SetBool("isAttacking_01", false);
        anim.SetBool("isAttacking_02", false);
        anim.SetBool("isAttacking_03", false);
        anim.SetBool("isAttacking_04", false);
        anim.SetBool("isAttacking_05", false);
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", false);
        anim.SetTrigger("doDead");
    }

    private void SendStatistics()
    {
        if (NetworkManager.Instance.isPlayer1)
        {
            health = player1.health;
            stamina = player1.stamina;
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            health = player2.health;
            stamina = player2.stamina;
        }

        JObject data = new JObject();
        data.Add("hp1", player1.health);
        data.Add("stamina1", player1.stamina);
        data.Add("hp2", player2.health);
        data.Add("stamina2", player2.stamina);
        NetworkManager.Instance.socket.Emit("statistics", data.ToString());
    }

    private void SendPosition()
    {
        JObject data = new JObject();
        data.Add("x1", player1.transform.position.x);
        data.Add("y1", player1.transform.position.y);
        data.Add("x2", player2.transform.position.x);
        data.Add("y2", player2.transform.position.y);
        NetworkManager.Instance.socket.Emit("position", data.ToString());
    }

    private void SendRotation()
    {
        JObject data = new JObject();
        data.Add("x1", player1.transform.rotation.x);
        data.Add("y1", player1.transform.rotation.y);
        data.Add("x2", player2.transform.rotation.x);
        data.Add("y2", player2.transform.rotation.y);
        NetworkManager.Instance.socket.Emit("rotation", data.ToString());
    }
}
