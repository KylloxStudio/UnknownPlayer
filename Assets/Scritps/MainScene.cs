using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    public Text title;
    public Text startMsg;

    private bool isActiveTitle;
    private bool isActiveStartMsg;

    // Start is called before the first frame update
    void Start()
    {
        title.text = "";
        UIManager.Instance.TypingEffect(title, "Unknown Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveTitle && title.text == "Unknown Player")
        {
            isActiveTitle = true;
        }

        if (isActiveTitle && !isActiveStartMsg)
        {
            startMsg.gameObject.SetActive(true);
            UIManager.Instance.BlinkEffect(startMsg, 0.5f, false);
            isActiveStartMsg = true;
        }

        if (isActiveStartMsg)
        {
            if (Input.anyKeyDown)
            {
                NetworkManager.Instance.socket.Emit("enter");
                SceneManager.LoadScene("LobbyScene");
            }
        }
    }
}
