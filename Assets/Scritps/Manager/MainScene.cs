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
    private void Start()
    {
        title.text = "";
        UIManager.Instance.TypingEffect(title, "Unknown Player");

        if (!NetworkManager.Instance.socket.Connected && !NetworkManager.Instance.isReconneting)
            NetworkManager.Instance.socket.Connect();
    }

    // Update is called once per frame
    private void Update()
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
                NetworkManager.Instance.socket.Emit("join");
                SceneManager.LoadScene("LobbyScene");
            }
        }
    }
}