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

    private void Awake()
    {
        Instance = this;

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

        StartCoroutine(SendPlayerData());
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Instance.isPlayer1)
        {
            health = player1.health;
            stamina = player1.stamina;
            isAttacking = player1.isAttacking;
            isAttackCanceled = player1.isAttackCanceled;
            isDamaged = player1.isDamaged;
            isDead = player1.isDead;
        }
        else if (NetworkManager.Instance.isPlayer2)
        {
            health = player2.health;
            stamina = player2.stamina;
            isAttacking = player2.isAttacking;
            isAttackCanceled = player2.isAttackCanceled;
            isDamaged = player2.isDamaged;
            isDead = player2.isDead;
        }

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

    private IEnumerator SendPlayerData()
    {
        yield return new WaitUntil(() => NetworkManager.Instance.isGameLoaded);
        while (NetworkManager.Instance.isGameLoaded)
        {
            if (!isAttacking && !isDamaged)
                SendStatistics();
            SendPosition();
            SendRotation();
            SendAnimation();
            yield return null;
        }
    }

    public void SendStatistics()
    {
        JObject data = new JObject();
        data.Add("hp1", player1.health);
        data.Add("stamina1", player1.stamina);
        data.Add("hp2", player2.health);
        data.Add("stamina2", player2.stamina);
        NetworkManager.Instance.socket.Emit("statistics", data.ToString());
    }

    public void SendPosition()
    {
        JObject data = new JObject();
        data.Add("x1", player1.transform.position.x);
        data.Add("y1", player1.transform.position.y);
        data.Add("x2", player2.transform.position.x);
        data.Add("y2", player2.transform.position.y);
        NetworkManager.Instance.socket.Emit("position", data.ToString());
    }

    public void SendRotation()
    {
        JObject data = new JObject();
        data.Add("y1", player1.transform.rotation.y != 0 ? 180 : 0);
        data.Add("y2", player2.transform.rotation.y != 0 ? 180 : 0);
        NetworkManager.Instance.socket.Emit("rotation", data.ToString());
    }

    public void SendAnimation()
    {
        JObject data = new JObject();
        string[] animList = { "isMoving", "isDashing", "isAttacking", "isAttacking_01", "isAttacking_02", "isAttacking_03", "isAttacking_04", "isAttacking_05", "isDamaged", "isDead" };
        for (int i = 0; i < animList.Length; i++)
        {
            data.Add("p1_" + animList[i], player1.anim.GetBool(animList[i]));
            data.Add("p2_" + animList[i], player2.anim.GetBool(animList[i]));
        }
        NetworkManager.Instance.socket.Emit("animation", data.ToString());
    }
}
