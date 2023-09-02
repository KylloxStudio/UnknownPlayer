using System.Collections;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    private BoxCollider2D[] boxColliders;
    private Rigidbody2D rigid;
    private Animator anim;
    private GameManager gameManager;
    private UIManager ui;
    private GameCamera gameCamera;

    public float maxSpeed;
    public float jumpPower;
    public float climbSpeed;
    public int health;
    public int stamina;
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
    private Coroutine runningCoroutine;

    public Transform attackPoint;
    public Vector2 attackRange;
    public GameObject bullet;
    public Transform bulletPos;

    // Start is called before the first frame update
    private void Start()
    {
        boxColliders = GetComponents<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        ui = GameManager.GetUIManager();
        gameCamera = GameManager.GetCamera();

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
    }

    private void Update()
    {
        // Animations
        if (anim != null)
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

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_01") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_01", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_02") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_02", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_03") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_03", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_04") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_04", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_05") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_05", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }

        // Check Can Jump
        isCanJump = CheckCanJump();

        // Jump
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

        // Check Can Move
        isCanMove = CheckCanMove();

        // Move
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

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            // Normal Attack
            if (Input.GetKeyDown(KeyCode.Z))
            {
                runningCoroutine = StartCoroutine(OnAttack(1));
            }
            // Skill 1
            else if (Input.GetKeyDown(KeyCode.X))
            {
                runningCoroutine = StartCoroutine(OnAttack(2));
            }
            // Skill 2
            else if (Input.GetKeyDown(KeyCode.C))
            {
                runningCoroutine = StartCoroutine(OnAttack(3));
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

            // Special Skill 1
            if (Input.GetKeyDown(KeyCode.X))
            {
                runningCoroutine = StartCoroutine(OnAttack(4));
            }

            // Special Skill 2
            if (Input.GetKeyDown(KeyCode.C))
            {
                runningCoroutine = StartCoroutine(OnAttack(5));
            }
        }

        // Attack Cancel
        if (isAttackCanceled)
        {
            isAttacking = false;
            ignoreDamaged = false;
            anim.SetBool("isAttacking_01", false);
            anim.SetBool("isAttacking_02", false);
            anim.SetBool("isAttacking_03", false);
            anim.SetBool("isAttacking_04", false);
            anim.SetBool("isAttacking_05", false);
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }

        // Death
        if (health <= 0 && !isDead)
        {
            Death();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject != gameObject)
        {
            if (anim.GetBool("isDashing"))
            {
                attackRange = new Vector2(1.1f, 2.58f);
                attackPoint.localPosition = new Vector3(0, 0.005f);
                float intensityY = rigid.velocity.y > 7.6f ? rigid.velocity.y * 1.8f : 0;
                AttackTo(attackPoint.position, attackRange, 20, 0, intensityY);
                return;
            }
            OnDamaged(50, 7.5f, 0.4f, collision.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
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

    private bool UseStamina(int value)
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

    private void SetHealth(int value)
    {
        health += value;
    }

    private IEnumerator Dash(Vector2 dirc)
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

        if (is1StepJumping)
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
        rigid.AddForce(dirc * 32.5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        rigid.gravityScale = 5f;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.75f, rigid.velocity.y);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isDashing", false);
        isAttackCanceled = false;
    }

    private IEnumerator OnAttack(int type)
    {
        if (isAttacking || isDamaged || isAttackCanceled)
        {
            yield break;
        }

        if (type == 1)
        {
            isAttacking = true;
            anim.SetBool("isAttacking_01", true);
            attackPoint.localPosition = new Vector3(0.19f, 0.05f);
            attackRange = new Vector2(4.8f, 3.9f);
            yield return new WaitForSeconds(0.6f);
            AttackTo(attackPoint.position, attackRange, 20, 0, 0);
        }
        if (type == 2)
        {
            if (!UseStamina(100))
            {
                yield break;
            }
            isAttacking = true;
            anim.SetBool("isAttacking_02", true);
            attackPoint.localPosition = new Vector3(0.23f, 0.028f);
            attackRange = new Vector2(2.8f, 1.6f);
            yield return new WaitForSeconds(0.4f);
            AttackTo(attackPoint.position, attackRange, 20, 8.4f, 1.2f);
            Instantiate(bullet, bulletPos.position, transform.rotation);
            yield return new WaitForSeconds(1f);
            AttackTo(attackPoint.position, attackRange, 20, 8.4f, 1.2f);
            Instantiate(bullet, bulletPos.position, transform.rotation);
        }
        if (type == 3)
        {
            if (health <= 20)
            {
                ui.HighlightTextColor(gameManager.hpText, new Color(1, 0, 0));
                yield break;
            }
            if (is1StepJumping || !UseStamina(75))
            {
                yield break;
            }
            SetHealth(-20);
            isAttacking = true;
            anim.SetBool("isAttacking_03", true);
            attackPoint.localPosition = new Vector3(0, 0);
            attackRange = new Vector2(5.8f, 2.7f);
            yield return new WaitForSeconds(1.1f);
            rigid.gravityScale = 0;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.32f);
            }
            AttackTo(attackPoint.position, attackRange, 150, 3.8f, 23.4f);
            yield return new WaitForSeconds(0.9f);
            rigid.gravityScale = 5f;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }
        if (type == 4)
        {
            if (!UseStamina(250))
            {
                yield break;
            }
            isAttacking = true;
            anim.SetBool("isAttacking_04", true);
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            attackRange = new Vector2(5.8f, 1.6f);
            yield return new WaitForSeconds(0.9f);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            AttackTo(attackPoint.position, attackRange, 30, 0, 8.2f);
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            AttackTo(attackPoint.position, attackRange, 70, 0, 16.4f);
            yield return new WaitForSeconds(1.1f);
            ignoreDamaged = true;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            AttackTo(attackPoint.position, attackRange, 30, 0, 9.8f);
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            AttackTo(attackPoint.position, attackRange, 70, 0, 18.6f);
            yield return new WaitForSeconds(0.8f);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
        }
        if (type == 5)
        {
            if (is1StepJumping || !UseStamina(250))
            {
                yield break;
            }
            isAttacking = true;
            anim.SetBool("isAttacking_05", true);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(-0.25f, 0.004f);
            }
            attackPoint.localPosition = new Vector3(0.21f, 0.32f);
            attackRange = new Vector2(3.6f, 9.1f);
            yield return new WaitForSeconds(0.6f);
            AttackTo(attackPoint.position, attackRange, 240, 1.7f, 26.3f);
            yield return new WaitForSeconds(1f);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.3f);
            ignoreDamaged = false;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }
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
        anim.SetTrigger("doDamaged");
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
        anim.ResetTrigger("doDamaged");
    }

    private void Death()
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
}