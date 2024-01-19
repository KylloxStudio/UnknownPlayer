using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Photon.Pun;

public class Player1 : MonoBehaviour
{
    public PhotonView PV;

    BoxCollider2D[] boxColliders;
    Rigidbody2D rigid;
    Animator animator;

    PlayerController controller;

    Coroutine runningCoroutine;

    public Transform attackPoint;
    public Vector2 attackRange;
    public Transform bulletPos;

    public float doubleTabSecond = 0.25f;
    private bool isOneTab = false;
    private double doubleTabTimer = 0;

    // Start is called before the first frame update
    void Awake()
    {
        boxColliders = GetComponents<BoxCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();

        runningCoroutine = null;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                switch (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name)
                {
                    case "Player1_Attack_01":
                        controller.isAttacking = false;
                        animator.SetBool("isAttacking_01", false);
                        break;
                    case "Player1_Attack_02":
                        controller.isAttacking = false;
                        animator.SetBool("isAttacking_02", false);
                        break;
                    case "Player1_Attack_03":
                        controller.isAttacking = false;
                        animator.SetBool("isAttacking_03", false);
                        break;
                    case "Player1_Attack_04":
                        controller.isAttacking = false;
                        animator.SetBool("isAttacking_04", false);
                        break;
                    case "Player1_Attack_05":
                        controller.isAttacking = false;
                        animator.SetBool("isAttacking_05", false);
                        break;
                    case "Player1_Death":
                        PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
                        break;
                    default:
                        //Debug.Log("cannot found case. " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                        break;
                }
            }

            if (isOneTab && ((Time.time - doubleTabTimer) > doubleTabSecond))
            {
                isOneTab = false;
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!isOneTab)
                {
                    doubleTabTimer = Time.time;
                    isOneTab = true;
                }
                else if (isOneTab && ((Time.time - doubleTabTimer) < doubleTabSecond))
                {
                    isOneTab = false;
                    runningCoroutine = StartCoroutine(Dash(Vector2.left));
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!isOneTab)
                {
                    doubleTabTimer = Time.time;
                    isOneTab = true;
                }
                else if (isOneTab && ((Time.time - doubleTabTimer) < doubleTabSecond))
                {
                    isOneTab = false;
                    runningCoroutine = StartCoroutine(Dash(Vector2.right));
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
            if (controller.isAttackCanceled)
            {
                controller.isAttacking = false;
                controller.ignoreDamaged = false;
                animator.SetBool("isAttacking_01", false);
                animator.SetBool("isAttacking_02", false);
                animator.SetBool("isAttacking_03", false);
                animator.SetBool("isAttacking_04", false);
                animator.SetBool("isAttacking_05", false);
                foreach (BoxCollider2D collider in boxColliders)
                {
                    collider.offset = new Vector2(0, 0.004f);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (PV.IsMine && collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (animator.GetBool("isDashing"))
            {
                attackRange = new Vector2(1.1f, 2.58f);
                attackPoint.localPosition = new Vector3(0, 0.005f);
                float intensityY = rigid.velocity.y > 7.6f ? rigid.velocity.y * 1.8f : 0;
                controller.AttackTo(attackPoint.position, attackRange, 20, 1.2f, intensityY, 0.2f);
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }

    public IEnumerator Dash(Vector2 dirc)
    {
        if (animator.GetBool("isDashing") || !controller.UseStamina(500))
        {
            yield break;
        }

        if (controller.isAttacking)
        {
            controller.isAttackCanceled = true;
        }
        animator.SetBool("isMoving", false);
        animator.SetBool("isDashing", true);

        if (controller.jumpCount >= 1)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.normalized.y);
        }
        transform.rotation = dirc.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        yield return new WaitForSeconds(0.1f);
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigid.AddForce(Vector2.up * 12.5f, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            rigid.AddForce(Vector2.down * 12.5f, ForceMode2D.Impulse);
        }
        rigid.AddForce(dirc * 37.5f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        rigid.gravityScale = 5f;
        rigid.velocity = new Vector2(rigid.velocity.normalized.x * 1.75f, rigid.velocity.y);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isDashing", false);
        controller.isAttackCanceled = false;
    }

    public IEnumerator OnAttack(int type)
    {
        if (!PV.IsMine || controller.isAttacking || controller.isDamaged || controller.isAttackCanceled)
        {
            yield break;
        }

        if (type == 1)
        {
            controller.isAttacking = true;
            animator.SetBool("isAttacking_01", true);
            attackPoint.localPosition = new Vector3(0.19f, 0.05f);
            attackRange = new Vector2(4.8f, 3.9f);
            yield return new WaitForSeconds(0.6f);
            controller.AttackTo(attackPoint.position, attackRange, 20, 0f, 0f, 0.2f);
        }
        if (type == 2)
        {
            if (!controller.UseStamina(100))
            {
                yield break;
            }
            controller.isAttacking = true;
            animator.SetBool("isAttacking_02", true);
            attackPoint.localPosition = new Vector3(0.23f, 0.028f);
            attackRange = new Vector2(2.8f, 1.6f);
            yield return new WaitForSeconds(0.4f);
            controller.AttackTo(attackPoint.position, attackRange, 20, 8.4f, 1.2f, 0.15f);
            PhotonNetwork.Instantiate("bullet", bulletPos.position, Quaternion.identity).GetComponent<PhotonView>().RPC("DircRPC", RpcTarget.All, transform.rotation.y == 0 ? 1 : -1);
            yield return new WaitForSeconds(1f);
            controller.AttackTo(attackPoint.position, attackRange, 20, 8.4f, 1.2f, 0.15f);
            PhotonNetwork.Instantiate("bullet", bulletPos.position, Quaternion.identity).GetComponent<PhotonView>().RPC("DircRPC", RpcTarget.All, transform.rotation.y == 0 ? 1 : -1);
        }
        if (type == 3)
        {
            if (controller.health <= 20)
            {
                UIManager.HighlightTextColor(controller.hpText, new Color(1, 0, 0));
                yield break;
            }
            if (controller.jumpCount >= 1 || !controller.UseStamina(75))
            {
                yield break;
            }
            controller.SetHealth(controller.health - 20);
            controller.isAttacking = true;
            animator.SetBool("isAttacking_03", true);
            attackPoint.localPosition = new Vector3(0, 0);
            attackRange = new Vector2(5.8f, 2.7f);
            yield return new WaitForSeconds(1.1f);
            rigid.gravityScale = 0;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.32f);
            }
            controller.AttackTo(attackPoint.position, attackRange, 150, 3.8f, 23.4f, 0.3f);
            yield return new WaitForSeconds(0.9f);
            rigid.gravityScale = 5f;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }
        if (type == 4)
        {
            if (!controller.UseStamina(250))
            {
                yield break;
            }
            controller.isAttacking = true;
            animator.SetBool("isAttacking_04", true);
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            attackRange = new Vector2(5.8f, 1.6f);
            yield return new WaitForSeconds(0.9f);
            controller.ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            controller.AttackTo(attackPoint.position, attackRange, 30, 0, 8.2f, 0.1f);
            yield return new WaitForSeconds(0.2f);
            controller.ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            controller.AttackTo(attackPoint.position, attackRange, 70, 0, 16.4f, 0.1f);
            yield return new WaitForSeconds(1.1f);
            controller.ignoreDamaged = true;
            attackPoint.localPosition = new Vector3(-0.06f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            controller.AttackTo(attackPoint.position, attackRange, 30, 0, 9.8f, 0.1f);
            yield return new WaitForSeconds(0.2f);
            controller.ignoreDamaged = false;
            attackPoint.localPosition = new Vector3(0.15f, 0.015f);
            yield return new WaitForSeconds(0.1f);
            controller.AttackTo(attackPoint.position, attackRange, 70, 0, 18.6f, 0.1f);
            yield return new WaitForSeconds(0.8f);
            controller.ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            controller.ignoreDamaged = false;
        }
        if (type == 5)
        {
            if (controller.jumpCount >= 1 || !controller.UseStamina(250))
            {
                yield break;
            }
            controller.isAttacking = true;
            animator.SetBool("isAttacking_05", true);
            controller.ignoreDamaged = true;
            yield return new WaitForSeconds(0.2f);
            controller.ignoreDamaged = false;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(-0.25f, 0.004f);
            }
            attackPoint.localPosition = new Vector3(0.21f, 0.32f);
            attackRange = new Vector2(3.6f, 9.1f);
            yield return new WaitForSeconds(0.6f);
            controller.AttackTo(attackPoint.position, attackRange, 240, 1.7f, 26.3f, 0.3f);
            yield return new WaitForSeconds(1f);
            controller.ignoreDamaged = true;
            yield return new WaitForSeconds(0.3f);
            controller.ignoreDamaged = false;
            foreach (BoxCollider2D collider in boxColliders)
            {
                collider.offset = new Vector2(0, 0.004f);
            }
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }
}