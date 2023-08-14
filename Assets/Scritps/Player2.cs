using System.Collections;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    public float maxSpeed;
    public float jumpPower;
    public float stamina;
    public bool is1StepJumping;
    public bool is2StepJumping;
    public bool isCanJump;
    public bool isCanMove;
    public float health;
    public Vector2 platformPosition;

    // Start is called before the first frame update
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        maxSpeed = 10f;
        jumpPower = 15f;
        stamina = 2000f;
        is1StepJumping = false;
        is2StepJumping = false;
        isCanJump = true;
        isCanMove = true;
        health = 500f;
        platformPosition = new Vector2(0, 0);
    }

    private void Update()
    {
        // Attack Animation
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Attack_01") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            anim.SetBool("isAttacking_01", false);
            anim.SetFloat("AttackSpeed", 1f);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Attack_02") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            anim.SetBool("isAttacking_02", false);
            anim.SetFloat("AttackSpeed", 1f);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Attack_03") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            anim.SetBool("isAttacking_03", false);
            anim.SetFloat("AttackSpeed", 1f);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Attack_Down") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            anim.SetBool("isAttacking_Down", false);
            anim.SetFloat("AttackSpeed", 1f);
        }

        // Jump
        if (!(anim.GetBool("isAttacking_01") || anim.GetBool("isAttacking_02") || anim.GetBool("isAttacking_03")))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && isCanJump)
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

        // Down Attack
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (stamina <= 0)
            {
                return;
            }

            float distance = Vector2.Distance(transform.position, platformPosition);
            if (distance <= 3f)
            {
                return;
            }

            if (distance <= 9.3f)
            {
                anim.SetFloat("AttackSpeed", 2f);
            }
            rigid.AddForce(Vector2.down * (distance + 10f), ForceMode2D.Impulse);
            SetStamina(-50f);
            anim.SetBool("isAttacking_Down", true);
        }

        // Check Can Move
        if (Input.GetKey(KeyCode.RightShift) || anim.GetBool("isAttacking_01") || anim.GetBool("isAttacking_02") || anim.GetBool("isAttacking_03") || anim.GetBool("isAttacking_Down") || anim.GetBool("isDashing"))
        {
            isCanMove = false;
        }
        else
        {
            isCanMove = true;
        }

        // Move
        if (isCanMove)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rigid.AddForce(Vector2.left * 1.2f, ForceMode2D.Impulse);
                anim.SetBool("isMoving", true);
            }
            else
            {
                rigid.AddForce(Vector2.left * 0.97f, ForceMode2D.Impulse);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                rigid.AddForce(Vector2.right * 1.2f, ForceMode2D.Impulse);
                anim.SetBool("isMoving", true);
            }
            else
            {
                rigid.AddForce(Vector2.right * 0.97f, ForceMode2D.Impulse);
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

        // Player Direction
        if (!(anim.GetBool("isAttacking_03") || anim.GetBool("isDashing")))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                spriteRenderer.flipX = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                spriteRenderer.flipX = false;
            }
        }

        // Walking Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
        {
            anim.SetBool("isMoving", false);
        }

        // Normal Attack
        if (!Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Period))
        {
            if (stamina <= 0 || is1StepJumping)
            {
                return;
            }

            if (anim.GetBool("isAttacking_01") || anim.GetBool("isAttacking_02") || anim.GetBool("isAttacking_03") || anim.GetBool("isAttacking_Down"))
            {
                return;
            }

            int random = Random.Range(1, 100);
            if (random < 50)
            {
                anim.SetBool("isAttacking_01", true);
                anim.SetBool("isAttacking_02", false);
            }
            else if (random < 100)
            {
                anim.SetBool("isAttacking_01", false);
                anim.SetBool("isAttacking_02", true);
            }

            SetStamina(-100f);
        }

        if (Input.GetKey(KeyCode.RightShift))
        {
            // Dash
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (stamina <= 0 || anim.GetBool("isDashing"))
                {
                    return;
                }

                anim.SetBool("isMoving", false);
                anim.SetBool("isDashing", true);
                StartCoroutine(Dash());
            }

            // Special Attack
            if (Input.GetKeyDown(KeyCode.Period))
            {
                if (stamina <= 0 || is1StepJumping)
                {
                    return;
                }

                if (anim.GetBool("isAttacking_01") || anim.GetBool("isAttacking_02") || anim.GetBool("isAttacking_03") || anim.GetBool("isAttacking_Down"))
                {
                    return;
                }

                anim.SetBool("isAttacking_03", true);
                SetStamina(-250f);
            }
        }

        // Death
        if (health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Fixed Camera
        Vector3 pos = Camera.main.WorldToViewportPoint(rigid.position);
        pos.x = Mathf.Clamp(pos.x, 0, 1);
        pos.y = Mathf.Clamp(pos.y, 0, 1);
        rigid.position = Camera.main.ViewportToWorldPoint(pos);

        // Landing
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 15);
        if (rayHit.collider != null)
        {
            platformPosition = new Vector2(rayHit.collider.transform.position.x, rayHit.collider.transform.position.y);
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

    private void SetStamina(float value)
    {
        stamina += value;
    }

    private void SetHealth(float value)
    {
        health += value;
    }

    private IEnumerator Dash()
    {
        SetStamina(-500f);
        if (is1StepJumping)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.normalized.y);
        }

        yield return new WaitForSeconds(0.25f);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigid.AddForce(Vector2.up * 15f, ForceMode2D.Impulse);
            isCanJump = false;
        }
        if (!spriteRenderer.flipX) // Right
        {
            rigid.AddForce(Vector2.right * 35f, ForceMode2D.Impulse);
        }
        else // Left
        {
            rigid.AddForce(Vector2.left * 35f, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(0.25f);
        rigid.gravityScale = 5f;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.75f, rigid.velocity.y);
        yield return new WaitForSeconds(0.35f);
        anim.SetBool("isDashing", false);
        isCanJump = true;
    }

    public IEnumerator OnDamage(float damage)
    {
        SetHealth(-damage);
        isCanMove = false;
        isCanJump = false;
        spriteRenderer.color = new Color(255, 0, 0);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = new Color(255, 255, 255);
        yield return new WaitForSeconds(0.1f);
        isCanMove = true;
        isCanJump = true;
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(0.25f);
        is1StepJumping = true;
        is2StepJumping = true;
        isCanMove = false;
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", false);
        anim.SetBool("isAttacking_01", false);
        anim.SetBool("isAttacking_02", false);
        anim.SetBool("isAttacking_03", false);
        anim.SetBool("isAttacking_Down", false);
        anim.SetBool("isDead", true);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Destroy(gameObject);
        }
    }
}