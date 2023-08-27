using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickMatchCancel()
    {
        NetworkManager.Instance.socket.Emit("quit");
        SceneManager.LoadScene("MainScene");
    }
}
