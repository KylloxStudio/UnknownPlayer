using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PhotonView PV;

    public float speed;
    public int dirc;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime * dirc);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (PV.IsMine)
        {
            if (collider.CompareTag("Player") && !collider.GetComponent<PhotonView>().IsMine)
            {
                PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
                collider.GetComponent<PhotonView>().RPC("OnDamaged", RpcTarget.AllBuffered, new object[] { 60, 1.7f, 0f, 0.1f, collider.transform.position });
            }
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    void DircRPC(int dirc)
    {
        this.dirc = dirc;
    }
}