using System.Collections;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    private Rigidbody2D rigid;
    public Animator anim;
    private GameManager gameManager;
    private UIManager ui;
    private GameCamera gameCamera;

    private PlayerController controller;

    public int health;
    public int stamina;

    public bool isAttacking;
    public bool isAttackCanceled;
    public bool isDamaged;
    public bool isDead;
    public bool ignoreDamaged;

    private Coroutine runningCoroutine;

    public Transform attackPoint;
    public Vector2 attackRange;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        ui = UIManager.Instance;
        gameCamera = GameManager.GetCamera();
        controller = GetComponent<PlayerController>();

        health = 1000;
        stamina = 5000;

        isAttacking = false;
        isAttackCanceled = false;
        isDamaged = false;
        isDead = false;

        runningCoroutine = null;
    }

    private void Update()
    {
        // Animations
        if (anim != null)
        {
            // 경고 메세지 방지
            anim.SetBool("isAttacking_01", anim.GetBool("isAttacking"));
            anim.SetBool("isAttacking_02", anim.GetBool("isAttacking"));
            anim.SetBool("isAttacking_03", anim.GetBool("isAttacking"));
            anim.SetBool("isAttacking_04", anim.GetBool("isAttacking"));
            anim.SetBool("isAttacking_05", anim.GetBool("isAttacking"));

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player2_Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }

        if (controller != null)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Attack
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    runningCoroutine = StartCoroutine(OnAttack());
                }
            }
            else
            {
                // Dash
                if (Input.GetKeyDown(KeyCode.A))
                {
                    runningCoroutine = StartCoroutine(Dash(Vector2.left));
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    runningCoroutine = StartCoroutine(Dash(Vector2.right));
                }
            }

            // Attack Cancel
            if (isAttackCanceled)
            {
                isAttacking = false;
                ignoreDamaged = false;
                anim.SetBool("isAttacking", false);
            }

            // Death
            if (health <= 0 && !isDead)
            {
                Death();
            }
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

        // Debug.DrawRay(rigid.position, new Vector3(0, -20, 0), new Color(0, 1, 0));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject)
        {
            OnDamaged(50, 7.5f, 0.4f, collision.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }

    public void SetHealth(int value)
    {
        health += value;
    }

    public bool UseStamina(int value)
    {
        if (stamina - value < 0)
        {
            ui.HighlightTextColor(gameManager.staminaText, new Color(1, 0, 0));
            return false;
        }
        else
        {
            stamina -= value;
            return true;
        }
    }

    public IEnumerator Dash(Vector2 dirc)
    {
        if (anim.GetBool("isDashing") || !UseStamina(500))
        {
            yield break;
        }

        if (isAttacking)
        {
            isAttackCanceled = true;
        }
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", true);

        if (controller.is1StepJumping)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.normalized.y);
        }
        transform.rotation = dirc.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.25f);
        if (Input.GetKey(KeyCode.W))
        {
            rigid.AddForce(Vector2.up * 13.5f, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rigid.AddForce(Vector2.down * 13.5f, ForceMode2D.Impulse);
        }
        rigid.AddForce(dirc * 64f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        rigid.gravityScale = 5f;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.75f, rigid.velocity.y);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isDashing", false);
        isAttackCanceled = false;
    }

    public IEnumerator OnAttack()
    {
        if (isAttacking || isDamaged || isAttackCanceled)
        {
            yield break;
        }

        isAttacking = true;
        ignoreDamaged = true;
        anim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1.1f);
        AttackTo(attackPoint.position, attackRange, 40, 0, 0);
        yield return new WaitForSeconds(1f);
        AttackTo(attackPoint.position, attackRange, 20, 0, 0);
        yield return new WaitForSeconds(0.1f);
        AttackTo(attackPoint.position, attackRange, 20, 0, 0);
        yield return new WaitForSeconds(0.1f);
        AttackTo(attackPoint.position, attackRange, 20, 0, 0);
        yield return new WaitForSeconds(0.1f);
        AttackTo(attackPoint.position, attackRange, 20, 0, 0);
        yield return new WaitForSeconds(0.4f);
        ignoreDamaged = false;
    }

    private void AttackTo(Vector3 pos, Vector2 range, int damage, float intensityX, float intensityY)
    {
        if (isDamaged || isAttackCanceled)
        {
            return;
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos, range, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.gameObject.CompareTag("Player") && collider.gameObject != gameObject)
            {
                collider.GetComponent<Player1>().OnDamaged(damage, intensityX, intensityY, transform.position);
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

    public void OnDamaged(int damage, float intensity, float time, Vector2 targetPos)
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

        StopAllCoroutines();

        isDamaged = true;
        if (isAttacking)
        {
            isAttackCanceled = true;
        }
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", false);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * intensity, ForceMode2D.Impulse);

        SetHealth(-damage);
        anim.SetBool("isDamaged", true);
        gameCamera.VibrateForTime(0.25f, 0.4f);
        Invoke("OffDamaged", time);
    }

    public void OffDamaged()
    {
        isDamaged = false;
        if (isAttackCanceled)
        {
            isAttackCanceled = false;
        }
        anim.SetBool("isDamaged", false);
    }

    public void Death()
    {
        health = 0;
        isDead = true;
        StopAllCoroutines();

        isAttacking = false;
        controller.isClimbing = false;
        controller.isCanClimb = false;

        anim.SetBool("isAttacking", false);
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", false);
        anim.SetBool("isDead", true);
    }
}