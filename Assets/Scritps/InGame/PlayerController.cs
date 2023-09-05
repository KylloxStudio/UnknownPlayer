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
    public bool isCanJump;
    public bool isCanMove;
    public bool isCanClimb;
    public bool isClimbing;

    public bool isAttacking;
    public bool isAttackCanceled;
    public bool isDamaged;
    public bool isDead;
    public bool ignoreDamaged;

    private bool isPlayerDataDiff = false;

    private void Awake()
    {
        Instance = this;

        NetworkManager.Instance.player1Hp = player1.health;
        NetworkManager.Instance.player1Stamina = player1.stamina;
        NetworkManager.Instance.player1Position = player1.transform.position;
        NetworkManager.Instance.player1Rotation = player1.transform.rotation;

        NetworkManager.Instance.player2Hp = player2.health;
        NetworkManager.Instance.player2Stamina = player2.stamina;
        NetworkManager.Instance.player2Position = player2.transform.position;
        NetworkManager.Instance.player2Rotation = player2.transform.rotation;

        if (NetworkManager.Instance.isPlayer1)
        {
            health = player1.health;
            stamina = player1.stamina;
            Destroy(player2.gameObject.GetComponent<PlayerController>());
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            health = player2.health;
            stamina = player2.stamina;
            Destroy(player1.gameObject.GetComponent<PlayerController>());
        }
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
        isCanJump = true;
        isCanMove = true;
        isCanClimb = false;
        isClimbing = false;

        GetStatistics();

        SendStatistics();
        SendPosition();
        SendRotation();
    }

    // Update is called once per frame
    void Update()
    {
        GetStatistics();

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

        if (isPlayerDataDiff)
        {
            SendStatistics();
            SendPosition();
            SendRotation();
            SendAnimation();
        }
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

    private void GetStatistics()
    {
        if (NetworkManager.Instance.isPlayer1)
        {
            health = player1.health;
            stamina = player1.stamina;
            isAttacking = player1.isAttacking;
            isAttackCanceled = player1.isAttackCanceled;
            isDamaged = player1.isDamaged;
            isDead = player1.isDead;

            foreach (KeyValuePair<string, bool> item in NetworkManager.Instance.player1Animations)
            {
                if (player1.anim.GetBool(item.Key) != item.Value)
                {
                    isPlayerDataDiff = true;
                    break;
                }
            }

            if (!isPlayerDataDiff)
            {
                if (NetworkManager.Instance.player1Hp != player1.health || NetworkManager.Instance.player1Stamina != player1.stamina || NetworkManager.Instance.player1Position != player1.transform.position || NetworkManager.Instance.player1Rotation != player1.transform.rotation)
                {
                    isPlayerDataDiff = true;
                }
                else
                {
                    isPlayerDataDiff = false;
                }
            }
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            health = player2.health;
            stamina = player2.stamina;
            isAttacking = player2.isAttacking;
            isAttackCanceled = player2.isAttackCanceled;
            isDamaged = player2.isDamaged;
            isDead = player2.isDead;

            foreach (KeyValuePair<string, bool> item in NetworkManager.Instance.player2Animations)
            {
                if (player2.anim.GetBool(item.Key) != item.Value)
                {
                    isPlayerDataDiff = true;
                    break;
                }
            }

            if (!isPlayerDataDiff)
            {
                if (NetworkManager.Instance.player2Hp != player2.health || NetworkManager.Instance.player2Stamina != player2.stamina || NetworkManager.Instance.player2Position != player2.transform.position || NetworkManager.Instance.player2Rotation != player2.transform.rotation)
                {
                    isPlayerDataDiff = true;
                }
                else
                {
                    isPlayerDataDiff = false;
                }
            }
        }
    }

    private void SendStatistics()
    {
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
        data.Add("y1", player1.transform.rotation.y != 0 ? 180 : 0);
        data.Add("y2", player2.transform.rotation.y != 0 ? 180 : 0);
        NetworkManager.Instance.socket.Emit("rotation", data.ToString());
    }

    private void SendAnimation()
    {
        JObject data = new JObject();
        data.Add("p1_isMoving", player1.anim.GetBool("isMoving"));
        data.Add("p1_isDashing", player1.anim.GetBool("isDashing"));
        data.Add("p1_isAttacking_01", player1.anim.GetBool("isAttacking_01"));
        data.Add("p1_isAttacking_02", player1.anim.GetBool("isAttacking_02"));
        data.Add("p1_isAttacking_03", player1.anim.GetBool("isAttacking_03"));
        data.Add("p1_isAttacking_04", player1.anim.GetBool("isAttacking_04"));
        data.Add("p1_isAttacking_05", player1.anim.GetBool("isAttacking_05"));
        data.Add("p1_isDamaged", player1.anim.GetBool("isDamaged"));
        data.Add("p1_isDead", player1.anim.GetBool("isDead"));

        data.Add("p2_isMoving", player2.anim.GetBool("isMoving"));
        data.Add("p2_isDashing", player2.anim.GetBool("isDashing"));
        data.Add("p2_isAttacking", player2.anim.GetBool("isAttacking"));
        data.Add("p2_isDamaged", player2.anim.GetBool("isDamaged"));
        data.Add("p2_isDead", player2.anim.GetBool("isDead"));

        NetworkManager.Instance.socket.Emit("animation", data.ToString());
    }
}
