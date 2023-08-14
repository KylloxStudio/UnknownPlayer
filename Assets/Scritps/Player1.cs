using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player1 : MonoBehaviour
{
    private BoxCollider2D[] boxColliders;
    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private GameManager gameManager;
    private Grid grid;
    private Tilemap tilemap;
    private GameCamera gameCamera;

    public float maxSpeed;
    public float jumpPower;
    public float climbSpeed;
    public int health;
    public int stamina;
    public bool is1StepJumping;
    public bool is2StepJumping;
    public bool isAttacking;
    public bool isClimbing;
    public bool isCanJump;
    public bool isCanMove;
    public bool isCanClimb;
    public bool isDamaged;
    public bool isDead;
    public bool ignoreDamaged;

    public Vector3Int cellTilePos;
    public Vector3 worldTilePos;
    public Vector2 victimPos;
    public Transform attackPoint;
    public Vector2 attackRange;
    public GameObject bullet;
    public Transform bulletPos;

    // Start is called before the first frame update
    private void Start()
    {
        boxColliders = GetComponents<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        grid = gameManager.GetGrid();
        tilemap = gameManager.GetTilemap();
        gameCamera = gameManager.GetCamera();

        maxSpeed = 8.25f;
        jumpPower = 12.75f;
        climbSpeed = 8.25f;
        health = 500;
        stamina = 2000;
        is1StepJumping = false;
        is2StepJumping = false;
        isAttacking = false;
        isClimbing = false;
        isCanJump = true;
        isCanMove = true;
        isCanClimb = false;
        isDamaged = false;
        isDead = false;

        cellTilePos = new Vector3Int();
        worldTilePos = new Vector3();
        victimPos = new Vector2();
    }

    private void Update()
    {
        // Attack Animation
        if (anim != null)
        {
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

            /*
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Attack_Down") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                isAttacking = false;
                anim.SetBool("isAttacking_Down", false);
                anim.SetFloat("DownAttackSpeed", 1f);
            }
            */

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Player1_Death") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                Destroy(gameObject);
            }
        }

        // Check Can Jump
        if (isDamaged || isDead || is2StepJumping || isCanClimb || anim.GetBool("isAttacking_03") || anim.GetBool("isAttacking_05") || anim.GetBool("isDashing"))
        {
            isCanJump = false;
        }
        else
        {
            isCanJump = true;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.W) && isCanJump)
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

        // Down Attack
        /*
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (stamina - 50 < 0)
            {
                if (!gameManager.isChangingTextColor)
                {
                    StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                }
                return;
            }

            if (isAttacking)
            {
                return;
            }

            float distance = Vector2.Distance(transform.position, victimPos);
            if (victimPos == new Vector2())
            {
                distance = Vector2.Distance(transform.position, worldTilePos);
            }

            if (distance <= 5.8f)
            {
                return;
            }

            if (distance <= 10f)
            {
                anim.SetFloat("DownAttackSpeed", 2f);
            }

            rigid.AddForce(Vector2.down * (distance + 10f), ForceMode2D.Impulse);

            isAttacking = true;
            UseStamina(50);
            anim.SetBool("isAttacking_Down", true);
        }
        */

        // Check Can Move
        if (Input.GetKey(KeyCode.LeftShift) || isAttacking || isDamaged || isDead || anim.GetBool("isDashing"))
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
            if (Input.GetKey(KeyCode.A))
            {
                rigid.AddForce(Vector2.left * 1.2f, ForceMode2D.Impulse);
            }
            else
            {
                rigid.AddForce(Vector2.left * 0.97f, ForceMode2D.Impulse);
            }

            if (Input.GetKey(KeyCode.D))
            {
                rigid.AddForce(Vector2.right * 1.2f, ForceMode2D.Impulse);
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

        // Direction
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

        // Walking Animation
        if (!anim.GetBool("isDashing"))
        {
            if (Mathf.Abs(rigid.velocity.x) > 0.3f)
            {
                anim.SetBool("isMoving", true);
            }
            else
            {
                anim.SetBool("isMoving", false);
            }
        }

        // Normal Attack
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            if (isAttacking || isDamaged || anim.GetBool("isDashing"))
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(OnAttack(1));
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                if (stamina - 100 < 0)
                {
                    if (!gameManager.isChangingTextColor)
                    {
                        StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                    }
                    return;
                }
                StartCoroutine(OnAttack(2));
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (is1StepJumping)
                {
                    return;
                }

                if (stamina - 75 < 0)
                {
                    if (!gameManager.isChangingTextColor)
                    {
                        StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                    }
                    return;
                }
                StartCoroutine(OnAttack(3));
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Dash
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (anim.GetBool("isDashing"))
                {
                    return;
                }

                if (stamina - 500 < 0)
                {
                    if (!gameManager.isChangingTextColor)
                    {
                        StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                    }
                    return;
                }

                if (isAttacking)
                {
                    isAttacking = false;
                    anim.SetBool("isAttacking_01", false);
                    anim.SetBool("isAttacking_02", false);
                    anim.SetBool("isAttacking_03", false);
                    anim.SetBool("isAttacking_04", false);
                    anim.SetBool("isAttacking_05", false);
                    anim.SetBool("isAttacking_Down", false);
                }

                anim.SetBool("isMoving", false);
                StartCoroutine(Dash());
            }

            // Special Attack
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (isAttacking || isDamaged)
                {
                    return;
                }

                if (stamina - 250 < 0)
                {
                    if (!gameManager.isChangingTextColor)
                    {
                        StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                    }
                    return;
                }

                StartCoroutine(OnAttack(4));
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (is1StepJumping || isAttacking || isDamaged)
                {
                    return;
                }

                if (stamina - 250 < 0)
                {
                    if (!gameManager.isChangingTextColor)
                    {
                        StartCoroutine(gameManager.ChangeTextColor(gameManager.GetText("stamina"), new Color(1, 0, 0)));
                    }
                    return;
                }

                StartCoroutine(OnAttack(5));
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

        /*
        // Debug.DrawRay(rigid.position, new Vector3(0, -20, 0), new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector2.down, 20);
        if (rayHit.collider != null)
        {
            if (rayHit.collider.gameObject != gameObject && rayHit.collider.CompareTag("Player"))
            {
                victimPos = new Vector2(rayHit.collider.transform.position.x, rayHit.collider.transform.position.y);
            }
            else
            {
                victimPos = new Vector2();
            }

            if (rayHit.collider.CompareTag("Platform"))
            {
                cellTilePos = grid.WorldToCell(new Vector3(transform.position.x, transform.position.y));
                if (tilemap.HasTile(new Vector3Int(cellTilePos.x, -3, 0)))
                {
                    worldTilePos = grid.CellToWorld(new Vector3Int(cellTilePos.x, -3, 0));
                }
            }
            else if (rayHit.collider.CompareTag("HighPlatform"))
            {
                worldTilePos = new Vector3(rayHit.collider.transform.position.x, rayHit.collider.transform.position.y);
            }
        }
        */
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
                    maxSpeed = 1.75f;
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
                    maxSpeed = 1.75f;
                    rigid.gravityScale = 0;
                    GetComponents<BoxCollider2D>()[0].enabled = false;
                    transform.Translate(transform.up * climbSpeed * -1 * Time.deltaTime);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            isCanClimb = false;
            if (isClimbing)
            {
                isClimbing = false;
                maxSpeed = 8.25f;
                GetComponents<BoxCollider2D>()[0].enabled = true;
                rigid.gravityScale = 5f;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            OnDamaged(50, 7.5f, 0.4f, collision.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }

    private void UseStamina(int value)
    {
        if (stamina - value < 0)
        {
            stamina = 0;
            return;
        }
        stamina -= value;
    }

    private void SetHealth(int value)
    {
        health += value;
    }

    private IEnumerator Dash()
    {
        UseStamina(500);
        anim.SetBool("isDashing", true);
        if (is1StepJumping)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.normalized.y);
        }

        yield return new WaitForSeconds(0.25f);
        if (Input.GetKey(KeyCode.W))
        {
            rigid.AddForce(Vector2.up * 13.5f, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rigid.AddForce(Vector2.down * 13.5f, ForceMode2D.Impulse);
        }
        rigid.AddForce((transform.rotation.y == 0 ? Vector2.right : Vector2.left) * 35f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.25f);
        rigid.gravityScale = 5f;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.75f, rigid.velocity.y);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isDashing", false);
    }

    private IEnumerator OnAttack(int type)
    {
        isAttacking = true;
        if (type == 1)
        {
            anim.SetBool("isAttacking_01", true);
            attackPoint.localPosition = new Vector3(0.19f, 0.05f);
            attackRange = new Vector2(4.8f, 3.9f);
            yield return new WaitForSeconds(0.6f);
            AttackTo(attackPoint.position, attackRange, 10, 0, 0);
        }
        if (type == 2)
        {
            UseStamina(100);
            anim.SetBool("isAttacking_02", true);
            attackPoint.localPosition = new Vector3(0.23f, 0.028f);
            attackRange = new Vector2(2.8f, 1.6f);
            yield return new WaitForSeconds(0.4f);
            AttackTo(attackPoint.position, attackRange, 10, 8.4f, 1.2f);
            Instantiate(bullet, bulletPos.position, transform.rotation);
            yield return new WaitForSeconds(1f);
            AttackTo(attackPoint.position, attackRange, 10, 8.4f, 1.2f);
            Instantiate(bullet, bulletPos.position, transform.rotation);
        }
        if (type == 3)
        {
            SetHealth(-20);
            UseStamina(75);
            anim.SetBool("isAttacking_03", true);
            attackPoint.localPosition = new Vector3(0, 0);
            attackRange = new Vector2(5.8f, 2.7f);
            yield return new WaitForSeconds(1.1f);
            rigid.gravityScale = 0;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.32f);
            }
            AttackTo(attackPoint.position, attackRange, 75, 3.8f, 23.4f);
            yield return new WaitForSeconds(0.9f);
            rigid.gravityScale = 5f;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }
        if (type == 4)
        {
            UseStamina(250);
            anim.SetBool("isAttacking_04", true);
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            attackRange = new Vector2(5.8f, 1.6f);
            yield return new WaitForSeconds(0.9f);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            AttackTo(attackPoint.position, attackRange, 15, 0, 8.2f);
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            AttackTo(attackPoint.position, attackRange, 35, 0, 16.4f);
            yield return new WaitForSeconds(1.1f);
            ignoreDamaged = true;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            AttackTo(attackPoint.position, attackRange, 15, 0, 9.8f);
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            AttackTo(attackPoint.position, attackRange, 35, 0, 18.6f);
            yield return new WaitForSeconds(0.8f);
            ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            ignoreDamaged = false;
        }
        if (type == 5)
        {
            UseStamina(250);
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
            AttackTo(attackPoint.position, attackRange, 120, 1.7f, 26.3f);
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
        if (isDamaged)
        {
            return;
        }

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos, range, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                collider.GetComponent<Enemy>().OnDamaged(damage, intensityX, intensityY, transform.position);
            }
        }

        float shakePower = 0.4f;
        if (intensityX + intensityY != 0)
        {
            shakePower = intensityX / 35 + intensityY / 35;
        }
        float shakeTime = shakePower > 0.7f ? 0.1f : 0.05f;
        gameCamera.VibrateForTime(shakePower, shakeTime);
    }

    public void OnDamaged(int damage, float intensity, float time, Vector2 targetPos)
    {
        if (isDamaged || isDead || ignoreDamaged)
        {
            return;
        }

        StopAllCoroutines();

        isDamaged = true;
        isAttacking = false;

        anim.SetBool("isAttacking_01", false);
        anim.SetBool("isAttacking_02", false);
        anim.SetBool("isAttacking_03", false);
        anim.SetBool("isAttacking_04", false);
        anim.SetBool("isAttacking_05", false);
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
        anim.ResetTrigger("doDamaged");
    }

    private void Death()
    {
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
}