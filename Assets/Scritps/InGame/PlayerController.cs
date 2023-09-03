using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField]
    private Player1 player1;
    [SerializeField]
    private Player2 player2;

    private Rigidbody2D rigid;
    private Animator anim;
    private GameCamera gameCamera;

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

    private Coroutine runningCoroutine;

    [SerializeField]
    private Vector2 oldPosition;
    [SerializeField]
    private Vector2 curPosition;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (NetworkManager.Instance.isPlayer1)
        {
            Destroy(player2.gameObject.GetComponent<PlayerController>());
            player1.gameObject.GetComponent<PlayerController>().gameObject.SetActive(true);
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            Destroy(player1.gameObject.GetComponent<PlayerController>());
            player2.gameObject.GetComponent<PlayerController>().gameObject.SetActive(true);
        }

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameCamera = GameManager.GetCamera();

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

        runningCoroutine = null;

        oldPosition = transform.position;
        curPosition = transform.position;
    }

    private void FixedUpdate()
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
    }

    // Update is called once per frame
    void Update()
    {
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

        SendPosition();
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

        gameCamera.VibrateForTime(0.3f, 0.4f);
    }

    private void SendPosition()
    {

    }
}
