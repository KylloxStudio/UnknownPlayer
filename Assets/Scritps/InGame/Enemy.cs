using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;

    public int hp;

    // Start is called before the first frame update
    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void OnDamaged(int damage, float intensityX, float intensityY, Vector2 targetPos)
    {
        hp -= damage;
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc * intensityX, intensityY), ForceMode2D.Impulse);
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public void OffDamaged()
    {
        anim.ResetTrigger("doDamaged");
    }
}