using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    Rigidbody2D rigid;
    Animator animator;

    GameCamera gameCamera;
    PlayerStatus status;
    public UnitCode unitCode;

    public Slider hpBar;
    public Text hpText;
    public Slider smallHpBar;
    public Slider staminaBar;
    public Text staminaText;

    public int health;
    public int stamina;

    public float imsiHealth;
    public float imsiSmallHealth;
    public float imsiStamina;

    public bool isCanJump;
    public int jumpCount;
    public bool isCanMove;
    public bool isCanChangeDirc;
    public bool isCanClimb;
    public bool isClimbing;
    public bool isAttacking;
    public bool isAttackCanceled;
    public bool isDamaged;
    public bool isDead;
    public bool ignoreDamaged;

    Vector3 curPos;
    Quaternion curRot;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameCamera = GameManager.GetCamera();

        status = new PlayerStatus().SetUnitStatus(unitCode);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            hpBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(true);
            hpBar.value = 1f;
            staminaBar.value = 1f;

            transform.rotation = NetworkManager.Instance.PlayerID == 1 ? Quaternion.Euler(0, 180, 0): Quaternion.Euler(0, 0, 0);
            transform.GetChild(2).transform.rotation = NetworkManager.Instance.PlayerID == 1 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        }

        health = status.maxHealth;
        stamina = status.maxStamina;

        isCanJump = true;
        jumpCount = 0;
        isCanMove = true;
        isCanClimb = false;
        isClimbing = false;
        isAttacking = false;
        isAttackCanceled = false;
        isDamaged = false;
        isDead = false;
        ignoreDamaged = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            HandleHpBar();
            HandleSmallHpBar();
            HandleStaminaBar();

            // Move
            isCanMove = CheckCanMove();
            if (isCanMove)
            {
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    rigid.velocity += new Vector2(0.8f, 0f);
                    //rigid.AddForce(Vector2.right * 1.1f, ForceMode2D.Impulse);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rigid.velocity += new Vector2(-0.8f, 0f);
                    //rigid.AddForce(Vector2.left * 1.1f, ForceMode2D.Impulse);
                }

                if (rigid.velocity.x > status.maxSpeed) // Right Max Speed
                {
                    rigid.velocity = new Vector2(status.maxSpeed, rigid.velocity.y);
                }
                else if (rigid.velocity.x < status.maxSpeed * (-1)) // Left Max Speed
                {
                    rigid.velocity = new Vector2(status.maxSpeed * (-1), rigid.velocity.y);
                }
            }

            // Jump
            isCanJump = CheckCanJump();
            if (isCanJump && Input.GetKeyDown(KeyCode.UpArrow))
            {
                photonView.RPC("JumpRPC", RpcTarget.All);
            }

            // Walking Animation
            if (Mathf.Abs(rigid.velocity.x) > 0.3f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            // Direction
            isCanChangeDirc = CheckCanChangeDirc();
            if (isCanChangeDirc)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    transform.GetChild(2).transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.GetChild(2).transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            // Death
            if (health <= 0 && !isDead)
            {
                Death();
            }
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100)
        {
            transform.position = curPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
            transform.rotation = curRot;
        }
    }

    void FixedUpdate()
    {
        // Fixed Camera
        Vector3 pos = Camera.main.WorldToViewportPoint(rigid.position);
        pos.x = Mathf.Clamp(pos.x, 0, 1);
        pos.y = Mathf.Clamp(pos.y, 0, 1);
        rigid.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void OnDrawGizmos()
    {
        if (rigid.velocity.y < 0f)
        {
            RaycastHit2D rayHit = Physics2D.BoxCast(rigid.position, new Vector2(0.8f, 0.04f), 0f, Vector2.down, 1.3f, LayerMask.GetMask("Platform"));
            Gizmos.color = Color.red;
            if (rayHit.collider != null)
            {
                Gizmos.DrawRay(rigid.position, Vector2.down * rayHit.distance);
                Gizmos.DrawWireCube(rigid.position + Vector2.down * rayHit.distance, new Vector2(0.8f, 0.04f));
            }
            else
            {
                Gizmos.DrawRay(rigid.position, Vector2.down * 1.3f);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (photonView.IsMine && collider.CompareTag("Ladder"))
        {
            OnClimb();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (photonView.IsMine && collider.CompareTag("Ladder"))
        {
            OffClimb();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(smallHpBar.value);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            smallHpBar.value = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void JumpRPC()
    {
        if (jumpCount >= 2) return;
        jumpCount++;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(Vector2.up * status.jumpPower, ForceMode2D.Impulse);
    }

    public void SetHealth(int value)
    {
        photonView.RPC("SetHealthRPC", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    void SetHealthRPC(int value)
    {
        health = value;
    }

    public bool UseStamina(int value)
    {
        if (stamina - value < 0)
        {
            UIManager.HighlightTextColor(staminaText, new Color(1, 0, 0));
            return false;
        }
        else
        {
            photonView.RPC("UseStaminaRPC", RpcTarget.AllBuffered, value);
            return true;
        }
    }

    [PunRPC]
    void UseStaminaRPC(int value)
    {
        stamina -= value;
    }

    private void HandleHpBar()
    {
        hpText.text = health + " / " + status.maxHealth;
        if (health > 0)
        {
            imsiHealth = (float)health / (float)status.maxHealth;
            hpBar.value = Mathf.Lerp(hpBar.value, imsiHealth, Time.deltaTime * 10);
        }
        else
        {
            Image hpFill = hpBar.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>();
            hpFill.color = new Color(1, 0, 0);
            hpBar.direction = Slider.Direction.RightToLeft;
            hpBar.value = Mathf.Lerp(hpBar.value, 1, Time.deltaTime * 10);
        }
    }

    private void HandleSmallHpBar()
    {
        smallHpBar.transform.rotation = transform.rotation.y == 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        if (health > 0)
        {
            imsiSmallHealth = (float)health / (float)status.maxHealth;
            smallHpBar.value = Mathf.Lerp(smallHpBar.value, imsiHealth, Time.deltaTime * 10);
        }
        else
        {
            Image hpFill = smallHpBar.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
            photonView.RPC("ChangeSmallHpBarColor", RpcTarget.All, new object[] { 1, 0, 0 });
            smallHpBar.direction = Slider.Direction.RightToLeft;
            smallHpBar.value = Mathf.Lerp(smallHpBar.value, 1, Time.deltaTime * 10);
        }
    }

    [PunRPC]
    void ChangeSmallHpBarColor(int r, int g, int b)
    {
        Image hpFill = smallHpBar.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
        hpFill.color = new Color(r, g, b);
    }

    private void HandleStaminaBar()
    {
        imsiStamina = (float)stamina / (float)status.maxStamina;
        staminaText.text = stamina + " / " + status.maxStamina;
        staminaBar.value = Mathf.Lerp(staminaBar.value, imsiStamina, Time.deltaTime * 10);
    }

    private bool CheckCanMove()
    {
        if (!isAttacking && !isDamaged && !isDead && !animator.GetBool("isDashing") && !Input.GetKey(KeyCode.LeftShift))
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
        if (rigid.velocity.y < 0f)
        {
            RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.04f), 0f, Vector2.down, 1.3f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                jumpCount = 0;
            }
        }
        if (jumpCount < 2 && !isDamaged && !isDead && !isCanClimb && !animator.GetBool("isDashing") && !animator.GetBool("isAttacking_03") && !animator.GetBool("isAttacking_05"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckCanChangeDirc()
    {
        if (!isDamaged && !isDead && !animator.GetBool("isDashing") && !animator.GetBool("isAttacking_04") && !animator.GetBool("isAttacking_05"))
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
        isCanClimb = true;
        //if (transform.position.y < 0.55f)
        //{
        //    isCanClimb = true;
        //}
        //else
        //{
        //    isCanClimb = false;
        //}

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (isCanClimb)
            {
                isClimbing = true;
                status.maxSpeed = 3.5f;
                rigid.drag = 10f;
                rigid.gravityScale = 0;
                GetComponent<CapsuleCollider2D>().enabled = false;
                transform.Translate(transform.up * status.climbSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (isCanClimb)
            {
                isClimbing = true;
                status.maxSpeed = 3.5f;
                rigid.drag = 10f;
                rigid.gravityScale = 0;
                GetComponent<CapsuleCollider2D>().enabled = false;
                transform.Translate(transform.up * status.climbSpeed * -1 * Time.deltaTime);
            }
        }
    }

    private void OffClimb()
    {
        isCanClimb = false;
        if (isClimbing)
        {
            isClimbing = false;
            status.maxSpeed = 8.25f;
            GetComponent<CapsuleCollider2D>().enabled = true;
            rigid.gravityScale = 5f;
            rigid.drag = 1f;
        }
    }

    public void AttackTo(Vector3 pos, Vector2 range, int damage, float intensityX, float intensityY, float time)
    {
        if (isDamaged || isAttackCanceled)
        {
            return;
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos, range, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Player") && !collider.GetComponent<PhotonView>().IsMine)
            {
                collider.GetComponent<PhotonView>().RPC("OnDamaged", RpcTarget.AllBuffered, new object[] { damage, intensityX, intensityY, time, transform.position });
            }
        }

        float shakePower = 0.4f;
        if (intensityX / 32.5f + intensityY / 32.5f > 0)
        {
            shakePower = intensityX / 32.5f + intensityY / 32.5f;
        }
        float shakeTime = shakePower > 0.7f ? 0.1f : 0.05f;
        gameCamera.VibrateForTime(shakePower, shakeTime);
    }

    [PunRPC]
    public void OnDamaged(int damage, float intensityX, float intensityY, float time, Vector3 targetPos)
    {
        if (isDamaged || ignoreDamaged)
        {
            return;
        }

        if (damage >= health)
        {
            Death();
            return;
        }

        isDamaged = true;
        if (isAttacking)
        {
            isAttackCanceled = true;
        }
        animator.SetBool("isMoving", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isDamaged", true);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        Vector2 force = new Vector2(dirc, 0) * intensityX + new Vector2(0, 1) * intensityY;
        rigid.AddForce(force, ForceMode2D.Impulse);

        SetHealth(health - damage);
        if (photonView.IsMine)
        {
            gameCamera.VibrateForTime(0.25f, 0.4f);
        }
        Invoke("OffDamaged", time);
    }

    public void OffDamaged()
    {
        isDamaged = false;
        if (isAttackCanceled)
        {
            isAttackCanceled = false;
        }
        animator.SetBool("isDamaged", false);
    }

    public void Death()
    {
        health = 0;
        isDead = true;
        StopAllCoroutines();
        if (photonView.IsMine)
        {
            gameCamera.VibrateForTime(0.35f, 0.4f);
        }

        isAttacking = false;
        isClimbing = false;
        isCanClimb = false;

        animator.SetBool("isAttacking", false);
        animator.SetBool("isAttacking_01", false);
        animator.SetBool("isAttacking_02", false);
        animator.SetBool("isAttacking_03", false);
        animator.SetBool("isAttacking_04", false);
        animator.SetBool("isAttacking_05", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("isDashing", false);
        animator.SetTrigger("onDeath");
    }
}
