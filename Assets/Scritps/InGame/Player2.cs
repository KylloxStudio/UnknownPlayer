using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Photon.Pun;

public class Player2 : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    GameManager gameManager;
    GameCamera gameCamera;

    PlayerController controller;
    PhotonView PV;

    Coroutine runningCoroutine;

    public Transform attackPoint;
    public Vector2 attackRange;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        gameCamera = GameManager.GetCamera();
        controller = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();

        runningCoroutine = null;
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                switch (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name)
                {
                    case "Player2_Attack":
                        controller.isAttacking = false;
                        anim.SetBool("isAttacking", false);
                        break;
                    case "Player2_Death":
                        Destroy(gameObject);
                        break;
                    default:
                        Debug.Log("cannot found case. " + anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                        break;
                }
            }

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
            if (controller.isAttackCanceled)
            {
                controller.isAttacking = false;
                controller.ignoreDamaged = false;
                anim.SetBool("isAttacking", false);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }

    public IEnumerator Dash(Vector2 dirc)
    {
        if (anim.GetBool("isDashing") || !controller.UseStamina(500))
        {
            yield break;
        }

        if (controller.isAttacking)
        {
            controller.isAttackCanceled = true;
        }
        anim.SetBool("isMoving", false);
        anim.SetBool("isDashing", true);

        if (controller.jumpCount >= 1)
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
        controller.isAttackCanceled = false;
    }

    public IEnumerator OnAttack()
    {
        if (controller.isAttacking || controller.isDamaged || controller.isAttackCanceled)
        {
            yield break;
        }

        controller.isAttacking = true;
        controller.ignoreDamaged = true;
        anim.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1.1f);
        controller.AttackTo(attackPoint.position, attackRange, 40, 0f, 0f, 0.05f);
        yield return new WaitForSeconds(1f);
        controller.AttackTo(attackPoint.position, attackRange, 20, 0f, 0f, 0.05f);
        yield return new WaitForSeconds(0.1f);
        controller.AttackTo(attackPoint.position, attackRange, 20, 0f, 0f, 0.05f);
        yield return new WaitForSeconds(0.1f);
        controller.AttackTo(attackPoint.position, attackRange, 20, 0f, 0f, 0.05f);
        yield return new WaitForSeconds(0.1f);
        controller.AttackTo(attackPoint.position, attackRange, 20, 0f, 0f, 0.05f);
        yield return new WaitForSeconds(0.4f);
        controller.ignoreDamaged = false;
    }
}